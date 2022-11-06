using Functionland.FxFiles.Client.Shared.Services.Implementations;

namespace Functionland.FxFiles.Client.Test.Services.Implementations;

public class FakePathUtilService : PathUtilService
{
    public override string GetZipEntryPath(string itemPath)
    {
        return itemPath.Replace("/", "\\");
    }
}