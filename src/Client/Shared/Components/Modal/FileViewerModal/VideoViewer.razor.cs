namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class VideoViewer : IFileViewerComponent
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public bool Visibility { get; set; }

    public bool IsSupported(FsArtifact artifact)
    {
        throw new NotImplementedException();
    }
}
