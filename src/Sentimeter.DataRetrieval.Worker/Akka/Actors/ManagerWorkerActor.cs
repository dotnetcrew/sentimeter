using Akka.Actor;
using Akka.Routing;
using Microsoft.Extensions.Options;
using Sentimeter.DataRetrieval.Worker.Akka.Messages;
using Sentimeter.DataRetrieval.Worker.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentimeter.DataRetrieval.Worker.Akka.Actors;

public class ManagerWorkerActor : ReceiverBaseActor
{
    private DataRetrivalSettings _settings;

    public ManagerWorkerActor(IServiceScopeFactory serviceScopeFactory,
        IOptions<DataRetrivalSettings> settings,
        ILogger<ManagerWorkerActor> logger) : base(serviceScopeFactory, logger)
    {
        _settings = settings.Value;
        Become(Working);
    }


    private void Working()
    {
        Receive<RetriveCommentsMessage>(WorkingOperationMessageHandler);
        Receive<ChangeServiceStatusMessage>(ChangeServiceStatusMessageHandler);
    }

    private void ChangeServiceStatusMessageHandler(ChangeServiceStatusMessage message)
    {
        LogInformation($"Change service status message received: {message}");
        var workerActorForNewVideo = GetChildWithSmallestMailboxPool<DataRetrivalWorkerRouter>("DataRetrivalForNewVideoWorkerRouter", _settings.WorkersCount);
        var workerActorForExistingVideo = GetChildWithSmallestMailboxPool<DataRetrivalWorkerRouter>("DataRetrivalForExistingVideoWorkerRouter", _settings.WorkersCount);

        LogDebug($"Broadcasting change service status message received to worker actors ...");
        workerActorForNewVideo.Forward(new Broadcast(message));
        LogDebug($"Broadcast change service status, Done");
    }

    private void WorkingOperationMessageHandler(RetriveCommentsMessage message)
    {
        IActorRef workerActor;

        if (message.CommentId == null)
        {
            workerActor = GetChildWithSmallestMailboxPool<DataRetrivalWorkerRouter>("DataRetrivalForNewVideoWorkerRouter", _settings.WorkersCount);
        }
        else
        {
            workerActor = GetChildWithSmallestMailboxPool<DataRetrivalWorkerRouter>("DataRetrivalForExistingVideoWorkerRouter", _settings.WorkersCount);
        }
        workerActor.Forward(message);
    }

}
