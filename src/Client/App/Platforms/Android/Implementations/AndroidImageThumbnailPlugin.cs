using Android.Graphics;
using Android.Media;
using Stream = System.IO.Stream;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public class AndroidImageThumbnailPlugin : IThumbnailPlugin
{
    public async Task<Stream> CreateThumbnailAsync(Stream input, CancellationToken? cancellationToken = null)
    {
        var bitmap = BitmapFactory.DecodeStream(input);
        var imageThumbnail = await ThumbnailUtils.ExtractThumbnailAsync(bitmap, 252, 146);

        var outputStream = new MemoryStream();
        await imageThumbnail.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);

        return outputStream;
    }

    public bool IsExtensionSupported(string extension)
    {
        throw new NotImplementedException();
    }
}
