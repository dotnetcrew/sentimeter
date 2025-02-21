using MassTransit;
using Sentimeter.DataRetrieval.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
});


builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(context.GetRequiredService<IConfiguration>().GetConnectionString("messaging"));

        cfg.ReceiveEndpoint("weather-forecast", e =>
        {
            e.ConfigureConsumer<VideoPublishedConsumer>(context);

        });


        cfg.ConfigureEndpoints(context);

    });
    x.AddConsumer<VideoPublishedConsumer>();

});

var host = builder.Build();
host.Run();
