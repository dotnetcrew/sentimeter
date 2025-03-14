using Akka.Actor;
using Sentimeter.DataRetrieval.Worker.Akka.Messages;
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
        ILogger<DataRetrivalWorkerRouter> logger) : base(serviceScopeFactory, logger)
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

        // ToDo: Retrive comments using message.VideoId and check lastupdate if commnets !=null

    }

}
