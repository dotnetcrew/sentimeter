using Microsoft.EntityFrameworkCore;
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

    public async Task<VideoListModel> GetVideosAsync(int page, int size, string userId)
    {
        var skip = (page - 1) * size;

        var videoQuery = _context.Videos.AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.PublishedAt);

        var totalNumberOfVideos = await videoQuery.CountAsync();

        var numberOfPages = (int)Math.Ceiling((double)totalNumberOfVideos / size);

        var videos = await videoQuery
            .Skip(skip)
            .Take(size)
            .Select(x => new VideoListModel.VideoItemDescriptor(
                x.Id,
                x.Title,
                x.ThumbnailUrl,
                x.Identifier))
            .ToArrayAsync();

        var model = new VideoListModel
        {
            IsFirstPage = page == 1,
            Items = videos,
            NumberOfPages = numberOfPages,
            HasNextPage = page < numberOfPages
        };

        return model;
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
