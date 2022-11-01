namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class FileViewer
{
    [Parameter] public IFileService FileService { get; set; } = default!;

    private FsArtifact? _currentArtifact;
    private bool _isModalOpen { get; set; } = false;

    public bool OpenArtifact(FsArtifact artifact)
    {
        if (!CanOpen(artifact))
            return false;

        _isModalOpen = true;
        _currentArtifact = artifact;
        StateHasChanged();
        return true;
    }

    public void Back()
    {
        _isModalOpen = false;
    }

    public bool CanOpen(FsArtifact artifact)
    {
        if (IsSupported<ImageViewer>(artifact))
            return true;
        else if (IsSupported<VideoViewer>(artifact))
            return true;
        else if (IsSupported<ZipViewer>(artifact))
            return true;
        else if (IsSupported<TextViewer>(artifact))
            return true;

        return false;
    }

    private bool IsSupported<TComponent>(FsArtifact? artifact)
        where TComponent : IFileViewerComponent
    {
        if (artifact is null)
            return false;

        if (typeof(TComponent) == typeof(ZipViewer) && artifact.FileCategory == FileCategoryType.Zip)
            return true;
        else if (typeof(TComponent) == typeof(ZipViewer) && artifact.FileCategory == FileCategoryType.Document)
            return true;

        return false;
    }
}
