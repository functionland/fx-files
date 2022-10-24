using Functionland.FxFiles.Client.Shared.Services.Contracts;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class FsFileProviderDependency
{
    [AutoInject] public ILocalDeviceFileService LocalDeviceFileService { get; set; }
    [AutoInject] public IFulaFileService FulaFileService { get; set; }
    [AutoInject] public IArtifactThumbnailService<ILocalDeviceFileService> LocalArtifactThumbnailService { get; set; }
    [AutoInject] public IArtifactThumbnailService<IFulaFileService> FulaArtifactThumbnailService { get; set; }


}
