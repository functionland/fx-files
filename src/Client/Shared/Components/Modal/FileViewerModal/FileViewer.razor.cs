using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.Services.Contracts.FileViewer;

namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class FileViewer
{
    [Parameter] public IFileService FileService { get; set; } = default!;

    private FsArtifact? _currentArtifact;
    private bool _isModalOpen { get; set; } = false;

    private List<IFileViewerComponent> _fileViewers = default!;
    private ImageViewer? _imageRef;
    private VideoViewer? _videoRef;

    protected override void OnAfterRender(bool firstRender)
    {
        _fileViewers = new()
        {
            _imageRef,
            _videoRef
        };

        base.OnAfterRender(firstRender);
    }

    public bool OpenArtifact(FsArtifact artifact)
    {
        SetFileViewersVisibilty(false);
        var fileviewer = _fileViewers.FirstOrDefault(fv => fv is not null && fv.IsSupported(artifact));

        if (fileviewer is null)
            return false;

        fileviewer.Visibility = true;
        _isModalOpen = true;

        _currentArtifact = artifact;
        return true;
    }

    public void Back()
    {
        _isModalOpen = false;
        SetFileViewersVisibilty(false);
    }

    private void SetFileViewersVisibilty(bool visibilty)
    {
        _fileViewers.ForEach(fv => fv.Visibility = false);
    }
}
