using Sentimeter.Shared;
using Sentimeter.Web.App.Identity;

namespace Sentimeter.Web.App.Services;

public static class ApiClientsServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(this IServiceCollection services)
    {
        services.AddHttpClient<VideosApiClient>(client =>
        {
            client.BaseAddress = new($"https+http://{ServiceNames.WebApi}/");
        }).AddHttpMessageHandler<SentimeterAuthorizationHandler>();

        return services;
    }
}
