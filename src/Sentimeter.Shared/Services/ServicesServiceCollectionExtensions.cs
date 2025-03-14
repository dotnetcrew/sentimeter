using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.DependencyInjection;

namespace Sentimeter.Shared.Services;

public static class ServicesServiceCollectionExtensions
{
    public static IServiceCollection AddYouTubeVideoRetriever(this IServiceCollection services, string youtubeApiKey)
    {
        services.AddSingleton<YouTubeService>(sp => new(new BaseClientService.Initializer
        {
            ApiKey = youtubeApiKey,
            ApplicationName = "Sentimeter"
        }));

        services.AddScoped<IVideoRetriever, YouTubeVideoRetriever>();

        return services;
    }
}
