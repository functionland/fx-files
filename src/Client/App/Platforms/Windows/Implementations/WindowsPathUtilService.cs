namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public class WindowsPathUtilService : PathUtilService
{
    public override string GetZipEntryPath(string itemPath)
    {
        return itemPath.Replace("/", "\\");
    }
}
