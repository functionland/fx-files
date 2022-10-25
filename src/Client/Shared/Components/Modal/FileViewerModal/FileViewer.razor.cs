using Functionland.FxFiles.Client.Shared.Services.Contracts.FileViewer;

namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class FileViewer
{
    [Parameter] public IFileService FileService { get; set; } = default!;

    private FsArtifact? _currentArtifact;

    private List<IFileViewerComponent> _fileViewers = default!;
    private ImageViewer? _imageRef;
    private VideoViewer? _videoRef;

    protected override Task OnInitAsync()
    {
        _fileViewers = new()
        {
            _imageRef,
            _videoRef
        };

        return base.OnInitAsync();
    }

    public bool OpenArtifactAsync(FsArtifact artifact)
    {
        var fileviewer = _fileViewers.FirstOrDefault(fv => fv.IsSupported(artifact));

        if (fileviewer is null)
            return false;

        fileviewer.Visibility = true;

        _currentArtifact = artifact;
        return true;
    }
}
