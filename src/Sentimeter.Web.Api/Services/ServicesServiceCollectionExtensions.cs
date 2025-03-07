using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace Sentimeter.Web.Api.Services;

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
