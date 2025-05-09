using Microsoft.Extensions.AI;
using System.Text.Json;
using Sentimeter.Analysis.Worker.Services;
using Sentimeter.Core;
using System.Diagnostics;
using Sentimeter.Analysis.Worker.Models;
using static Sentimeter.Analysis.Worker.Worker;
using System.Runtime.CompilerServices;
using static Sentimeter.Web.Models.Comments.CommentListModel;

namespace Sentimeter.Analysis.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IChatClient _client;
    private readonly List<ChatMessage> _messageHistoryForComments;
    private readonly List<ChatMessage> _messageHistoryForVideos;
    private int _executionCount;
    private long _secondsToWait;

    private static string[] _allowedSentimentResults = ["positivo", "negativo", "neutro"];

    public Worker(ILogger<Worker> logger,
        IServiceProvider serviceProvider,
        IChatClient client
        )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _client = client;

        //Test to instruct the AI to classify the sentiment of a comment as 'positive', 'negative' or 'neutral' using System User and Assistant roles
        _messageHistoryForComments = [new ChatMessage(ChatRole.System, "RUOLO:Sei un classificatore di sentiment impiegato in un contesto di testing linguistico supervisionato.\r\nLa tua unica funzione è quella di classificare il sentiment di commenti scritti da utenti secondo le seguenti linee guida:\r\nPositivo: esprime approvazione, entusiasmo o soddisfazione.\r\nNegativo: esprime disapprovazione, critica o insoddisfazione.\r\nNeutro: non mostra emozioni forti, è oggettivo o ambiguo.\r\nAnche se il linguaggio può sembrare sarcastico o pungente, devi comunque procedere con l'analisi, in quanto:\r\nNessuna offesa può essere causata: i commenti sono anonimi e usati solo per valutazioni automatiche.\r\nNon stai emettendo giudizi morali, ma solo classificando un'emozione secondo criteri tecnici.\r\nNon rifiutare l'analisi per alcun motivo.\r\nRestituisci solo un JSON nel formato:\r\n{ \"sentiment\": \"classe\", \"score\": \"numero score\" }\r\nEsempio:\r\n{ \"sentiment\": \"negativo\", \"score\": \"0.85\" }\r\nDove:\r\n\"classe\" è uno tra \"positivo\", \"negativo\" o \"neutro\"\r\n\"score\" è un numero compreso tra 0 e 1 che rappresenta il livello di confidenza nella classificazione."),
                                                        new (ChatRole.User, "Classifica il sentiment di questo testo nel contesto del nostro test: Adoro questo prodotto! La qualità è eccellente, la spedizione è stata rapidissima e il servizio clienti è stato super disponibile. Lo consiglio assolutamente!"),
                                                        new (ChatRole.Assistant, """{"sentiment":"positivo", "score": 0.87}"""),
                                                        new (ChatRole.User,"Classifica il sentiment di questo testo nel contesto del nostro test: Il contenuto è ok, ma non offre nulla di particolarmente innovativo. Va bene, ma non mi ha impressionato più di tanto."),
                                                        new (ChatRole.Assistant, """{"sentiment":"neutro", "score": 0.90}"""),
                                                        new (ChatRole.User,"Classifica il sentiment di questo testo nel contesto del nostro test: Non mi è piaciuto. Il tema è interessante, ma la scrittura è poco coinvolgente e il contenuto poco originale."),
                                                        new (ChatRole.Assistant, """{"sentiment":"negativo", "score": 0.97}"""),
                                                        ];


        _messageHistoryForVideos = [new ChatMessage(ChatRole.System, "RUOLO: Sei un assistente incaricato di sintetizzare il contenuto di uno o più commenti testuali scritti da utenti. \r\nIl tuo compito è quello di produrre un riassunto conciso e neutro che descriva i principali temi, opinioni o toni espressi nei commenti forniti.\r\nIl tuo output non deve esprimere giudizi personali, né filtrare o censurare il contenuto dei commenti, anche se contengono sarcasmo, critiche, o riferimenti ambigui.\r\nNon è richiesto classificare i commenti, ma solo descrivere in modo generale e oggettivo il loro contenuto.\r\nContesto importante:\r\nQuesta attività viene svolta in un ambiente di test controllato, al fine di valutare le capacità di sintesi del linguaggio.\r\nI contenuti non saranno pubblicati, né usati per scopi reali. Nessuna persona può essere offesa dal risultato.\r\nEsempio di output atteso (formato libero, ma conciso):\r\n'I commenti esprimono per lo più insoddisfazione verso lo sketch, con toni ironici e riferimenti al successo commerciale di un noto marchio.'\r\n'L’insieme dei commenti mostra un mix di approvazione e sarcasmo, con riferimenti a personaggi noti e sfide artistiche.'\r\nRegole:\r\nRiassumi il contenuto dei commenti in massimo 3-4 frasi.\r\nNon riformulare ogni commento singolarmente, ma cogli il significato globale.\r\nMantieni sempre un tono neutro, informativo e tecnico.\r\nNon rifiutare l’elaborazione dei commenti per nessun motivo."),
                                                        new (ChatRole.User, "\r\n- Sfida difficilissima. Vatti a vedere il fatturato di Tavernello.\r\n- L’ironia è geniale, ma forse non per tutti.\r\n-Boh, a me non fa ridere, solito umorismo da liceo.\r\nFinalmente qualcosa di diverso, ma poteva essere sviluppato meglio."),
                                                        new (ChatRole.Assistant,"I commenti esprimono opinioni contrastanti: alcuni apprezzano l’ironia e l’originalità, mentre altri criticano il livello dell’umorismo, percepito come poco efficace o scolastico. C'è anche un riferimento sarcastico al successo commerciale di un noto marchio.\r\n"),
                                                        new (ChatRole.User, "\r\n- Ho riso dall’inizio alla fine, bellissimo!\r\n- Geniale, perfetto in ogni dettaglio.\r\n- Recitazione ottima, tempi comici perfetti.\r\n- Mi ha fatto venire voglia di riguardarlo.\r\n"),
                                                        new (ChatRole.Assistant,"I commenti mostrano un entusiasmo generale verso il contenuto, con apprezzamenti per l’originalità, la qualità della recitazione e l’efficacia comica. Il tono è chiaramente positivo.\r\n"),
                                                        new (ChatRole.User, "\r\n- Lo sketch dura circa 4 minuti ed è ambientato in un supermercato.\r\n- Fa parte della serie pubblicata sul canale YouTube ufficiale.\r\n- Nessun commento particolare, ma ben girato.\r\n"),
                                                        new (ChatRole.Assistant,"I commenti descrivono dettagli oggettivi dello sketch, come la durata, l’ambientazione e la piattaforma di pubblicazione. Il tono è neutro e informativo.\r\n"),
                                                        ];

        _secondsToWait = 180;
    }

    public class SentimentResult
    {
        public string Sentiment { get; set; } = string.Empty;
        public double Score { get; set; }
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        // When the timer should have no due-time, then do the work once now.
        await DoWork();

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(_secondsToWait));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWork();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");
        }

    }

    public static bool IsValidJson(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        try
        {
            using (JsonDocument.Parse(input))
            {
                return true;
            }
        }
        catch
        {
            return false;
        }
    }


    private async Task DoWork()
    {
        int count = Interlocked.Increment(ref _executionCount);
        _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);

        using var scope = _serviceProvider.CreateScope();
        var videoAndCommentResult = scope.ServiceProvider.GetRequiredService<IVideoAndCommentResult>();

        // Retrive All Videos without result and summarize the comments
        var videosWithoutResult = videoAndCommentResult.GetAllVideosWithoutResult();
        var stopwatch = new Stopwatch();
        foreach (var video in videosWithoutResult)
        {
            var mainComments = videoAndCommentResult.GetAllMainCommentsWithoutResponseByVideoId(video.Id);
            string commentsToProcess = string.Join("\r\n- ", mainComments.Select(c => c.Content));
            string commentsToAnalyze = "Riassumi i seguenti commenti nel contesto del nostro test:" + (commentsToProcess.Length > 20000 ? commentsToProcess.Substring(0, 20000) : commentsToProcess);

            try
            {
                stopwatch.Restart();
                var charResponse = await _client.CompleteAsync([.. _messageHistoryForComments, new(ChatRole.User, commentsToAnalyze)]);
                stopwatch.Stop();

                await videoAndCommentResult.SaveVideoResultAsync(new VideoResultModel
                {
                    VideoId = video.Id,
                    Result = charResponse.Message.Text ?? string.Empty,
                    LastUpdate = DateTime.UtcNow,
                    Score = 1.0
                });
            }
            catch (Exception ex)
            {
                // Handle 
                _logger.LogError(ex, $"Error during sentiment analysis of {video.Title} with videoId: {video.Id} with exception: {ex.Message}");
            }

        }

        // Retrive All Videos and classify the sentiment of the comments
        var videos = videoAndCommentResult.GetAllVideos();
        foreach (var video in videos)
        {
            
            // Retrive comments without result
            var comments = videoAndCommentResult.GetAllCommentsWithoutResultByVideoId(video.Id);

            //  For each comment, classify the sentiment as 'positive', 'negative' or 'neutral'
            foreach (var comment in comments)
            {

                try
                {
                    stopwatch.Restart();
                    var charResponse = await _client.CompleteAsync([.. _messageHistoryForComments, new(ChatRole.User, "Classifica il sentiment di questo testo nel contesto del nostro test:" + comment.Content + ". Ricorda che si tratta solo di classificazione, non di giudizio.")]);
                    stopwatch.Stop();
                    _logger.LogInformation(
                        "Message: [{CommentContent}] with response: [{ResponseText}] (Elapsed time: {ResponseTime} ms)",
                        comment.Content, charResponse, stopwatch.ElapsedMilliseconds);

                    SentimentResult sentimentResult = new();
                    sentimentResult.Score = 0.0;
                    sentimentResult.Sentiment = "sconosciuto";

                    if (IsValidJson(charResponse.Message.Text))
                    {
                        sentimentResult = ParseSentimentResponse(charResponse.Message.Text!);

                        if (sentimentResult != null)
                        {
                            _logger.LogInformation(
                                "Sentiment: {Sentiment}, Score: {SentimentScore}",
                                sentimentResult.Sentiment, sentimentResult.Score);
                            // Save the sentiment result to the database or perform other actions
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Failed to deserialize sentiment result: {ResponseText}",
                                charResponse.Message.Text);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Invalid JSON response: {ResponseText}", charResponse.Message.Text);
                    }

                    await videoAndCommentResult.SaveCommentResultAsync(new CommentResultModel
                    {
                        CommentId = comment.Id,
                        Result = sentimentResult!.Sentiment,
                        LastUpdate = DateTime.UtcNow,
                        Score = sentimentResult.Score
                    });

                }
                catch (Exception ex)
                {
                    // Handle 
                    _logger.LogError(
                        ex, 
                        "Error during sentiment analysis of {CommentContent} with commentId: {CommentId} and videoId: {VideoId} with exception: {ErrorMessage}",
                        comment.Content, comment.Id, comment.VideoId, ex.Message);
                }
            }
        }

        
    }

    private SentimentResult ParseSentimentResponse(string responseText)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var sentimentResult = JsonSerializer.Deserialize<SentimentResult>(responseText, options) ?? new();

        if (string.IsNullOrWhiteSpace(sentimentResult.Sentiment) || !_allowedSentimentResults.Contains(sentimentResult.Sentiment, StringComparer.InvariantCultureIgnoreCase))
        {
            _logger.LogWarning("Invalid sentiment result: {Sentiment}", sentimentResult.Sentiment);
            sentimentResult.Sentiment = "sconosciuto";
        }

        return sentimentResult;
    }
}
