using Functionland.FxFiles.Client.App.Implementations;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.Services.Contracts.FileViewer;

using System.Reflection.Metadata.Ecma335;

namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class FileViewer
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [AutoInject] public INativeNavigation NativeNavigation { get; set; } = default!;
    private FsArtifact? _currentArtifact;
    private bool _isModalOpen { get; set; } = false;

    public async Task<bool> OpenArtifact(FsArtifact artifact)
    {
        if (!CanOpen(artifact))
            return false;

        
       
        _currentArtifact = artifact;
        if (IsSupported<VideoViewer>(_currentArtifact))
        {
            _isModalOpen = false;
            await NavigateToView(_currentArtifact);
        }
        else
        {
            _isModalOpen = true;
        }

        return true;
    }

    public void Back()
    {
        _isModalOpen = false;
    }

    public bool CanOpen(FsArtifact artifact)
    {
        if (IsSupported<ImageViewer>(artifact))
        {
            return true;
        }
        else if (IsSupported<VideoViewer>(artifact))
        {
            return true;
        }

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

        //if (artifact.FileCategory == FileCategoryType.Image && typeof(TComponent) == typeof(ImageViewer))
        //{
        //    return true;
        //}

        if (artifact.FileCategory == FileCategoryType.Video && typeof(TComponent) == typeof(VideoViewer))
        {
            return true;
        }

        return false;
    }

}
