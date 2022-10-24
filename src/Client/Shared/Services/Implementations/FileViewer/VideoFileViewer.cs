using Functionland.FxFiles.Client.Shared.Pages.FileViewer;
using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.FileViewer;
public class VideoFileViewer : BlazorFileViewer<VideoFileViewerPage>
{
    public VideoFileViewer(NavigationManager navigationManager) : base(navigationManager)
    {
    }

    protected override async Task<bool> OnIsSupportedAsync(string artrifactPath, IFileService fileService, CancellationToken? cancellationToken = null)
    {
        var extension = Path.GetExtension(artrifactPath);
        var videoTypes = FsArtifactUtils.FileExtentionsType.Where(e => e.Value == FileCategoryType.Video);
        return videoTypes.Any(v => v.Key == extension);
    }

    //protected override async Task<bool> OnIsSupportedAsync(string artrifactPath,
    //                                                       IFileService fileService,
    //                                                       CancellationToken? cancellationToken = null)
    //    => new string[] { ".mp4", ".mkv" }.Contains(Path.GetExtension(artrifactPath));
}