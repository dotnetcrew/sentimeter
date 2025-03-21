using Google.Apis.YouTube.v3;
using Sentimeter.Web.Models;

namespace Sentimeter.Shared.Services;

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
        var thumbnailUrl = videoItem.Snippet.Thumbnails.Standard.Url;

        return new DiscoveryVideoInformationResponseModel(
            videoItem.Snippet.Title,
            videoItem.Snippet.Description,
            publishedAt.HasValue ? null : publishedAt!.Value.DateTime,
            thumbnailUrl);
    }

    public async Task<DiscoveryVideoCommentsResponseModel?> DiscoveryVideoCommmentsAsync(DiscoveryVideoCommentsModel model)
    {
        List<Comment> lstComments = new List<Comment>();
        var request = youtubeService.CommentThreads.List("snippet,replies");
        string? pageToken = null;

        request.Order = CommentThreadsResource.ListRequest.OrderEnum.Relevance;  // Or Time???
        request.VideoId = model.VideoIdentifier; 
        request.MaxResults = model.MaxResults;

        do
        {
            request.PageToken = pageToken;
            var response = await request.ExecuteAsync(); // Execute the request
            pageToken = response.NextPageToken;
                        
            foreach (var comment in response.Items)
            {
                var updatedAt = comment.Snippet.TopLevelComment.Snippet.UpdatedAtDateTimeOffset;
                var replies = comment.Replies;

                List<Comment> lstReplies = new List<Comment>();
                if (replies is not null and { Comments.Count: > 0 })
                {
                    foreach (var reply in replies.Comments)
                    {
                        lstReplies.Add(new Comment(reply.Id, reply.Snippet.AuthorDisplayName, reply.Snippet.TextDisplay, reply.Snippet.UpdatedAtDateTimeOffset, null));
                    }
                }

                lstComments.Add(new Comment(comment.Snippet.TopLevelComment.Id, comment.Snippet.TopLevelComment.Snippet.AuthorDisplayName, comment.Snippet.TopLevelComment.Snippet.TextDisplay, comment.Snippet.TopLevelComment.Snippet.UpdatedAtDateTimeOffset,lstReplies));
            }

        } while (pageToken is not null);

        return new DiscoveryVideoCommentsResponseModel(lstComments);
    }


}
