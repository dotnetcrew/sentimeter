using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sentimeter.Shared.Services;
using Sentimeter.Web.Models;

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

        return builder;
    }

    private static async Task<Results<Ok<DiscoveryVideoInformationResponseModel>, BadRequest<string>>> DiscoveryVideoInformation(
        IVideoRetriever videoRetriever,
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
