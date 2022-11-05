namespace Functionland.FxFiles.Client.App.Platforms.MacCatalyst.Implementations;

public class MacPathUtilService : PathUtilService
{
    public override string GetRarEntryPath(string itemPath) => itemPath.Replace("\\", "/");
}
