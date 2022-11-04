using Android.Media;
using Functionland.FxFiles.Client.Shared.Enums;
using Bitmap = Android.Graphics.Bitmap;
using Stream = System.IO.Stream;
using Size = Android.Util.Size;
using MediaMetadataRetriever = Android.Media.MediaMetadataRetriever;
using Functionland.FxFiles.Client.Shared.Utils;

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
            throw new InvalidOperationException("FilePath should be provided for this plugin.");

        //Grap the first frame of the video in order to pass it to ScaleImage method for the proper size of the output thumbnail.
        var media = new MediaMetadataRetriever();
        media.SetDataSource(filePath);
        var firstFrame = media.GetFrameAtTime(0);

        if (firstFrame is null)
            throw new InvalidOperationException("Unable to retrieve the first frame of the video input.");

        (int frameWidth, int frameHeight) = ImageUtils.ScaleImage(firstFrame.Width, firstFrame.Height, thumbnailScale);
        var size = new Size(frameWidth, frameHeight);

        var bitmap = await ThumbnailUtils.CreateVideoThumbnailAsync(new Java.IO.File(filePath), size, null);
        var outputStream = new MemoryStream();
        await bitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);

        return outputStream;
    }

    public override bool IsSupported(string extension)
    {
        if (base.IsSupported(extension)) return true;

        return new string[]
        {
            ".f4v"
        }.Contains(extension.ToLower());

    }
}
