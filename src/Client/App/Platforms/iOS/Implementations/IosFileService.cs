using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.App.Platforms.iOS.Implementations;

public partial class IosFileService : LocalDeviceFileService
{
    public override IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, CancellationToken? cancellationToken = null)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            path = "./";
        }

        return base.GetArtifactsAsync(path, cancellationToken);
    }

    public override FsFileProviderType GetFsFileProviderType(string filePath)
    {
        return FsFileProviderType.InternalMemory;
    }

    public override List<FsArtifact> GetDrives()
    {
        return new List<FsArtifact>();
    }
}
