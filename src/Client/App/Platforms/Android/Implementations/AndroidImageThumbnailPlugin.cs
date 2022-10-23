﻿using Android.Graphics;
using Android.Media;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Utils;
using Bitmap = Android.Graphics.Bitmap;
using ExifInterface = AndroidX.ExifInterface.Media.ExifInterface;
using Stream = System.IO.Stream;
using Size = Android.Util.Size;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public class AndroidImageThumbnailPlugin : ImageThumbnailPlugin
{
    protected override async Task<Stream> OnCreateThumbnailAsync(Stream? stream, string? filePath, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null)
    {
        // Todo: Exception
        if (filePath is null && stream is null)
            throw new InvalidOperationException($"{nameof(filePath)} and {nameof(stream)}");

        //ToDo: Ask about which one should come first; stream or filePath. Any prioroties?
        if (stream is not null)
        {
            var bitmap = await BitmapFactory.DecodeStreamAsync(stream);

            if (bitmap is null)
                throw new InvalidDataException("Input stream must be in its correct format for image thumbnails in order to be able to create bitmap out of it.");

            bitmap = CorrectRotationIfNeeded(stream, bitmap);

            (int imageWidth, int imageHeight) = ImageUtils.ScaleImage(bitmap.Width, bitmap.Height, 252, 146);
            var imageThumbnail = await ThumbnailUtils.ExtractThumbnailAsync(bitmap, imageWidth, imageHeight);

            if (imageThumbnail is null)
                throw new InvalidOperationException("Unable to create bitmap thumbnail.");

            var outputStream = new MemoryStream();
            await imageThumbnail.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);

            return outputStream;
        }
        else //if (filePath is not null)
        {
            var file = new Java.IO.File(filePath); // We'll be here ONLY WHEN filePath IS NOT NULL! I don't understand you!

            var bitmap = await BitmapFactory.DecodeFileAsync(filePath);

            if (bitmap is null)
                throw new InvalidOperationException("Unable to create bitmap thumbnail.");

            (int imageWidth, int imageHeight) = ImageUtils.ScaleImage(bitmap.Width, bitmap.Height, 252, 146);

            var size = new Size(imageWidth, imageHeight);
            var imageThumbnail = ThumbnailUtils.CreateImageThumbnail(file, size, null);

            var outputStream = new MemoryStream();
            await imageThumbnail.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);

            return outputStream;
        }
    }

    private Bitmap CorrectRotationIfNeeded(Stream? inputStream, Bitmap bitmap)
    {
        if (inputStream is null)
            throw new InvalidOperationException("InputStream can not be null");

        //The cast wast needed in order to access to Name property.
        //absolutePath is needed here, to be able to work with ExifInterface. Only FileStram provides the absolutePath through Name property.
        //ToDo: Check if FileStream is OK here.
        var ei = new ExifInterface(((FileStream)inputStream).Name);

        int orientation = ei.GetAttributeInt(ExifInterface.TagOrientation, ExifInterface.OrientationUndefined);

        var rotatedBitmap = orientation switch
        {
            ExifInterface.OrientationRotate90 => RotateImage(bitmap, 90),
            ExifInterface.OrientationRotate180 => RotateImage(bitmap, 180),
            ExifInterface.OrientationRotate270 => RotateImage(bitmap, 270),
            _ => bitmap,
        };
        return rotatedBitmap;
    }

    private Bitmap RotateImage(Bitmap source, float angle)
    {
        var matrix = new Matrix();
        matrix.PostRotate(angle);
        var rotatedBitmap = Bitmap.CreateBitmap(source, 0, 0, source.Width, source.Height, matrix, true);

        if (rotatedBitmap is null)
            throw new Exception("Unable to create the bitmap.");

        return rotatedBitmap;
    }
}
