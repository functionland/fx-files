using Functionland.FxFiles.Client.Shared.Enums;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations;

public partial class WindowsFileService : LocalDeviceFileService
{
    public override FsFileProviderType GetFsFileProviderType(string filePath)
    {
        return FsFileProviderType.InternalMemory;
    }
}
