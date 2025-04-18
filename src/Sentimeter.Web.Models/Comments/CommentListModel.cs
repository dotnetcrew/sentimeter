namespace Sentimeter.Web.Models.Comments;

public class CommentListModel
{
    public IEnumerable<CommentItemDescriptor> Items { get; set; } = [];

    public int NumberOfPages { get; set; }

    public bool HasNextPage { get; set; }

    public bool IsFirstPage { get; set; }

    public record CommentItemDescriptor(
        Guid Id,
        string Author,
        string Content,
        DateTime? LastUpdate,
        SentimentResult? SentimentResult);

    public record SentimentResult(
        string Result,
        double Score);
}
