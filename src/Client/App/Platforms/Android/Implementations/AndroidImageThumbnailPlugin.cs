﻿using Android.Graphics;
using Android.Media;
using Functionland.FxFiles.Client.Shared.Utils;
using Stream = System.IO.Stream;
using Bitmap = Android.Graphics.Bitmap;
using ExifInterface = AndroidX.ExifInterface.Media.ExifInterface;
using Functionland.FxFiles.Client.Shared.Models;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public class AndroidImageThumbnailPlugin : ImageThumbnailPlugin
{
    protected override async Task<Stream> OnCreateThumbnailAsync(Stream input, CancellationToken? cancellationToken = null)
    {
        var bitmap = BitmapFactory.DecodeStream(input);

        //ToDo: Null check for bitmap
        bitmap = CorrectRotationIfNeeded(input, bitmap);
        (int imageWidth, int imageHeight) = ImageUtils.ScaleImage(bitmap.Width, bitmap.Height, 252, 146);
        var imageThumbnail = await ThumbnailUtils.ExtractThumbnailAsync(bitmap, imageWidth, imageHeight);

        //ToDo: Null check for imageThumbnail
        var outputStream = new MemoryStream();
        await imageThumbnail.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);

        return outputStream;
    }

    private Bitmap? CorrectRotationIfNeeded(Stream input, Bitmap? bitmap)
    {
        if (bitmap == null) return null;

        //The cast wast needed in order to access to Name property.
        //absolutePath is needed here, to be able to work with ExifInterface. Only FileStram provides the absolutePath through Name property.
        //ToDo: Check if FileStream is OK here.
        var ei = new ExifInterface(((FileStream)input).Name);
        int orientation = ei.GetAttributeInt(ExifInterface.TagOrientation, ExifInterface.OrientationUndefined);

        Bitmap? rotatedBitmap = null;
        rotatedBitmap = orientation switch
        {
            ExifInterface.OrientationRotate90 => RotateImage(bitmap, 90),
            ExifInterface.OrientationRotate180 => RotateImage(bitmap, 180),
            ExifInterface.OrientationRotate270 => RotateImage(bitmap, 270),
            _ => bitmap,
        };
        return rotatedBitmap;
    }

    private static Bitmap? RotateImage(Bitmap source, float angle)
    {
        var matrix = new Matrix();
        matrix.PostRotate(angle);
        return Bitmap.CreateBitmap(source, 0, 0, source.Width, source.Height, matrix, true);
    }

}
