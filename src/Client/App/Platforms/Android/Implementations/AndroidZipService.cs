using Functionland.FxFiles.Client.Shared.Enums;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public partial class AndroidZipService : ZipService
{
    public override FsFileProviderType GetFsFileProviderType(string filePath)
    {
        return FsFileProviderType.InternalMemory;
    }
}
