using Sentimeter.Web.Models;

namespace Sentimeter.Web.Api.Services;

public interface IVideoRetriever
{
    Task<DiscoveryVideoInformationResponseModel?> DiscoveryVideoInformationAsync(DiscoveryVideoInformationModel model);
}
