using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Models;

namespace Functionland.FxFiles.Client.App.Platforms.iOS.Implementations;

public partial class IosFileService : LocalDeviceFileService
{
    public override IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, string? searchText = null, CancellationToken? cancellationToken = null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            path = "./";
        }

        return base.GetArtifactsAsync(path, searchText, cancellationToken);
    }

    public override async Task<FsFileProviderType> GetFsFileProviderTypeAsync(string filePath)
    {
        return FsFileProviderType.InternalMemory;
    }
}
