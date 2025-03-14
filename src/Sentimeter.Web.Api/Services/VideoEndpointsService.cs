using Sentimeter.Core;
using Sentimeter.Core.Models;
using Sentimeter.Web.Models;

namespace Sentimeter.Web.Api.Services;

public class VideoEndpointsService : IVideoEndpointsService
{
    private readonly SentimeterDbContext _context;

    public VideoEndpointsService(SentimeterDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Guid> RegisterVideoAsync(RegisterVideoModel model, string userId)
    {
        var video = new Video
        {
            Description = model.Description,
            Title = model.Title,
            PublishedAt = model.PublishedAt,
            Identifier = model.VideoId,
            UserId = userId,
            ThumbnailUrl = model.ThumbnailUrl
        };

        _context.Videos.Add(video);
        await _context.SaveChangesAsync();

        return video.Id;
    }
}
