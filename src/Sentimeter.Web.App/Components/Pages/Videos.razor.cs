using Microsoft.AspNetCore.Components;
using Sentimeter.Web.App.Services;
using Sentimeter.Web.Models.Videos;

namespace Sentimeter.Web.App.Components.Pages;

public partial class Videos(VideosApiClient videosApiClient)
{
    [Parameter]
    public int Page { get; set; } = 1;

    private VideoListModel model = new();

    private bool loading = false;

    protected override async Task OnInitializedAsync()
    {
        if (Page <= 0)
        {
            Page = 1;
        }

        await LoadVideosAsync();
    }

    private async Task LoadVideosAsync()
    {
        loading = true;

        try
        {
            model = await videosApiClient.GetVideosAsync(Page, 12);
        }
        finally
        {
            loading = false;
        }
    }
}
