namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public class AndroidPathUtilService : PathUtilService
{
    public override string GetRarEntryPath(string itemPath) => itemPath.Replace("\\", "/");
}
