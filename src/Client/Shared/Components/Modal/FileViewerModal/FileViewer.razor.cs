using System.Reflection.Metadata.Ecma335;

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

        return false;
    }

    private bool IsSupported<TComponent>(FsArtifact? artifact)
        where TComponent : IFileViewerComponent
    {
        if (artifact is null)
            return false;

        if (artifact.FileCategory == FileCategoryType.Zip && typeof(TComponent) == typeof(ZipViewer))
            return true;

        //if (artifact.FileCategory == FileCategoryType.Image && typeof(TComponent) == typeof(ImageViewer))
        //{
        //    return true;
        //}

        return false;
    }
}
