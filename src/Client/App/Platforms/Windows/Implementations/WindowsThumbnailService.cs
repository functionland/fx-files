using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Resources;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations;

public partial class WindowsThumbnailService : LocalThumbnailService
{
    [AutoInject] public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

    public override async Task<string> MakeThumbnailAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
    {
        var thumbPath = GetThumbnailFullPath(fsArtifact);

        if (File.Exists(thumbPath)) return thumbPath;

        var image = System.Drawing.Image.FromFile(fsArtifact.FullPath);

        var imageWidth = image.Width / 2;
        var imageHeight = image.Height / 2;

        if (imageWidth <= 252)
        {
            imageWidth = image.Width;
        }

        if (imageHeight <= 146)
        {
            imageHeight = image.Height;
        }

        var thumb = image.GetThumbnailImage(imageWidth, imageHeight, () => false, IntPtr.Zero);
        try
        {
            thumb.Save(thumbPath);
        }
        catch (Exception)
        {
            thumb.Dispose();
        }

        return thumbPath;
    }

    public override string GetAppCacheDirectory()
    {
        return FileSystem.CacheDirectory;
    }
}
