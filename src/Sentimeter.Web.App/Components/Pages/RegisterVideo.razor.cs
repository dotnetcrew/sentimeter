using Microsoft.AspNetCore.Components;
using Sentimeter.Web.App.Services;
using Sentimeter.Web.Models.Videos;

namespace Sentimeter.Web.App.Components.Pages;

public partial class RegisterVideo(VideosApiClient videosApiClient, NavigationManager navigationManager)
{
    private RegisterVideoModel model = new();

    private bool loadingVideoInformation = false;
    private string? loadingVideoInformationErrorMessage;

    private bool registeringVideo = false;
    private string? registeringVideoErrorMessage;

    private async Task DiscoveryVideoInformationAsync()
    {
        loadingVideoInformation = true;
        loadingVideoInformationErrorMessage = null;

        try
        {
            var videoInformation = await videosApiClient.DiscoveryVideoInformationAsync(new(model.VideoId));
            if (!videoInformation.Success)
            {
                loadingVideoInformationErrorMessage = videoInformation.ErrorMessage;
            }

            model.Title = videoInformation.Content!.Title;
            model.Description = videoInformation.Content!.Description;
            model.PublishedAt = videoInformation.Content!.PublishedAt;
            model.ThumbnailUrl = videoInformation.Content!.ThumbnailUrl;
        }
        finally
        {
            loadingVideoInformation = false;
        }
    }

    private async Task RegisterVideoAsync()
    {
        registeringVideoErrorMessage = null;
        registeringVideo = true;

        try
        {
            var result = await videosApiClient.RegisterVideoAsync(model);

            if (result.Success)
            {
                navigationManager.NavigateTo("/videos", forceLoad: true);
                return;
            }

            registeringVideoErrorMessage = result.ErrorMessage;
        }
        finally
        {
            registeringVideo = false;
        }
    }
}
