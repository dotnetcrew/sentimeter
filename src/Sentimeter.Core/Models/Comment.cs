namespace Sentimeter.Core.Models;

public class Comment
{
    public Guid Id { get; set; }

    public string Identifier { get; set; } = string.Empty;

    public DateTime? LastUpdate { get; set; }

    public string Author { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public virtual Video Video { get; set; } = default!;

    public Guid VideoId { get; set; }

    public Guid? CommentId { get; set; } = null;

    public virtual IList<Comment> Replies { get; set; } = [];

    public virtual CommentSentimentResult? SentimentResult  { get; set; } = default;
}
