using Android.Graphics;
using Android.Media;
using Functionland.FxFiles.Client.Shared.Utils;
using Stream = System.IO.Stream;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public class AndroidImageThumbnailPlugin : ImageThumbnailPlugin
{
    protected override async Task<Stream> OnCreateThumbnailAsync(Stream input, CancellationToken? cancellationToken = null)
    {
        var bitmap = BitmapFactory.DecodeStream(input);

        //ToDo: Null check for bitmap
        (int imageWidth, int imageHeight) = ImageUtils.ScaleImage(bitmap.Width, bitmap.Height, 252, 146);
        var imageThumbnail = await ThumbnailUtils.ExtractThumbnailAsync(bitmap, imageWidth, imageHeight);

        //ToDo: Null check for imageThumbnail
        var outputStream = new MemoryStream();
        await imageThumbnail.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);

        return outputStream;
    }
}
