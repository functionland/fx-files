using Android.Graphics;
using Android.Media;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Utils;
using Stream = System.IO.Stream;
using Size = Android.Util.Size;
using Functionland.FxFiles.Client.Shared.Exceptions;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public class AndroidAudioThumbnailPlugin : AudioThumbnailPlugin
{
    private static List<string> notSupportExtentions => new() { ".m4a" };
    public override bool IsJustFilePathSupported => true;
    protected override async Task<Stream> OnCreateThumbnailAsync(
        Stream? inputAudioStream,
        string? filePath,
        ThumbnailScale thumbnailScale,
        CancellationToken? cancellationToken = null)
    {
        if (inputAudioStream is not null)
            throw new InvalidOperationException("Stream is not supported by this plugin.");

        if (filePath is null)
            throw new InvalidOperationException("FilePath should be provided for this plugin.");

        var file = new Java.IO.File(filePath);
        var (width, height) = ImageUtils.GetHeightAndWidthFromThumbnailScale(thumbnailScale);

        var size = new Size(width, height);
        var outputStream = new MemoryStream();
        Bitmap? audioThumbnail;
        try
        {
            audioThumbnail = ThumbnailUtils.CreateAudioThumbnail(file, size, null);
            await audioThumbnail.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);
        }
        catch (Java.IO.IOException ex) 
        {
            await outputStream.DisposeAsync().AsTask();
            throw new KnownIOException("No album art available to show as thumbnail.", ex);
        }

        return outputStream;
    }

    public override bool IsSupported(string extension)
    {
        if (!base.IsSupported(extension))
        {
            return false;
        }

        return !notSupportExtentions.Contains(extension);
    }
}
