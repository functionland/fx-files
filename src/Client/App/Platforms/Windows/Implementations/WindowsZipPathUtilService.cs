namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public class WindowsZipPathUtilService : ZipPathUtilService
{
    public override string GetZipEntryPath(string itemPath)
    {
        return itemPath.Replace("/", "\\");
    }
}
