using Akka.Actor;
using Akka.Event;
using Akka.Hosting;
using Microsoft.Extensions.Options;
using Sentimeter.DataRetrieval.Worker.Akka.Messages;
using Sentimeter.DataRetrieval.Worker.Configuration;
using Sentimeter.DataRetrieval.Worker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentimeter.DataRetrieval.Worker.Akka.Actors;

public class SchedulerActor : ReceiverBaseActor, IWithTimers
{
    private DataRetrivalSettings _settings;

    public ITimerScheduler Timers { get; set; }

    private bool _currentStatusEnabled;
    private IRequiredActor<ManagerWorkerActor> _managerWorkerActor;

    public SchedulerActor(IServiceScopeFactory serviceScopeFactory,
        IRequiredActor<ManagerWorkerActor> managerWorkerActor,
        IOptions<DataRetrivalSettings> settings,
        ILogger<SchedulerActor> logger) 
        : base(serviceScopeFactory, logger)
    {
        _settings = settings.Value;
        _currentStatusEnabled = _settings.Enabled;
        _managerWorkerActor = managerWorkerActor;

        Become(Working);

        Timers?.StartSingleTimer("SchedulerTimer", new ScheduleMessage(_settings.SchedulerWorkingItems), TimeSpan.Zero);
    }

    private void Working()
    {
        ReceiveAsync<ScheduleMessage>(ScheduleMessageHandlerAsync);
        Receive<ChangeServiceStatusMessage>(ChangeServiceStatusHandler);
    }

    private void ChangeServiceStatusHandler(ChangeServiceStatusMessage message)
    {
        ChangeAndForwardServiceStatus(message.Enabled);
    }
    private async Task ScheduleMessageHandlerAsync(ScheduleMessage message)
    {
        var scheduledItemsCount = 0;

        // Get services to work with DB (retrive videos, or videos and it's last commentId with lastUpdate?)
        using var scope = ServiceScopeFactory.CreateScope();
        var videoCommentsService = scope.ServiceProvider.GetRequiredService<IVideoAndCommentService>();


        if (!_currentStatusEnabled)
        {
            LogInformation("Service status is disabled, skip scheduler");
            return;
        }

        try
        {
            // Non avendo id sequenziali per i video ma GUI, devo fare una query in cui prendo i video ad intervalli tra una schedulazione e l'altra
            // List<VideoOperationInfo> operations = videoOperationService.GetVideoOperations(_settings.SchedulerWorkingItems);

            var newVideo = videoCommentsService.RetriveNewVideoWithoutComments(1, _settings.SchedulerWorkingItems);
            var existingVideo = videoCommentsService.RetriveVideosWithLastComment(1, _settings.SchedulerWorkingItems);

            var operations = newVideo.Concat(existingVideo).ToList();

            foreach (var operation in operations)
            {
                // Foreach operation, send message to ManagerWorkerActor
                _managerWorkerActor.ActorRef.Tell(operation, Self);
                scheduledItemsCount++;
            }
        }
        catch (Exception ex)
        {
            LogError(ex, "Error during scheduler execution");
        }
        finally
        {
            // If scheduledItemsCount >=  SchedulerWorkingItems, reschedule immediatly, otherwise wait SchedulerIntervalSeconds
            Timers?.StartSingleTimer("SchedulerTimer", 
                new ScheduleMessage(_settings.SchedulerWorkingItems), 
                (_currentStatusEnabled && scheduledItemsCount >= _settings.SchedulerWorkingItems) ? TimeSpan.Zero : TimeSpan.FromSeconds(_settings.SchedulerIntervalSeconds));
            LogInformation($"Scheduler executed, scheduled items: {scheduledItemsCount}");
        }



    }
    private void ChangeAndForwardServiceStatus(bool enabled)
    {
        if (enabled != _currentStatusEnabled)
        {
            LogInformation($"Service status has changed to: {enabled}");
            _currentStatusEnabled = enabled;
            _managerWorkerActor.ActorRef.Tell(new ChangeServiceStatusMessage(_currentStatusEnabled), Self);
        }
    }

}