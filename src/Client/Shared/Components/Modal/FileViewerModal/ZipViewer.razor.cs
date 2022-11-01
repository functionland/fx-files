namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ZipViewer : IFileViewerComponent
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }

    protected override Task OnInitAsync()
    {
        return base.OnInitAsync();
    }
}
