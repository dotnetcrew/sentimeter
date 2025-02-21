using Sentimeter.Analysis.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddOllamaSharpChatClient("sentimeter-llama3");

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
