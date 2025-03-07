using Sentimeter.Web.Models;

namespace Sentimeter.Web.App.Services;

public class VideosApiClient(HttpClient httpClient)
{
    private const string ResourceEndpoint = "api/videos";

    public async Task<DiscoveryVideoInformationResponseModel> DiscoveryVideoInformationAsync(DiscoveryVideoInformationModel model)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{ResourceEndpoint}/discovery", model);
            response.EnsureSuccessStatusCode();

            var responseModel = await response.Content.ReadFromJsonAsync<DiscoveryVideoInformationResponseModel>();
            return responseModel!;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
        
    }
}
