using Microsoft.AspNetCore.Components;
using Sentimeter.Web.App.Services;
using Sentimeter.Web.Models.Videos;

namespace Sentimeter.Web.App.Components.Pages;

public partial class VideoDetail(VideosApiClient videosApiClient)
{
    [Parameter]
    public Guid Id { get; set; }

    private VideoDetailModel? model;

    private bool loading = false;

    protected override async Task OnInitializedAsync()
    {
        loading = true;

        try
        {
            model = await videosApiClient.GetVideoDetailAsync(Id);
        }
        finally
        {
            loading = false;
        }
    }
}
