using Microsoft.EntityFrameworkCore;
using Sentimeter.Analysis.Worker.Models;
using Sentimeter.Core;
using Sentimeter.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentimeter.Analysis.Worker.Services
{

    public interface IVideoAndCommentResult
    {
        Task<Guid> SaveVideoResultAsync(VideoResultModel videoResult);
        Task<Guid> SaveCommentResultAsync(CommentResultModel videoResult);

        public List<Comment> GetAllCommentsByVideoId(Guid videoId);
        public List<Comment> GetAllCommentsWithoutResultByVideoId(Guid videoId);

        public List<Video> GetAllVideos();
    }
    public class VideoAndCommentResultService : IVideoAndCommentResult
    {

        private SentimeterDbContext _context;

        public VideoAndCommentResultService(SentimeterDbContext ctx)
        {
            _context = ctx;
        }

        public async Task<Guid> SaveVideoResultAsync(VideoResultModel videoResult)
        {

            var result = new VideoSentimentResult
            {
                VideoId = videoResult.VideoId,
                Result = videoResult.Result,
                LastUpdate = videoResult.LastUpdate,
            };
            
            _context.VideoSentimentResult.Add(result);
            await _context.SaveChangesAsync();
            return result.Id;
        }

        public async Task<Guid> SaveCommentResultAsync(CommentResultModel videoResult)
        {

            var result = new CommentSentimentResult
            {
                CommentId  = videoResult.CommentId,
                Result = videoResult.Result,
                LastUpdate = videoResult.LastUpdate,
                Score = videoResult.Score
            };

            _context.CommentSentimentResult.Add(result);
            await _context.SaveChangesAsync();
            return result.Id;
        }

        public List<Video> GetAllVideos()
        {
            return _context.Videos.AsNoTracking().OrderBy(x => x.Id).ToList();
        }


        public List<Comment> GetAllCommentsByVideoId(Guid videoId)
        {
            return _context.Comments.Where(c => c.VideoId == videoId).OrderBy(x => x.Id).ToList();
        }

        public List<Comment> GetAllCommentsWithoutResultByVideoId(Guid videoId)
        {
            return _context.Comments
                .Include(c => c.SentimentResult)
                .Where(c => c.VideoId == videoId && c.SentimentResult == null )
                .OrderBy(x => x.Id).ToList();

        }

    }

    


    }
