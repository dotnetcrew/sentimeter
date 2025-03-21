using Microsoft.AspNetCore.Components;
using Sentimeter.Web.App.Services;
using Sentimeter.Web.Models.Videos;

namespace Sentimeter.Web.App.Components.Pages;

public partial class RegisterVideo(VideosApiClient videosApiClient, NavigationManager navigationManager)
{
    private RegisterVideoModel model = new();

    private bool loadingVideoInformation = false;
    private string? loadingVideoInformationErrorMessage;

    private async Task DiscoveryVideoInformationAsync()
    {
        loadingVideoInformation = true;
        loadingVideoInformationErrorMessage = null;

        try
        {
            var videoInformation = await videosApiClient.DiscoveryVideoInformationAsync(new(model.VideoId));

            model.Title = videoInformation.Title;
            model.Description = videoInformation.Description;
            model.PublishedAt = videoInformation.PublishedAt;
            model.ThumbnailUrl = videoInformation.ThumbnailUrl;
        }
        catch (InvalidOperationException ex)
        {
            loadingVideoInformationErrorMessage = ex.Message;
        }
        finally
        {
            loadingVideoInformation = false;
        }
    }

    private async Task RegisterVideoAsync()
    {
        await videosApiClient.RegisterVideoAsync(model);
        navigationManager.NavigateTo("/videos", forceLoad: true);
    }
}
