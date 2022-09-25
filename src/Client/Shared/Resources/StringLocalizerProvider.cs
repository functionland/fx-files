using System.Reflection;

namespace Functionland.FxFiles.Client.Shared.Resources;

public static class StringLocalizerProvider
{
    public static IStringLocalizer ProvideLocalizer(Type dtoType, IStringLocalizerFactory factory)
    {
        return factory.Create(dtoType.GetCustomAttribute<ModelResourceTypeAttribute>()?.ResourceType ?? typeof(AppStrings));
    }
}
