using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Resources;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.Services.Implementations;

namespace Functionland.FxFiles.Client.Test.Services.Implementations;

public partial class GenericFileService : LocalDeviceFileService, ILocalDeviceFileService
{
    public GenericFileService(IStringLocalizer<AppStrings> stringLocalizer) : base(stringLocalizer)
    {
        
    }

    public override FsFileProviderType GetFsFileProviderType(string filePath)
    {
        return FsFileProviderType.InternalMemory;
    }

    public override string GetShowablePath(string artifactPath)
    {
        if (artifactPath is null)
            throw new ArtifactPathNullException(nameof(artifactPath));

        return artifactPath.Replace($"{Path.VolumeSeparatorChar}", "")
            .Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}