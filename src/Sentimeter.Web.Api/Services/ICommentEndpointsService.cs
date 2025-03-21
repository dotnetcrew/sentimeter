using Sentimeter.Web.Models.Comments;

namespace Sentimeter.Web.Api.Services;

public interface ICommentEndpointsService
{
    Task<CommentListModel> GetCommentsAsync(Guid videoId, int page, int size);
}
