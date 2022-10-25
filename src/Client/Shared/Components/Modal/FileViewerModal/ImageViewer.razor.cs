using Functionland.FxFiles.Client.Shared.Services.Contracts.FileViewer;

namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ImageViewer :IFileViewerComponent
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public bool Visibility { get; set; }

    public bool IsSupported(FsArtifact artifact)
    {
        if (artifact.FileCategory == FileCategoryType.Image)
            return true;

        return false;
    }
}
