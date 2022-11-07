namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class ZipPathUtilService : IZipPathUtilService
{
    public virtual string GetZipEntryPath(string itemPath) => itemPath;

    public virtual string GetRarEntryPath(string itemPath) => itemPath;
}
