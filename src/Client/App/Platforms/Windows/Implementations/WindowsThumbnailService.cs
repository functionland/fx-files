using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Resources;
using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations;

public partial class WindowsThumbnailService : LocalThumbnailService
{
    [AutoInject] public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

    public override async Task<string> MakeThumbnailAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
    {
        var thumbPath = GetThumbnailFullPath(fsArtifact);

        if (File.Exists(thumbPath)) return thumbPath;

        var image = System.Drawing.Image.FromFile(fsArtifact.FullPath);

        (int imageWidth, int imageHeight) = ImageUtils.ScaleImage(image.Width, image.Height, 252, 146);

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
