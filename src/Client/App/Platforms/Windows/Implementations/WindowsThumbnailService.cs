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

    private System.Drawing.Image? CorrectRotation(System.Drawing.Image? image)
    {
        if (image != null && Array.IndexOf(image.PropertyIdList, 274) > -1)
        {
            var orientationByte = image.GetPropertyItem(274)?.Value?[0];
            var orientation = orientationByte == null ? 0 : (int)orientationByte;

            switch (orientation)
            {
                case 1:
                    // No rotation required.
                    break;
                case 2:
                    image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case 3:
                    image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 4:
                    image.RotateFlip(RotateFlipType.Rotate180FlipX);
                    break;
                case 5:
                    image.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case 6:
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 7:
                    image.RotateFlip(RotateFlipType.Rotate270FlipX);
                    break;
                case 8:
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
            image.RemovePropertyItem(274);
        }

        return image;
    }

}
