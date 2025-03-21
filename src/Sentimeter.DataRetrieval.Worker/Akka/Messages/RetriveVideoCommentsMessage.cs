using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentimeter.DataRetrieval.Worker.Akka.Messages;

public class RetriveVideoCommentsMessage
{
    
    public Guid VideoId { get; }

    public string Identifier { get; }

    public Guid? CommentId { get; }
    public DateTimeOffset? CommentDate { get; }


    public RetriveVideoCommentsMessage(string identifier, Guid videoId, Guid? commentId, DateTimeOffset? commentDate)
    {
        Identifier = identifier;
        VideoId = videoId;
        CommentId = commentId;
        CommentDate = commentDate;
    }

    public override string ToString()
    {
        return $"[VideoId: {VideoId}]";
    }

}
