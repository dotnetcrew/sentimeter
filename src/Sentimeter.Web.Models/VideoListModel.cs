namespace Sentimeter.Web.Models;

public class VideoListModel
{
    public IEnumerable<VideoItemDescriptor> Items { get; set; } = [];

    public int NumberOfPages { get; set; }

    public bool HasNextPage { get; set; }

    public bool IsFirstPage { get; set; }

    public record VideoItemDescriptor(
        Guid Id,
        string Title,
        string? ThumbnailUrl,
        string VideoIdentifier);
}
