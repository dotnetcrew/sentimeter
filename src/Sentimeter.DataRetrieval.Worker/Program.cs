using MassTransit;
using Sentimeter.DataRetrieval.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

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
