using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Resources;
using Functionland.FxFiles.Client.Shared.Utils;
using System.Drawing;
using Windows.System;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations;

public partial class WindowsThumbnailService : LocalThumbnailService
{
    [AutoInject] public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

    public override async Task<string> MakeThumbnailAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
    {
        var thumbPath = GetThumbnailFullPath(fsArtifact);

        if (File.Exists(thumbPath)) return thumbPath;

        var image = System.Drawing.Image.FromFile(fsArtifact.FullPath);

        image = CorrectRotation(image);

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

    public System.Drawing.Image CorrectRotation(System.Drawing.Image? img)
    {
        if (Array.IndexOf(img.PropertyIdList, 274) > -1)
        {
            var orientation = (int)img.GetPropertyItem(274).Value[0];
            switch (orientation)
            {
                case 1:
                    // No rotation required.
                    break;
                case 2:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case 3:
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 4:
                    img.RotateFlip(RotateFlipType.Rotate180FlipX);
                    break;
                case 5:
                    img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case 6:
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 7:
                    img.RotateFlip(RotateFlipType.Rotate270FlipX);
                    break;
                case 8:
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
            // This EXIF data is now invalid and should be removed.
            img.RemovePropertyItem(274);
        }

        return img;
    }

}
