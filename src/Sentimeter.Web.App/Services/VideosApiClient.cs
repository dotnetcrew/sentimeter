using OperationResults;
using Sentimeter.Web.Models.Videos;

namespace Sentimeter.Web.App.Services;

public class VideosApiClient(HttpClient httpClient)
{
    private const string ResourceEndpoint = "api/videos";

    public async Task<Result<DiscoveryVideoInformationResponseModel>> DiscoveryVideoInformationAsync(DiscoveryVideoInformationModel model)
    {
        var response = await httpClient.PostAsJsonAsync($"{ResourceEndpoint}/discovery", model);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
            var errors = error?.Errors
                .Select(x => new ValidationError(x.Key, x.Value.FirstOrDefault() ?? string.Empty))
                .ToArray() ?? [];

            return Result.Fail(FailureReasons.ClientError, "There was an error while retrieving video informations", errors);
        }

        var responseModel = await response.Content.ReadFromJsonAsync<DiscoveryVideoInformationResponseModel>();
        return responseModel!;

    }

    public async Task<Result> RegisterVideoAsync(RegisterVideoModel model)
    {
        var response = await httpClient.PostAsJsonAsync(ResourceEndpoint, model);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
            var errors = error?.Errors
                .Select(x => new ValidationError(x.Key, x.Value.FirstOrDefault() ?? string.Empty))
                .ToArray() ?? [];

            return Result.Fail(FailureReasons.ClientError, "There was an error while registering the informations of the video", errors);
        }

        return Result.Ok();
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
