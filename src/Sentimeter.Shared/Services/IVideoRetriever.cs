using Sentimeter.Web.Models;

namespace Sentimeter.Shared.Services;

public interface IVideoRetriever
{
    Task<DiscoveryVideoInformationResponseModel?> DiscoveryVideoInformationAsync(DiscoveryVideoInformationModel model);
}
