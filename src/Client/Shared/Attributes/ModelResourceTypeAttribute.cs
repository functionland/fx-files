namespace Functionland.FxFiles.Client.Shared.Attributes;

/// <summary>
/// Gets or sets the resource type to use for error message & localizations lookups.
/// </summary>
public class ModelResourceTypeAttribute : Attribute
{
    public Type ResourceType { get; }

    public ModelResourceTypeAttribute(Type resourceType)
    {
        ResourceType = resourceType;
    }
}
