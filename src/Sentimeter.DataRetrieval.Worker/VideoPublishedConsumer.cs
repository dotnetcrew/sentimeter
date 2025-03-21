using Akka.Actor;
using Akka.Hosting;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Sentimeter.DataRetrieval.Worker.Akka.Actors;
using Sentimeter.DataRetrieval.Worker.Akka.Messages;
using Sentimeter.DataRetrieval.Worker.Services;
using Sentimeter.Shared.Messages;

internal class VideoPublishedConsumer : IConsumer<VideoPublishedMessage>
{
    private readonly ILogger<VideoPublishedConsumer> _logger;
    private readonly IRequiredActor<ManagerWorkerActor> _managerWorkerActor;
    private readonly IVideoAndCommentService _videoAndCommentService;

    public VideoPublishedConsumer(ILogger<VideoPublishedConsumer> logger,
        IRequiredActor<ManagerWorkerActor> managerWorkerActor,
        IVideoAndCommentService videoAndCommentService)
    {
        _logger = logger;
        _managerWorkerActor = managerWorkerActor;
        _videoAndCommentService = videoAndCommentService;
    }
    public async Task Consume(ConsumeContext<VideoPublishedMessage> context)
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