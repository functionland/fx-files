using Functionland.FxFiles.App.Services.Implementations;
using Functionland.FxFiles.Shared.Services.DateTime.Contracts;
using Functionland.FxFiles.Shared.Services.DateTime.Implementations;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddScoped<IStateService, StateService>();
        services.AddScoped<IExceptionHandler, ExceptionHandler>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
