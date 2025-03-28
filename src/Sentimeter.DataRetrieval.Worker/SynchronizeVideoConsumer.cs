using Akka.Actor;
using Akka.Hosting;
using MassTransit;
using Sentimeter.DataRetrieval.Worker.Akka.Actors;
using Sentimeter.DataRetrieval.Worker.Akka.Messages;
using Sentimeter.DataRetrieval.Worker.Services;
using Sentimeter.Shared.Messages.RabbitMQ;

internal class SynchronizeVideoConsumer : IConsumer<SynchronizeVideoMessage>
{
    private readonly ILogger<SynchronizeVideoConsumer> _logger;
    private readonly IRequiredActor<ManagerWorkerActor> _managerWorkerActor;
    private readonly IVideoAndCommentService _videoAndCommentService;

    public SynchronizeVideoConsumer(ILogger<SynchronizeVideoConsumer> logger,
        IRequiredActor<ManagerWorkerActor> managerWorkerActor,
        IVideoAndCommentService videoAndCommentService)
    {
        _logger = logger;
        _managerWorkerActor = managerWorkerActor;
        _videoAndCommentService = videoAndCommentService;
    }
    public async Task Consume(ConsumeContext<SynchronizeVideoMessage> context)
    {

        _logger.LogInformation($"Video published: {context.Message.Identifier}");

        var managerWorkerActor = await _managerWorkerActor.GetAsync();

        if (managerWorkerActor is null || managerWorkerActor.IsNobody())
        {
            _logger.LogWarning("managerWorkerActorRef is nobody");
            return;
        }

        // Retrive video Identifier from DB

        Guid? videoId = _videoAndCommentService.GetVideoIdFromVideoIdentifier(context.Message.Identifier);

        if (videoId != null)
        {
            managerWorkerActor.Tell(new RetriveVideoCommentsMessage(context.Message.Identifier, videoId.Value, null, null));
        }

    }
}