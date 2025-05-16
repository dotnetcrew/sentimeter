using Sentimeter.Web.App.Services;
using Sentimeter.Web.Models.Comments;
using Sentimeter.Web.Models.Videos;

namespace Sentimeter.Web.App.Components.UI;

public partial class DashboardWidgets(VideosApiClient videosApiClient, CommentsApiClient commentsApiClient)
{
    private VideoStatsModel videoStats = new(0, 0);
    private CommentStatsModel commentStats = new(0, 0);

    private bool loadingStats = false;

    protected override async Task OnInitializedAsync()
    {
        loadingStats = true;

        try
        {
            videoStats = await videosApiClient.GetVideoStatsAsync();
            commentStats = await commentsApiClient.GetCommentStatsAsync();
        }
        finally
        {
            loadingStats = false;
        }
    }
}
