namespace Sentimeter.Core.Models;

public class VideoSentimentResult
{
    public Guid Id { get; set; }
    public Guid VideoId { get; set; }
    public virtual Video Video { get; set; } = default!;
    public DateTime LastUpdate { get; set; }
    public string Result { get; set; } = string.Empty;
}
