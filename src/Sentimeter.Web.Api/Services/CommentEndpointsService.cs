using Microsoft.EntityFrameworkCore;
using Sentimeter.Core;
using Sentimeter.Web.Models.Comments;

namespace Sentimeter.Web.Api.Services;

public class CommentEndpointsService : ICommentEndpointsService
{
    private readonly SentimeterDbContext _context;

    public CommentEndpointsService(SentimeterDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<CommentListModel> GetCommentsAsync(Guid videoId, int page, int size)
    {
        var skip = (page - 1) * size;

        var commentsQuery = _context.Comments.AsNoTracking()
            .Include(c => c.SentimentResult)
            .Where(x => x.VideoId == videoId);

        var totalNumberOfComments = await commentsQuery.CountAsync();

        var numberOfPages = (int)Math.Ceiling((double)totalNumberOfComments / size);

        var comments = await commentsQuery
            .Skip(skip)
            .Take(size)
            .Select(c => new CommentListModel.CommentItemDescriptor(
                c.Id,
                c.Author,
                c.Content,
                c.LastUpdate,
                c.SentimentResult == null ? null : new(c.SentimentResult.Result, c.SentimentResult.Score))).ToListAsync();

        return new CommentListModel
        {
            Items = comments,
            NumberOfPages = numberOfPages,
            HasNextPage = page < numberOfPages,
            IsFirstPage = page == 1
        };
    }

    public async Task<CommentSentimentStatsModel[]?> GetCommentSentimentStatsAsync(Guid videoId)
    {
        var commentsStats = await _context.Comments.AsNoTracking()
            .Include(c => c.SentimentResult)
            .Where(c => c.VideoId == videoId)
            .Select(c => new CommentSentimentStatsModel(
                c.SentimentResult == null ? "N/A" : c.SentimentResult.Result, 
                c.SentimentResult == null ? 0.00 : c.SentimentResult.Score)).ToArrayAsync();

        return commentsStats;
    }

    public async Task<CommentStatsModel> GetCommentStatsAsync()
    {
        var commentsQuery = _context.Comments.AsNoTracking()
            .Include(c => c.SentimentResult);

        var totalNumberOfComments = await commentsQuery.CountAsync();
        var numberOfCommentsAnalyzed = await commentsQuery.CountAsync(c => c.SentimentResult != null);

        return new(totalNumberOfComments, numberOfCommentsAnalyzed);
    }
}
