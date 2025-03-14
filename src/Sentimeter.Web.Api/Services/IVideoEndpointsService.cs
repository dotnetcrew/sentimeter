using Sentimeter.Web.Models;

namespace Sentimeter.Web.Api.Services;

public interface IVideoEndpointsService
{
    Task<VideoListModel> GetVideosAsync(int page, int size, string userId);

    Task<Guid> RegisterVideoAsync(RegisterVideoModel model, string userId);
}
