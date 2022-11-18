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
        var page = renderer.OpenPage(0);

        if (page is null)
            throw new InvalidOperationException("Page can not be null.");

        (int imageWidth, int imageHeight) = ImageUtils.ScaleImage(page.Width, page.Height, thumbnailScale);

        //ToDo: Check Bitmap.Config nullability (although it seems nonsense at the moment).
        Bitmap? bmp = Bitmap.CreateBitmap(imageWidth, imageHeight, Bitmap.Config.Argb8888);

        if (bmp is null)
            throw new InvalidOperationException("Bitmap can not be null.");

        //Make the background ready (yes, white) in case of pdf background transparency.
        var canvas = new Canvas(bmp);
        canvas.DrawColor(Color.White);
        canvas.DrawBitmap(bmp, 0, 0, null);

        page.Render(bmp, null, null, PdfRenderMode.ForDisplay);

        var outputStream = new MemoryStream();
        await bmp.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);

        page.Close();
        renderer.Close();

        return outputStream;
        
    }


}
