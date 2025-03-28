namespace Sentimeter.Shared.Messages.RabbitMQ;

public record SynchronizeVideoMessage(
    Guid VideoId,
    string Identifier);
