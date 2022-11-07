namespace Functionland.FxFiles.Client.App.Platforms.MacCatalyst.Implementations;

public class MacZipPathUtilService : ZipPathUtilService
{
    public override string GetRarEntryPath(string itemPath) => itemPath.Replace("\\", "/");
}
