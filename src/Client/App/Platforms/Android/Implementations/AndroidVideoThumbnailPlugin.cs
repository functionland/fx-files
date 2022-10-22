﻿using Android.Graphics;
using Android.Media;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Utils;
using Bitmap = Android.Graphics.Bitmap;
using ExifInterface = AndroidX.ExifInterface.Media.ExifInterface;
using Stream = System.IO.Stream;
using Size = Android.Util.Size;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public class AndroidVideoThumbnailPlugin : VideoThumbnailPlugin
{
    public override bool IsJustFilePathSupported => true;
    protected override async Task<Stream> OnCreateThumbnailAsync(
        Stream? inputVideoStream,
        string? filePath, 
        ThumbnailScale thumbnailScale,
        CancellationToken? cancellationToken = null)
    {
        if (inputVideoStream is not null)
            throw new InvalidOperationException($"Stream is not supported by this plugin.");

        if (filePath is null)
        {
            throw new InvalidOperationException("FilePath should be provided for this plugin.");
        }

        var size = new Size(1280, 640);
        var bitmap = await ThumbnailUtils.CreateVideoThumbnailAsync(new Java.IO.File(filePath), size, null);
        var outputStream = new MemoryStream();
        await bitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);

        return outputStream;
    }
}