using MassTransit;
using Microsoft.EntityFrameworkCore;
using Sentimeter.Core;
using Sentimeter.Core.Models;
using Sentimeter.Shared.Messages.RabbitMQ;
using Sentimeter.Web.Models.Videos;

namespace Sentimeter.Web.Api.Services;

public class VideoEndpointsService : IVideoEndpointsService
{
    private readonly SentimeterDbContext _context;

    private readonly IBus _bus;

    public VideoEndpointsService(SentimeterDbContext context, IBus bus)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    public async Task<VideoDetailModel?> GetVideoDetailAsync(Guid videoId, string userId)
    {
        var video = await _context.Videos.AsNoTracking()
            .Where(v => v.UserId == userId)
            .SingleOrDefaultAsync(v => v.Id == videoId);

        if (video is null)
        {
            return null;
        }

        var model = new VideoDetailModel
        {
            Description = video.Description,
            Title = video.Title,
            PublishedAt = video.PublishedAt,
            ThumbnailUrl = video.ThumbnailUrl,
            Identifier = video.Identifier,
            Id = video.Id
        };

        return model;
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
            Description = model.Description?.Length > 1024 ? model.Description.Substring(0, 1024) : model.Description,
            Title = model.Title,
            PublishedAt = model.PublishedAt?.ToUniversalTime(),
            Identifier = model.VideoId,
            UserId = userId,
            ThumbnailUrl = model.ThumbnailUrl
        };

        _context.Videos.Add(video);
        await _context.SaveChangesAsync();

        var message = new SynchronizeVideoMessage(
            video.Id,
            video.Identifier);

        await _bus.Send(message);

        return video.Id;
    }
}
