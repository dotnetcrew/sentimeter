using Microsoft.AspNetCore.Http.HttpResults;
using Sentimeter.Web.Api.Services;
using Sentimeter.Web.Models.Comments;

namespace Sentimeter.Web.Api.Endpoints;

internal static class CommentEndpoints
{
    public static IEndpointRouteBuilder MapCommentEndpoints(this IEndpointRouteBuilder builder)
    {
        var commentGroup = builder.MapGroup("/api/videos/{videoId:guid}/comments")
            .RequireAuthorization();

        commentGroup.MapGet("", GetComments)
            .WithOpenApi()
            .WithName(nameof(GetComments));

        return builder;
    }

    private static async Task<Ok<CommentListModel>> GetComments(
        ICommentEndpointsService service,
        Guid videoId,
        int page = 1,
        int size = 20)
    {
        var comments = await service.GetCommentsAsync(videoId, page, size);
        return TypedResults.Ok(comments);
    }
}
