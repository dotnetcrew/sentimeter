using Sentimeter.Core;
using Sentimeter.Shared;
using Sentimeter.Support.Migration.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<SentimeterDbContext>(connectionName: ServiceNames.Db);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
