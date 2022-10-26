using Functionland.FxFiles.Client.Shared.Enums;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations;

public partial class WindowsFileService : LocalDeviceFileService
{
    public override async Task<FsFileProviderType> GetFsFileProviderTypeAsync(string filePath)
    {
        return FsFileProviderType.InternalMemory;
    }
}
