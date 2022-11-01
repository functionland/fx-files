namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class TextViewer : IFileViewerComponent
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
}