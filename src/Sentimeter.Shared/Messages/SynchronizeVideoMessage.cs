namespace Sentimeter.Shared.Messages;

public record SynchronizeVideoMessage(
    Guid VideoId,
    string Identifier);
