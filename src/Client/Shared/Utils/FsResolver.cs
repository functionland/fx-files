namespace Functionland.FxFiles.Client.Shared.Utils;

public static class FsResolver
{
    public static IServiceProvider? ServiceProvider { get; private set; }
    public static T Resolve<T>() where T : class
        => ServiceProvider?.GetRequiredService<T>() ?? 
        throw new NullReferenceException(nameof(ServiceProvider));

    public static void UseResolver(this IServiceProvider sp)
    {
        ServiceProvider = sp;
    }
}
