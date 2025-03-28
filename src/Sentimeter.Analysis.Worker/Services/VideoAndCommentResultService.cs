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
        Task<Guid> SaveVideoResult(VideoResultModel videoResult);
        Task<Guid> SaveCommentResult(CommentResultModel videoResult);
    }
    public class VideoAndCommentResultService : IVideoAndCommentResult
    {

        private SentimeterDbContext _context;

        public VideoAndCommentResultService(SentimeterDbContext ctx)
        {
            _context = ctx;
        }

        public async Task<Guid> SaveVideoResult(VideoResultModel videoResult)
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

        public async Task<Guid> SaveCommentResult(CommentResultModel videoResult)
        {

            var result = new CommentSentimentResult
            {
                CommentId  = videoResult.CommentId,
                Result = videoResult.Result,
                LastUpdate = videoResult.LastUpdate,
            };

            _context.CommentSentimentResult.Add(result);
            await _context.SaveChangesAsync();
            return result.Id;
        }

    }

    
}
