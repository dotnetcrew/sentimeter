using Google.Apis.YouTube.v3;
using Sentimeter.Web.Models;

namespace Sentimeter.Web.Api.Services;

public interface IVideoRetriever
{
    Task<DiscoveryVideoInformationResponseModel> DiscoveryVideoInformationAsync(DiscoveryVideoInformationModel model);
}

public class YouTubeVideoRetriever(YouTubeService youtubeService) : IVideoRetriever
{
    public async Task<DiscoveryVideoInformationResponseModel?> DiscoveryVideoInformationAsync(DiscoveryVideoInformationModel model)
    {
        var videoRequest = youtubeService.Videos.List("snippet");
        videoRequest.Id = model.VideoId;

        var videos = await videoRequest.ExecuteAsync();

        var videoItem = videos.Items.FirstOrDefault();
        if (videoItem == null)
        {
            return null;
        }

        var publishedAt = videoItem.Snippet.PublishedAtDateTimeOffset;

        return new DiscoveryVideoInformationResponseModel(
            videoItem.Snippet.Title,
            videoItem.Snippet.Description,
            publishedAt.HasValue ? null : publishedAt!.Value.DateTime);
    }
}
