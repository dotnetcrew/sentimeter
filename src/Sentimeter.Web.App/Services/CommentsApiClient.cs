using Sentimeter.Web.Models.Comments;

namespace Sentimeter.Web.App.Services;

public class CommentsApiClient(HttpClient httpClient)
{
    public async Task<CommentListModel> GetCommentsAsync(Guid videoId, int page, int size)
    {
        var model = await httpClient.GetFromJsonAsync<CommentListModel>($"api/videos/{videoId}/comments?page={page}&size={size}");
        return model ?? new();
    }

    public async Task<CommentSentimentStatsModel[]> GetCommentSentimentStatsAsync(Guid videoId)
    {
        var model = await httpClient.GetFromJsonAsync<CommentSentimentStatsModel[]>($"api/videos/{videoId}/comments/sentiment/stats");
        return model ?? [];
    }
}
