using Sentimeter.Web.Models;

namespace Sentimeter.Web.Api.Services;

public interface IVideoEndpointsService
{
    Task<Guid> RegisterVideoAsync(RegisterVideoModel model, string userId);
}
