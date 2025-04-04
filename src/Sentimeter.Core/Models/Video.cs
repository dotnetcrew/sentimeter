namespace Sentimeter.Core.Models;

public class Video
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the youtube video id
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? PublishedAt { get; set; }

    public string? ThumbnailUrl { get; set; }

    public virtual IList<Comment> Comments { get; set; } = [];
    
    public virtual VideoSentimentResult SentimentResult { get; set; } = default;
}
