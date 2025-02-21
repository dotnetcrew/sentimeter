using Microsoft.Extensions.AI;
using System.Diagnostics;

namespace Sentimeter.Analysis.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IChatClient _client;

    public Worker(ILogger<Worker> logger, IChatClient client)
    {
        _logger = logger;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //while (!stoppingToken.IsCancellationRequested)
        //{
        //    if (_logger.IsEnabled(LogLevel.Information))
        //    {
        //        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //    }
        //    await Task.Delay(1000, stoppingToken);
        //}

        
        //Test to instruct the AI to classify the sentiment of a comment as 'positive', 'negative' or 'neutral' using System User and Assistant roles
        
        List<ChatMessage> messageHistory = [new ChatMessage(ChatRole.System, "Classifica il sentiment del seguente commento come 'positivo', 'negativo' o 'neutro'. Il sentiment è positivo se il commento esprime approvazione, entusiasmo o soddisfazione; è negativo se contiene critiche, insoddisfazione o disapprovazione; è neutro se è oggettivo, ambiguo o privo di emozioni forti. Restituisci solo il JSON contenente la classe identificata in questo formato { “sentiment”: “classe“, ”score”: ”numero score” } dove “classe” è il valore del sentiment classificato e lo score contiene un valore tra 0 e 1 che esprime la percentuale di confidenza della classificazione."),
                                                        new (ChatRole.User, "Analizza il commento seguente: Adoro questo prodotto! La qualità è eccellente, la spedizione è stata rapidissima e il servizio clienti è stato super disponibile. Lo consiglio assolutamente!"),
                                                        new (ChatRole.Assistant, """{"sentiment":"positivo", "score": 0.87}"""),
                                                        new (ChatRole.User,"Analizza il seguente commento: Il contenuto è ok, ma non offre nulla di particolarmente innovativo. Va bene, ma non mi ha impressionato più di tanto."),
                                                        new (ChatRole.Assistant, """{"sentiment":"neutro", "score": 0.90}"""),
                                                        new (ChatRole.User,"Analizza il seguente commento: Non mi è piaciuto. Il tema è interessante, ma la scrittura è poco coinvolgente e il contenuto poco originale."),
                                                        new (ChatRole.Assistant, """{"sentiment":"negativo", "score": 0.97}"""),
                                                        ];

        var stopwatch = new Stopwatch();
        stopwatch.Start();        
        var charResponse = await _client.CompleteAsync([..messageHistory,new (ChatRole.User, "Analizza il seguente commento: Non sono riuscito ad utilizzare il prodotto perchè è troppo complicato")]);
        stopwatch.Stop();
        _logger.LogInformation($"JSON 1:{charResponse} (Elapsed time: {stopwatch.ElapsedMilliseconds} ms)");
        stopwatch.Restart();

        charResponse = await _client.CompleteAsync([..messageHistory, new(ChatRole.User, "Analizza il seguente commento: Articolo discreto, ci sono alcuni punti interessanti, ma nulla che non si trovi già facilmente online.")]);
        stopwatch.Stop();
        _logger.LogInformation($"JSON 2:{charResponse} (Elapsed time: {stopwatch.ElapsedMilliseconds} ms)");
        stopwatch.Restart();

        charResponse = await _client.CompleteAsync([.. messageHistory, new(ChatRole.User, "Analizza il seguente commento: Purtroppo l'articolo non è molto approfondito e alcune informazioni sembrano superficiali. Mi aspettavo di più.")]);
        stopwatch.Stop();
        _logger.LogInformation($"JSON 3:{charResponse} (Elapsed time: {stopwatch.ElapsedMilliseconds} ms)");
        stopwatch.Restart();

        charResponse = await _client.CompleteAsync([.. messageHistory, new(ChatRole.User, "Analizza il seguente commento: Ottimo lavoro! Le informazioni sono chiare e ben strutturate, si vede che c’è stato un grande impegno nella ricerca. Consigliato!")]);
        stopwatch.Stop();
        _logger.LogInformation($"JSON 4:{charResponse} (Elapsed time: {stopwatch.ElapsedMilliseconds} ms)");

    }
}
