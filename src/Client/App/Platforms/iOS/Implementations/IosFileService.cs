using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Exceptions;
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

    protected override string GetFolderOrDriveShowablePath(string artifactPath)
    {
        if (artifactPath is null)
            throw new ArtifactPathNullException(nameof(artifactPath));

        //ToDo: Implement iOS version of how to shape the fullPath to be shown in UI.
        return artifactPath;
    }
}
