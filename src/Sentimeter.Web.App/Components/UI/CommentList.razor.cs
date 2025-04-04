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

    private int currentPage = 1;

    private int size = 10;

    protected override async Task OnInitializedAsync()
    {
        await LoadCommentsAsync();
    }

    private async Task LoadCommentsAsync()
    {
        loading = true;

        try
        {
            model = await commentsApiClient.GetCommentsAsync(VideoId, currentPage, size);
        }
        finally
        {
            loading = false;
        }
    }

    private async Task MoveBack()
    {
        currentPage--;
        await LoadCommentsAsync();
    }

    private async Task MoveNext()
    {
        currentPage++;
        await LoadCommentsAsync();
    }
}
