using Functionland.FxFiles.Client.App.Implementations;

namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class FileViewer
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public IArtifactThumbnailService<IFileService> ThumbnailService { get; set; } = default!;
    [Parameter] public EventCallback OnBack { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> OnPin { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> OnUnpin { get; set; }
    [Parameter] public EventCallback<FsArtifact> OnOptionClick { get; set; }
    [AutoInject] public INativeNavigation NativeNavigation { get; set; } = default!;
    [Parameter] public EventCallback<Tuple<FsArtifact, List<FsArtifact>?, string?>> OnExtract { get; set; }

    public bool IsModalOpen { get; set; } = false;

    private FsArtifact? _currentArtifact;

    public async Task<bool> OpenArtifact(FsArtifact artifact)
    {
        if (!CanOpen(artifact))
            return false;

        _currentArtifact = artifact;
        if (IsSupported<VideoViewer>(_currentArtifact))
        {
            IsModalOpen = false;
            await NavigateToView(_currentArtifact);
        }
        else
        {
            IsModalOpen = true;
        }

        StateHasChanged();
        return true;
    }

    private bool CanOpen(FsArtifact artifact)
    {
        if (IsSupported<ImageViewer>(artifact))
            return true;
        if (IsSupported<VideoViewer>(artifact))
            return true;
        if (IsSupported<ZipViewer>(artifact))
            return true;
        if (IsSupported<TextViewer>(artifact))
            return true;

        return false;
    }

    public async Task NavigateToView(FsArtifact artifact)
    {
        await NativeNavigation.NavigateToVidoeViewer(artifact.FullPath);
    }

    private bool IsSupported<TComponent>(FsArtifact? artifact)
        where TComponent : IFileViewerComponent
    {
        if (artifact is null)
            return false;

        if (typeof(TComponent) == typeof(ImageViewer) && artifact.FileCategory == FileCategoryType.Image)
            return true;
        if (typeof(TComponent) == typeof(VideoViewer) && artifact.FileCategory == FileCategoryType.Video)
            return true;
        if (typeof(TComponent) == typeof(ZipViewer) && artifact.FileCategory == FileCategoryType.Zip)
            return true;
        if (typeof(TComponent) == typeof(TextViewer) && new string[] { ".txt" }.Contains(artifact.FileExtension))
            return true;

        return false;
    }

    public async Task HandleBackAsync()
    {
        IsModalOpen = false;
        await OnBack.InvokeAsync();
    }
}