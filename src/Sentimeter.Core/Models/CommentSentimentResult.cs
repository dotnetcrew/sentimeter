namespace Sentimeter.Core.Models;

public class CommentSentimentResult
{
    public Guid Id { get; set; }
    public Guid CommentId { get; set; }
    public virtual Comment Comment { get; set; } = default!;
    public DateTime LastUpdate { get; set; }
    public string Result { get; set; } = string.Empty;
}
