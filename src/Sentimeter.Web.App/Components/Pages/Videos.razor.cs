using Sentimeter.Web.App.Services;
using Sentimeter.Web.Models;

namespace Sentimeter.Web.App.Components.Pages;

public partial class Videos(VideosApiClient videosApiClient)
{
    private VideoListModel model = new();

    private bool loading = false;

    protected override async Task OnInitializedAsync()
    {
        loading = true;

        try
        {
            model = await videosApiClient.GetVideosAsync(1, 12);
        }
        finally
        {
            loading = false;
        }
    }
}
