namespace Sentimeter.Web.Models;

public record DiscoveryVideoInformationModel(
    string VideoId);

public record DiscoveryVideoInformationResponseModel(
    string Title,
    string? Description,
    DateTime? PublishedAt,
    string ThumbnailUrl);
