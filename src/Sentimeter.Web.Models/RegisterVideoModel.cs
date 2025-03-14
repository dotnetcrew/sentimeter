using System.ComponentModel.DataAnnotations;

namespace Sentimeter.Web.Models;

public class RegisterVideoModel
{
    [Required]
    public string VideoId { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? PublishedAt { get; set; }

    public string ThumbnailUrl { get; set; } = string.Empty;
}
