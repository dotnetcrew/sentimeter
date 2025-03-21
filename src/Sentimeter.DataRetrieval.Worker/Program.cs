using Akka.Hosting;
using Sentimeter.Core;
using Sentimeter.DataRetrieval.Worker;
using Sentimeter.DataRetrieval.Worker.Akka.Actors;
using Sentimeter.DataRetrieval.Worker.Configuration;
using Sentimeter.DataRetrieval.Worker.Services;
using Sentimeter.Shared;
using Sentimeter.Shared.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<SentimeterDbContext>(connectionName: ServiceNames.Db);

builder.AddMassTransitRabbitMq(
    "messaging",
    options => options.DisableTelemetry = false,
    configuration =>
    {
        configuration.AddConsumer<VideoPublishedConsumer>();
    });

builder.Services.Configure<DataRetrivalSettings>(builder.Configuration.GetSection(DataRetrivalSettings.JsonSection));

builder.Services.AddScoped<IVideoAndCommentService, VideoAndCommentService>();

builder.Services.AddAkka("SentimeterActorSystem", (configurationBuilder, builder) =>
{
    configurationBuilder.ConfigureLoggers(configBuilder => { configBuilder.AddLoggerFactory(builder.GetRequiredService<ILoggerFactory>()); });
    configurationBuilder.WithActors((system, registry, resolver) =>
    {
        var propsManagerWorkerActor = resolver.Props<ManagerWorkerActor>();
        var ManagerWorkerActor = system.ActorOf(propsManagerWorkerActor, "DataRetrival_ManagerWorkerActor");
        registry.Register<ManagerWorkerActor>(ManagerWorkerActor);

        var propsOcrSchedulerActor = resolver.Props<SchedulerActor>();
        var schedulerActor = system.ActorOf(propsOcrSchedulerActor, "DataRetrival_SchedulerActor");
        registry.Register<SchedulerActor>(schedulerActor);

    });
});

// Read the YoutubeApiKey from user secrets
var youtubeApiKey = builder.Configuration["YoutubeApiKey"];
builder.Services.AddYouTubeVideoRetriever(youtubeApiKey!);

var host = builder.Build();
host.Run();
