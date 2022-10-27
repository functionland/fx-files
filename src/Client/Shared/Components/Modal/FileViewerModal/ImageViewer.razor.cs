using Functionland.FxFiles.Client.Shared.Services.Contracts.FileViewer;

namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ImageViewer :IFileViewerComponent
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
}
