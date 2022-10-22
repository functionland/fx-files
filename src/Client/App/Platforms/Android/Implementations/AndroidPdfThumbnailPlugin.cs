using Android.Graphics.Pdf;
using Android.OS;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Utils;
using Bitmap = Android.Graphics.Bitmap;
using Stream = System.IO.Stream;

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

        //ToDo: ScaleImage needs some changes in order to get the proper size for the output image.
        (int imageWidth, int imageHeight) = ImageUtils.ScaleImage(page.Width, page.Height, 252, 146);

        Bitmap? bmp = Bitmap.CreateBitmap(imageWidth, imageHeight, Bitmap.Config.Argb8888);

        if (bmp is null)
            throw new InvalidOperationException("Bitmap can not be null.");

        page.Render(bmp, null, null, PdfRenderMode.ForDisplay);

        page.Close();
        renderer.Close();

        var outputStream = new MemoryStream();
        await bmp.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);
        return outputStream;
        
    }


}
