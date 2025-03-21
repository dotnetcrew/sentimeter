using Akka.Streams.Implementation.Fusing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Sentimeter.Core;
using Sentimeter.Core.Models;
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
    string? GetVideoIdentifierFromId(Guid videoId);
    Guid? GetVideoIdFromVideoIdentifier(string identifier);
    Guid UpdateOrSaveVideoComment(Guid videoId, string currentCommentIdentifier, Guid? replyCommentIdentifier, string authorDisplayName, string textDisplay, DateTimeOffset? updatedAtDateTimeOffset);
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

        return lastComments.Select(c => new RetriveVideoCommentsMessage(c.Video.Identifier, c.Video.Id, c.Id, c.LastUpdate)).ToList();
    }

    public List<RetriveVideoCommentsMessage> RetriveNewVideoWithoutComments(int skipItemNum, int takeItemNum)
    {
        var resVideoWithoutComments = _context.Videos
            .Include(v => v.Comments)
            .Where(c => c.Comments.Count == 0);

        return resVideoWithoutComments.Select(c => new RetriveVideoCommentsMessage(c.Identifier, c.Id, null, null)).ToList();
    }

    public string? GetVideoIdentifierFromId(Guid videoId)
    {
        var res = _context.Videos.FirstOrDefault(v => v.Id == videoId);

        return res == null ? null : res.Identifier;

    }

    public Guid? GetVideoIdFromVideoIdentifier(string identifier)
    {
        var res = _context.Videos.FirstOrDefault(v => v.Identifier == identifier);

        return res == null ? null : res.Id;
    }

    public Guid UpdateOrSaveVideoComment(Guid videoId, string currentCommentIdentifier, Guid? replyCommentIdentifier, string authorDisplayName, string textDisplay, DateTimeOffset? updatedAtDateTimeOffset)
    {
        Guid idComment = Guid.Empty;
        try
        {

           var res = _context.Comments.Where( c => c.VideoId == videoId).FirstOrDefault(c => c.Identifier == currentCommentIdentifier);

            if (res == null)
            {
                var c = new Comment
                {
                    Author = authorDisplayName,
                    Content = textDisplay,
                    Identifier = currentCommentIdentifier,
                    LastUpdate = updatedAtDateTimeOffset!=null?updatedAtDateTimeOffset.Value.UtcDateTime:null ,
                    CommentId = replyCommentIdentifier,
                    VideoId = videoId
                };
                _context.Comments.Add(c);
                _context.SaveChanges();
                idComment = c.Id;
            }
            else
            {
                // Check if the comment has been updated
                if (updatedAtDateTimeOffset != null &&
                    res.LastUpdate < updatedAtDateTimeOffset )
                {
                    res.Content = textDisplay;
                    res.LastUpdate = updatedAtDateTimeOffset != null ? updatedAtDateTimeOffset.Value.UtcDateTime : null;
                    _context.SaveChanges();
                    idComment = res.Id;
                }
            }

        }
        catch (Exception ex)
        {
            
            Console.WriteLine(ex.Message);

        }

        return idComment;
    }
}
