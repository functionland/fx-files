namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class PathUtilService : IPathUtilService
{
    public virtual string GetZipEntryPath(string itemPath) => itemPath;

    public virtual string GetRarEntryPath(string itemPath) => itemPath;
}
