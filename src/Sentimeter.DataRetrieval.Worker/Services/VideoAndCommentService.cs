using Akka.Streams.Implementation.Fusing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Sentimeter.Core;
using Sentimeter.DataRetrieval.Worker.Akka.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentimeter.DataRetrieval.Worker.Services;

public interface IVideoAndCommentService
{
    List<RetriveVideoCommentsMessage> RetriveVideosWithLastComment(int skipItemNum, int takeItemNum);
    List<RetriveVideoCommentsMessage> RetriveNewVideoWithoutComments(int skipItemNum, int takeItemNum);
}


public class VideoAndCommentService : IVideoAndCommentService
{
    private SentimeterDbContext _context;

    public VideoAndCommentService(SentimeterDbContext ctx)
    {
        _context = ctx;
    }


    public List<RetriveVideoCommentsMessage> RetriveVideosWithLastComment(int skipItemNum, int takeItemNum)
    {
        var lastComments = _context.Comments
            .Include(c => c.Video)
            .OrderByDescending( v => v.LastUpdate)
            .GroupBy(c => c.Video.Id)
            .Select(g => g.OrderByDescending(c => c.LastUpdate).FirstOrDefault())
            //.Where(c => c != null) // Potrei avere nuovi video ancora senza commenti...
            //.Skip(skipItemNum) // ToDo: implementare per prendere ogni volta un blocco di video diverso
            .Take(takeItemNum)
            .ToList();

        return lastComments.Select(c => new RetriveVideoCommentsMessage(c.Video.Id, c.Id, c.LastUpdate)).ToList();
    }

    public List<RetriveVideoCommentsMessage> RetriveNewVideoWithoutComments(int skipItemNum, int takeItemNum)
    {
        var resVideoWithoutComments = _context.Videos
            .Include(v => v.Comments)
            .Where(c => c.Comments.Count == 0);

        return resVideoWithoutComments.Select(c => new RetriveVideoCommentsMessage(c.Id, null, null)).ToList();
    }



}
