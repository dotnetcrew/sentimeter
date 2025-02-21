using MassTransit;
using Sentimeter.Shared.Messages;

internal class VideoPublishedConsumer : IConsumer<VideoPublishedMessage>
{
    private readonly ILogger<VideoPublishedConsumer> _logger;

    public VideoPublishedConsumer(ILogger<VideoPublishedConsumer> logger)
    {
        _logger = logger;
    }
    public Task Consume(ConsumeContext<VideoPublishedMessage> context)
    {
        _logger.LogInformation($"Video published: {context.Message.videoId}");

        return Task.CompletedTask;
    }
}