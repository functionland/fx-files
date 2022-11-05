namespace Functionland.FxFiles.Client.App.Platforms.iOS.Implementations;

public class IosPathUtilService : PathUtilService
{
    public override string GetRarEntryPath(string itemPath) => itemPath.Replace("\\", "/");
}
