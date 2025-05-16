using Sentimeter.Web.Models.Videos;

namespace Sentimeter.Web.Api.Services;

public interface IVideoEndpointsService
{
    Task<VideoDetailModel?> GetVideoDetailAsync(Guid videoId, string userId);

    Task<VideoListModel> GetVideosAsync(int page, int size, string userId);
    Task<VideoStatsModel> GetVideoStatsAsync();
    Task<Guid> RegisterVideoAsync(RegisterVideoModel model, string userId);
}
