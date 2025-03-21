using Microsoft.AspNetCore.Components;
using Sentimeter.Web.App.Services;
using Sentimeter.Web.Models.Comments;

namespace Sentimeter.Web.App.Components.UI;

public partial class CommentList(CommentsApiClient commentsApiClient)
{
    [Parameter]
    public Guid VideoId { get; set; }

    private CommentListModel model = new();

    private bool loading = false;

    protected override async Task OnInitializedAsync()
    {
        loading = true;

        try
        {
            model = await commentsApiClient.GetCommentsAsync(VideoId, 1, 10);
        }
        finally
        {
            loading = false;
        }
    }
}
