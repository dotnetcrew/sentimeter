﻿namespace Sentimeter.Web.Models.Videos;

public class VideoDetailModel
{
    public Guid Id { get; set; }

    public string Identifier { get; set; } = string.Empty;

    public string? ThumbnailUrl { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? PublishedAt { get; set; }

    public string CommentsSummary { get; set; } = string.Empty;
}
