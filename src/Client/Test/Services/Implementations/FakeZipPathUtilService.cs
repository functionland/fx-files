using Functionland.FxFiles.Client.Shared.Services.Implementations;

namespace Functionland.FxFiles.Client.Test.Services.Implementations;

public class FakeZipPathUtilService : ZipPathUtilService
{
    public override string GetZipEntryPath(string itemPath)
    {
        return itemPath.Replace("/", "\\");
    }
}