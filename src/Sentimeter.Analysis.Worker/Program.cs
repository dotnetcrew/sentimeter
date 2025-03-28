using Sentimeter.Analysis.Worker;
using Sentimeter.Analysis.Worker.Services;
using Sentimeter.Core;
using Sentimeter.Shared;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<SentimeterDbContext>(connectionName: ServiceNames.Db);

builder.Services.AddScoped<IVideoAndCommentResult, VideoAndCommentResultService>();

builder.AddOllamaSharpChatClient("sentimeter-llama3");

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
