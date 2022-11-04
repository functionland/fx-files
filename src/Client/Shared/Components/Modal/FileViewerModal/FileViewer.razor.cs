namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class FileViewer
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public EventCallback OnBack { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> OnPin { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> OnUnpin { get; set; }
    [Parameter] public EventCallback<FsArtifact> OnOptionClick { get; set; }

    public bool IsModalOpen { get; set; } = false;

    private FsArtifact? _currentArtifact;

    public bool OpenArtifact(FsArtifact artifact)
    {
        if (!CanOpen(artifact))
            return false;

        IsModalOpen = true;
        _currentArtifact = artifact;
        StateHasChanged();
        return true;
    }

    private bool CanOpen(FsArtifact artifact)
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

        if (typeof(TComponent) == typeof(ImageViewer) && artifact.FileCategory == FileCategoryType.Image)
            return true;
        else if (typeof(TComponent) == typeof(ZipViewer) && artifact.FileCategory == FileCategoryType.Zip)
            return true;
        else if (typeof(TComponent) == typeof(TextViewer) && new string[] { ".txt" }.Contains(artifact.FileExtension))
            return true;

        return false;
    }

    public async Task HandleBackAsync()
    {
        IsModalOpen = false;
        await OnBack.InvokeAsync();
    }
}
