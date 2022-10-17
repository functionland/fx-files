using Android.Graphics;

using AndroidX.ExifInterface.Media;

using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Services.Implementations.Thumbnail;
using Functionland.FxFiles.Client.Shared.Utils;

using Java.IO;

using Microsoft.Maui.Graphics.Platform;

using System.Drawing;

using static Android.Webkit.WebStorage;

using Bitmap = Android.Graphics.Bitmap;
using IImage = Microsoft.Maui.Graphics.IImage;
using Size = Microsoft.Maui.Graphics.Size;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public class AndroidThumbnailService : ThumbnailService
{
    public override async Task<string> MakeThumbnailAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
    {
        var thumbPath = GetThumbnailFullPath(fsArtifact);

        if (System.IO.File.Exists(thumbPath)) return thumbPath;

        var imageFile = new Java.IO.File(fsArtifact.FullPath);
        var bitmap = BitmapFactory.DecodeFile(imageFile.AbsolutePath);
        bitmap = CorrectRotation(fsArtifact.FullPath, bitmap);

        if (bitmap != null)
        {
            (int imageWidth, int imageHeight) = ImageUtils.ScaleImage((int)bitmap.Width, (int)bitmap.Height, 252, 146);
            var newBitmap = bitmap.Downsize(imageWidth, imageHeight);
            using var fileStream = new FileStream(thumbPath, FileMode.Create);
            await newBitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, fileStream);
        }

        return thumbPath;
    }

    public override string GetAppCacheDirectory()
    {
        return MauiApplication.Current.CacheDir.Path;
    }

    private Bitmap? CorrectRotation(string photoPath, Bitmap? bitmap)
    {
        if(bitmap == null) return null;

        ExifInterface ei = new ExifInterface(photoPath);
        int orientation = ei.GetAttributeInt(ExifInterface.TagOrientation,ExifInterface.OrientationUndefined);

        Bitmap? rotatedBitmap = null;
        switch (orientation)
        {
            case ExifInterface.OrientationRotate90:
                rotatedBitmap = rotateImage(bitmap, 90);
                break;

            case ExifInterface.OrientationRotate180:
                rotatedBitmap = rotateImage(bitmap, 180);
                break;

            case ExifInterface.OrientationRotate270:
                rotatedBitmap = rotateImage(bitmap, 270);
                break;

            case ExifInterface.OrientationNormal:
            default:
                rotatedBitmap = bitmap;
                break;
        }

        return rotatedBitmap;
    }

    private static Bitmap? rotateImage(Bitmap source, float angle)
    {
        Matrix matrix = new Matrix();
        matrix.PostRotate(angle);
        return Bitmap.CreateBitmap(source, 0, 0, source.Width, source.Height, matrix, true);
    }
}
