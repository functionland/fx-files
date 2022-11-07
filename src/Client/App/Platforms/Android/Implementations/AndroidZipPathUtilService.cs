namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public class AndroidZipPathUtilService : ZipPathUtilService
{
    public override string GetRarEntryPath(string itemPath) => itemPath.Replace("\\", "/");
}
