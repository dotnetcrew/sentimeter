using Sentimeter.Core;
using Sentimeter.DataRetrieval.Worker;
using Sentimeter.Shared;

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

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
