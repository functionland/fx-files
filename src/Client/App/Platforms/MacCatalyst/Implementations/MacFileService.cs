using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.App.Platforms.MacCatalyst.Implementations;

public partial class MacFileService : LocalDeviceFileService
{
    public override FsFileProviderType GetFsFileProviderType(string filePath)
    {
        return FsFileProviderType.InternalMemory;
    }

    protected override string GetFolderOrDriveShowablePath(string artifactPath)
    {
        if (artifactPath is null)
            throw new ArtifactPathNullException(nameof(artifactPath));

        //ToDo: Implement Mac version of how to shape the fullPath to be shown in UI.
        return artifactPath;
    }
}
