
namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddScoped<IExceptionHandler, ExceptionHandler>();
        return services;
    }
}
