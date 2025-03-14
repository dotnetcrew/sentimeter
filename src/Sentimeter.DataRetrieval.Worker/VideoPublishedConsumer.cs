using Akka.Actor;
using Akka.Hosting;
using MassTransit;
using Sentimeter.DataRetrieval.Worker.Akka.Actors;
using Sentimeter.DataRetrieval.Worker.Akka.Messages;
using Sentimeter.Shared.Messages;

internal class VideoPublishedConsumer : IConsumer<VideoPublishedMessage>
{
    private readonly ILogger<VideoPublishedConsumer> _logger;
    private readonly IRequiredActor<ManagerWorkerActor> _managerWorkerActor;

    public VideoPublishedConsumer(ILogger<VideoPublishedConsumer> logger,
        IRequiredActor<ManagerWorkerActor> managerWorkerActor)
    {
        _logger = logger;
        _managerWorkerActor = managerWorkerActor;
    }
    public async Task Consume(ConsumeContext<VideoPublishedMessage> context)
    {

        _logger.LogInformation($"Video published: {context.Message.VideoId}");

        var managerWorkerActor = await _managerWorkerActor.GetAsync();

        if (managerWorkerActor is null || managerWorkerActor.IsNobody())
        {
            _logger.LogWarning("managerWorkerActorRef is nobody");
            return;
        }

        managerWorkerActor.Tell(new RetriveVideoCommentsMessage(context.Message.VideoId,null,null));

    }
}