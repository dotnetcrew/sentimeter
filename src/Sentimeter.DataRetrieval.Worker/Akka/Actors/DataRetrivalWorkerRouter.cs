using Akka.Actor;
using Akka.Streams.Dsl;
using Sentimeter.DataRetrieval.Worker.Akka.Messages;
using Sentimeter.DataRetrieval.Worker.Services;
using Sentimeter.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentimeter.DataRetrieval.Worker.Akka.Actors;

internal class DataRetrivalWorkerRouter : ReceiverBaseActor, IWithTimers, IWithUnboundedStash
{
    public ITimerScheduler Timers { get; set; }
    public IStash Stash { get; set; }

    public DataRetrivalWorkerRouter(IServiceScopeFactory serviceScopeFactory, 
        ILogger<DataRetrivalWorkerRouter> logger) 
        : base(serviceScopeFactory, logger)
    {
        Become(WorkingEnabled);
    }

    private void WorkingEnabled()
    {
        LogInformation("Actor working enabled");
        ReceiveAsync<RetriveVideoCommentsMessage>(WorkingOperationMessageHandler);
        Receive<ChangeServiceStatusMessage>(ChangeServiceStatusMessageHandler);
    }

    private void WorkingDisabled()
    {
        LogWarning("!!! Actor working disabled !!!");
        Receive<RetriveVideoCommentsMessage>(WorkOperationStashHandler);
        Receive<ChangeServiceStatusMessage>(ChangeServiceStatusMessageHandler);
    }

    private void ChangeServiceStatusMessageHandler(ChangeServiceStatusMessage message)
    {
        LogInformation($"Change service status message received: {message}");

        if (message.Enabled)
        {
            LogDebug("Unstashing all messages ...");
            Stash.UnstashAll();
            LogDebug("All messages unstashed");
            Become(WorkingEnabled);
        }
        else
        {
            Become(WorkingDisabled);
        }
    }

    private void WorkOperationStashHandler(RetriveVideoCommentsMessage message)
    {
        LogInformation("Service disabled -> message stashed: {Message}", message.ToString());
        Stash.Stash();
    }

    private async Task WorkingOperationMessageHandler(RetriveVideoCommentsMessage message)
    {
        LogInformation($"WorkingOperationMessageHandler: VideoId={message.VideoId}" );
        LogInformation( message.CommentId != null ? $"CommentId={message.CommentId}, CommentDate={message.CommentDate}" : "No CommentId");
        // Get services to work with DB (retrive videos, or videos and it's last commentId with lastUpdate?)
        using var scope = ServiceScopeFactory.CreateScope();
        var videoCommentsService = scope.ServiceProvider.GetRequiredService<IVideoAndCommentService>();
        var videoRetriever = scope.ServiceProvider.GetRequiredService<IVideoRetriever>();

        try
        {
            // Retrive comments using message.VideoId and check lastupdate if comments !=null
            var result = await videoRetriever.DiscoveryVideoCommmentsAsync(new Web.Models.Videos.DiscoveryVideoCommentsModel(message.Identifier, 100, message.CommentId.ToString(), message.CommentDate));

            if (result != null && 
                result.comments.Count>0)
            {
                foreach(var comment in result.comments)
                {
                    Guid newCommentId= videoCommentsService.UpdateOrSaveVideoComment(message.VideoId, comment.CommentIdentifier, null, comment.AuthorDisplayName, comment.TextDisplay, comment.UpdatedAtDateTimeOffset);
                    
                    foreach(var reply in comment.replies)
                    {
                        videoCommentsService.UpdateOrSaveVideoComment(message.VideoId, reply.CommentIdentifier, newCommentId, reply.AuthorDisplayName, reply.TextDisplay, reply.UpdatedAtDateTimeOffset);
                    }
                }                
            }
        }
        catch (Exception ex)
        {
            LogError($"Exception into WorkingOperationMessageHandler: {ex.Message} ");
        }

        
    }

}
