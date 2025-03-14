using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sentimeter.Shared.Services;
using Sentimeter.Web.Api.Security;
using Sentimeter.Web.Api.Services;
using Sentimeter.Web.Models;
using System.Security.Claims;

namespace Sentimeter.Web.Api.Endpoints;

internal static class VideoEndpoints
{
    public static IEndpointRouteBuilder MapVideoEndpoints(this IEndpointRouteBuilder builder)
    {
        var videoGroup = builder.MapGroup("/api/videos")
            .RequireAuthorization();

        videoGroup.MapPost("discovery", DiscoveryVideoInformation)
            .WithOpenApi()
            .WithName(nameof(DiscoveryVideoInformation));

        videoGroup.MapGet("", GetVideos)
            .WithOpenApi()
            .WithName(nameof(GetVideos));

        videoGroup.MapGet("{id:guid}", GetVideoDetail)
            .WithOpenApi()
            .WithName(nameof(GetVideoDetail));

        videoGroup.MapPost("", RegisterVideo)
            .WithOpenApi()
            .WithName(nameof(RegisterVideo));

        return builder;
    }

    private static async Task<Ok<VideoListModel>> GetVideos(
        IVideoEndpointsService service,
        ClaimsPrincipal user,
        int page = 1,
        int size = 10)
    {
        var model = await service.GetVideosAsync(page, size, user.GetUserId());
        return TypedResults.Ok(model);
    }

    private static async Task<Results<Ok<object>, NotFound>> GetVideoDetail(
        IVideoEndpointsService service,
        ClaimsPrincipal user,
        Guid id)
    {
        throw new NotImplementedException();
    }

    private static async Task<Results<CreatedAtRoute<RegisterVideoModel>, BadRequest>> RegisterVideo(
        IVideoEndpointsService service,
        ClaimsPrincipal user,
        [FromBody] RegisterVideoModel model)
    {
        var videoId = await service.RegisterVideoAsync(model, user.GetUserId());
        return TypedResults.CreatedAtRoute(model, nameof(GetVideoDetail), new { id = videoId });
    }

    private static async Task<Results<Ok<DiscoveryVideoInformationResponseModel>, BadRequest<string>>> DiscoveryVideoInformation(
        IVideoRetriever videoRetriever,
        ClaimsPrincipal user,
        [FromBody] DiscoveryVideoInformationModel model)
    {
        if (string.IsNullOrWhiteSpace(model.VideoId))
        {
            return TypedResults.BadRequest("VideoId is required.");
        }

        var videoInformation = await videoRetriever.DiscoveryVideoInformationAsync(model);
        if (videoInformation == null)
        {
            return TypedResults.BadRequest("Video not found.");
        }

        return TypedResults.Ok(videoInformation);
    }
}
