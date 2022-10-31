using Functionland.FxFiles.Client.Shared.Enums;

namespace Functionland.FxFiles.Client.App.Platforms.iOS;

public partial class IosZipService : ZipService
{
    public override FsFileProviderType GetFsFileProviderType(string filePath)
    {
        return FsFileProviderType.InternalMemory;
    }
}
