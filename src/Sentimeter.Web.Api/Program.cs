using MassTransit;
using Sentimeter.Shared.Messages;
using Sentimeter.Web.Api.Endpoints;
using Sentimeter.Shared.Services;
using Sentimeter.Web.Api.Services;
using Sentimeter.Core;
using Sentimeter.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<SentimeterDbContext>(connectionName: ServiceNames.Db);

builder.AddMassTransitRabbitMq(
    "messaging",
    massTransitConfiguration: _ =>
    {
        EndpointConvention.Map<VideoPublishedMessage>(new Uri("queue:video-published"));
    });

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer(
        serviceName: "sentimeter-identity",
        realm: "Sentimeter",
        configureOptions: options =>
        {
            options.RequireHttpsMetadata = false;
            options.Audience = "sentimeter.api";
        });

builder.Services.AddAuthorizationBuilder();

builder.Services.AddYouTubeVideoRetriever(builder.Configuration["YOUTUBE_APIKEY"]!);

builder.Services.AddScoped<IVideoEndpointsService, VideoEndpointsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/test", async (IBus bus)  =>
{
    //var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:weather-forecast"));
    //await endpoint.Send(new VideoPublishedMessage(1));

    await bus.Send(new VideoPublishedMessage("B1IiYhBgEXU"));

    return Results.Accepted();
})
.WithName("GetWeatherForecast");

app.MapVideoEndpoints();

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

