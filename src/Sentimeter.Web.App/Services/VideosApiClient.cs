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

    public async Task RegisterVideoAsync(RegisterVideoModel model)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(ResourceEndpoint, model);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }

    public async Task<VideoListModel> GetVideosAsync(int page, int size)
    {
        var model = await httpClient.GetFromJsonAsync<VideoListModel>($"{ResourceEndpoint}?page={page}&size={size}");
        return model ?? new();
    }

    public async Task<VideoDetailModel?> GetVideoDetailAsync(Guid videoId)
    {
        var model = await httpClient.GetFromJsonAsync<VideoDetailModel>($"{ResourceEndpoint}/{videoId}");
        return model;
    }
}
