namespace Sentimeter.Core.Models;

public class Video
{
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the youtube video id
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? PublishedAt { get; set; }

    public virtual IList<Comment> Comments { get; set; } = [];
}
