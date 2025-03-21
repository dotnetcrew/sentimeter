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
        EndpointConvention.Map<SynchronizeVideoMessage>(new Uri("queue:synchronize-video"));
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

builder.Services
    .AddScoped<IVideoEndpointsService, VideoEndpointsService>()
    .AddScoped<ICommentEndpointsService, CommentEndpointsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app
    .MapVideoEndpoints()
    .MapCommentEndpoints();

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

