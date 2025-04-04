using Microsoft.Extensions.AI;
using System.Text.Json;
using Sentimeter.Analysis.Worker.Services;
using Sentimeter.Core;
using System.Diagnostics;
using Sentimeter.Analysis.Worker.Models;

namespace Sentimeter.Analysis.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IChatClient _client;
    private readonly List<ChatMessage> _messageHistory;
    private int _executionCount;
    private long _secondsToWait;

    public Worker(ILogger<Worker> logger,
        IServiceProvider serviceProvider,
        IChatClient client
        )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _client = client;

        //Test to instruct the AI to classify the sentiment of a comment as 'positive', 'negative' or 'neutral' using System User and Assistant roles
        _messageHistory = [new ChatMessage(ChatRole.System, "Classifica il sentiment del seguente commento come 'positivo', 'negativo' o 'neutro'. Il sentiment è positivo se il commento esprime approvazione, entusiasmo o soddisfazione; è negativo se contiene critiche, insoddisfazione o disapprovazione; è neutro se è oggettivo, ambiguo o privo di emozioni forti. Restituisci solo il JSON contenente la classe identificata in questo formato { “sentiment”: “classe“, ”score”: ”numero score” } dove “classe” è il valore del sentiment classificato e lo score contiene un valore tra 0 e 1 che esprime la percentuale di confidenza della classificazione."),
                                                        new (ChatRole.User, "Analizza il commento seguente: Adoro questo prodotto! La qualità è eccellente, la spedizione è stata rapidissima e il servizio clienti è stato super disponibile. Lo consiglio assolutamente!"),
                                                        new (ChatRole.Assistant, """{"sentiment":"positivo", "score": 0.87}"""),
                                                        new (ChatRole.User,"Analizza il seguente commento: Il contenuto è ok, ma non offre nulla di particolarmente innovativo. Va bene, ma non mi ha impressionato più di tanto."),
                                                        new (ChatRole.Assistant, """{"sentiment":"neutro", "score": 0.90}"""),
                                                        new (ChatRole.User,"Analizza il seguente commento: Non mi è piaciuto. Il tema è interessante, ma la scrittura è poco coinvolgente e il contenuto poco originale."),
                                                        new (ChatRole.Assistant, """{"sentiment":"negativo", "score": 0.97}"""),
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


    private async Task DoWork()
    {
        int count = Interlocked.Increment(ref _executionCount);
        _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);

        using var scope = _serviceProvider.CreateScope();
        var videoAndCommentResult = scope.ServiceProvider.GetRequiredService<IVideoAndCommentResult>();

        // Retrive All Videos
        var videos = videoAndCommentResult.GetAllVideos();
        var stopwatch = new Stopwatch();
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
                    var charResponse = await _client.CompleteAsync([.. _messageHistory, new(ChatRole.User, "Analizza il seguente commento:" + comment.Content)]);
                    stopwatch.Stop();
                    _logger.LogInformation($"Message: [{comment.Content}] with response: [{charResponse}] (Elapsed time: {stopwatch.ElapsedMilliseconds} ms)");

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var sentimentResult = JsonSerializer.Deserialize<SentimentResult>(charResponse.Message.Text, options);
                    if (sentimentResult != null)
                    {
                        _logger.LogInformation($"Sentiment: {sentimentResult.Sentiment}, Score: {sentimentResult.Score}");
                        // Save the sentiment result to the database or perform other actions
                        await videoAndCommentResult.SaveCommentResultAsync(new CommentResultModel
                        {
                            CommentId = comment.Id,
                            Result = sentimentResult.Sentiment,
                            LastUpdate = DateTime.UtcNow,
                            Score = sentimentResult.Score
                            
                        });

                    }
                    else
                    {
                        _logger.LogWarning($"Failed to deserialize sentiment result: {charResponse.Message.Text}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error during sentiment analysis of {comment.Content} with commentId: {comment.Id} and videoId: {comment.VideoId} with exception: {ex.Message}");
                }

            }

        }

        
    }

}
