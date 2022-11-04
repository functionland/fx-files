using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Exceptions;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations;

public partial class WindowsFileService : LocalDeviceFileService
{
    public override FsFileProviderType GetFsFileProviderType(string filePath)
    {
        return FsFileProviderType.InternalMemory;
    }

    public override string? GetFsArtifactShowablePath(string? artifactPath)
    {
        if (artifactPath is null)
            throw new ArtifactPathNullException(nameof(artifactPath));

        return artifactPath.Replace('\\', '/');
    }
}
