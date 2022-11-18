using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Utils;
using Bitmap = Android.Graphics.Bitmap;
using Stream = System.IO.Stream;
using Color = Android.Graphics.Color;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public class AndroidPdfThumbnailPlugin : PdfThumbnailPlugin
{
    public override bool IsJustFilePathSupported => true;
    protected override async Task<Stream> OnCreateThumbnailAsync(
        Stream? inputPdfStream,
        string? filePath,
        ThumbnailScale thumbnailScale,
        CancellationToken? cancellationToken = null)
    {
        if (inputPdfStream is not null)
            throw new InvalidOperationException($"Stream is not supported by this plugin.");

        if (filePath is null)
            throw new InvalidOperationException("FilePath should be provided for this plugin.");

        var pdfFile = ParcelFileDescriptor.Open(new Java.IO.File(filePath), ParcelFileMode.ReadOnly);

        if (pdfFile is null)
            throw new InvalidOperationException("pdfFile can not be null.");

        PdfRenderer renderer = new(pdfFile);
        var firstPage = renderer.OpenPage(0);

        if (firstPage is null)
            throw new InvalidOperationException("Page can not be null.");

        (int thumbnailWidth, int thumbnailHeight) = ImageUtils.ScaleImage(firstPage.Width, firstPage.Height, thumbnailScale);

        //ToDo: Check Bitmap.Config nullability (although it seems nonsense at the moment).
        var bitmap = Bitmap.CreateBitmap(thumbnailWidth, thumbnailHeight, Bitmap.Config.Argb8888);

        if (bitmap is null)
            throw new InvalidOperationException("Bitmap can not be null.");

        //Make the background ready (yes, white) in case of pdf background transparency.
        var canvas = new Canvas(bitmap);
        canvas.DrawColor(Color.White);
        canvas.DrawBitmap(bitmap, 0, 0, null);

        firstPage.Render(bitmap, null, null, PdfRenderMode.ForDisplay);

        var outputStream = new MemoryStream();
        await bitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);

        firstPage.Close();
        renderer.Close();

        return outputStream;
        
    }


}
