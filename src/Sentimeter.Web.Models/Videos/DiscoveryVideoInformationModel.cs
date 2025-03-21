namespace Sentimeter.Web.Models.Videos;

public record DiscoveryVideoInformationModel(
    string VideoId);

public record DiscoveryVideoInformationResponseModel(
    string Title,
    string? Description,
    DateTime? PublishedAt,
    string ThumbnailUrl);

public record Comment(
    string CommentIdentifier,
    string AuthorDisplayName,
    string TextDisplay,
    DateTimeOffset? UpdatedAtDateTimeOffset,
    List<Comment>? replies
    );

public record DiscoveryVideoCommentsModel(
    string VideoIdentifier,
    uint MaxResults,
    string? LastCommentId,
    DateTimeOffset? LastUpdate
    );

public record DiscoveryVideoCommentsResponseModel(List<Comment> comments);
