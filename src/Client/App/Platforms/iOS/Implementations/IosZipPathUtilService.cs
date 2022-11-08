namespace Functionland.FxFiles.Client.App.Platforms.iOS.Implementations;

public class IosZipPathUtilService : ZipPathUtilService
{
    public override string GetRarEntryPath(string itemPath) => itemPath.Replace("\\", "/");
}
