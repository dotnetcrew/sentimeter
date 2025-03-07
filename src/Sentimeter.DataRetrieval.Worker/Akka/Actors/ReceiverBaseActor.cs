using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Routing;

namespace Sentimeter.DataRetrieval.Worker.Akka.Actors;


public class ReceiverBaseActor : ReceiveActor
{
    protected readonly IServiceScopeFactory ServiceScopeFactory;
    private readonly ILogger _logger;

    public ReceiverBaseActor(IServiceScopeFactory serviceScopeFactory, ILogger logger)
    {
        ServiceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected IActorRef GetChild<T>(string childName) where T : ActorBase
    {
        IActorRef child = Context.Child(childName);
        if (child.IsNobody())
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("{TypeName} create child actor {ChildName}", GetType().Name, childName);
            }

            var props = DependencyResolver.For(Context.System).Props<T>();

            child = Context.ActorOf(props, childName);
        }

        return child;
    }

    protected IActorRef GetChildWithSmallestMailboxPool<T>(string childName, int numberOfInstances) where T : ActorBase
    {
        if (numberOfInstances <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfInstances), "Value must be greater than 0");
        if (numberOfInstances > 300) throw new ArgumentOutOfRangeException(nameof(numberOfInstances), "Value must be less than 300");

        var child = Context.Child(childName);
        if (child.IsNobody())
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("{TypeName} create child actor {ChildName} with Router SmallestMailboxPool instances: {NumberOfInstances}", GetType().Name, childName, numberOfInstances);
            }

            var props = DependencyResolver
                .For(Context.System)
                .Props<T>()
                .WithRouter(new SmallestMailboxPool(numberOfInstances));

            child = Context.ActorOf(props, childName);
        }

        return child;
    }

    protected override void PreStart()
    {
        base.PreStart();

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            using (_logger.BeginScope("{TypeName}{ActorPath}", GetType().Name, Self.Path))
            {
                _logger.LogDebug("PRE-START {SelfPath}", this.Self.Path);
            }
        }
    }

    protected override void PostStop()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            // Lo devo ricreare perché post stop è eseguito dopo e ho perso lo scope del logger
            using (_logger.BeginScope("{TypeName}{ActorPath}", GetType().Name, Self.Path))
            {
                _logger.LogDebug("POST-STOP");
            }
        }

        base.PostStop();
    }

    protected void LogInformation(string message, params object[] args)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            using (_logger.BeginScope("{TypeName}{ActorPath}", GetType().Name, Self.Path))
            {
                _logger.LogInformation(message, args);
            }
        }
    }

    protected void LogDebug(string message, params object[] args)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            using (_logger.BeginScope("{TypeName}{ActorPath}", GetType().Name, Self.Path))
            {
                _logger.LogDebug(message, args);
            }
        }
    }

    protected void LogWarning(string message, params object[] args)
    {
        using (_logger.BeginScope("{TypeName}{ActorPath}", GetType().Name, Self.Path))
        {
            _logger.LogWarning(message, args);
        }
    }

    protected void LogWarning(Exception ex, string message, params object[] args)
    {
        using (_logger.BeginScope("{TypeName}{ActorPath}", GetType().Name, Self.Path))
        {
            _logger.LogWarning(ex, message, args);
        }
    }

    protected void LogError(string message, params object[] args)
    {
        using (_logger.BeginScope("{TypeName}{ActorPath}", GetType().Name, Self.Path))
        {
            _logger.LogError(message, args);
        }
    }

    protected void LogError(Exception ex, string message, params object[] args)
    {
        using (_logger.BeginScope("{TypeName}{ActorPath}", GetType().Name, Self.Path))
        {
            _logger.LogError(ex, message, args);
        }
    }
}
