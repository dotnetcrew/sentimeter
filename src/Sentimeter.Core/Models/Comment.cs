namespace Sentimeter.Core.Models;

public class Comment
{
    public Guid Id { get; set; }

    public string Author { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public virtual Video Video { get; set; } = default!;

    public virtual IList<Comment> Replies { get; set; } = [];
}
