using Sentimeter.Web.Models;

namespace Sentimeter.Shared.Services;

public interface IVideoRetriever
{
    Task<DiscoveryVideoInformationResponseModel?> DiscoveryVideoInformationAsync(DiscoveryVideoInformationModel model);
    Task<DiscoveryVideoCommentsResponseModel?> DiscoveryVideoCommmentsAsync(DiscoveryVideoCommentsModel model);
}
