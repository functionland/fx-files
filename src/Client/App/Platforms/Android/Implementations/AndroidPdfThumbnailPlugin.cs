using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using Functionland.FxFiles.Client.Shared.Enums;
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

        var fileDescriptor = ParcelFileDescriptor.Open(new Java.IO.File(filePath), ParcelFileMode.ReadOnly);

        if (fileDescriptor is null)
            throw new InvalidOperationException("fileDescriptor can not be null.");

        PdfRenderer renderer = new(fileDescriptor);
        var page = renderer.OpenPage(1);
        if (page is null)
            throw new InvalidOperationException("Page can not be null.");

        Bitmap? bmp = Bitmap.CreateBitmap(page.Width, page.Height, Bitmap.Config.Argb8888);

        if (bmp is null)
            throw new InvalidOperationException("Bitmap can not be null.");

        //page.Render(bmp, null, null, PdfRenderMode.ForDisplay);

        var outputStream = new MemoryStream();
        await bmp.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);
        return outputStream;
        
        
        ////if (inputStream is not null)
        ////    throw new InvalidOperationException($"Stream is not supported by this plugin.");

        ////if (filePath is null)
        ////{
        ////    throw new InvalidOperationException("FilePath should be provided for this plugin.");
        ////}

        //// Get the first page of the Pdf in bytes out of the stream.
        //var inputPdf = new PdfReader(inputStream);
        //var pageContent = inputPdf.GetPageContent(1);

        //// Convert the first page from bytes to bitmap.
        //var firstPageStream = new MemoryStream(pageContent);
        //var firstPageInBitmapStream =  BitmapFactory.DecodeStream(firstPageStream);

        //var firstPageInBitmapByte = await BitmapFactory.DecodeByteArrayAsync(pageContent, 0, pageContent.Length);

        ////var fileStream = new FileStream()
        ////var fileStreamBitmap = BitmapFactory.DecodeStream(fileStream);
        ////var pdfFileThumbnail = await ThumbnailUtils.ExtractThumbnailAsync(fileStreamBitmap, 1280, 640);

        ////var javaFile = new Java.IO.File("/storage/emulated/0/javaStream1stPage.pdf");

        //// Ready to extract the thumbnail.
        //(int imageWidth, int imageHeight) = ImageUtils.ScaleImage(firstPageInBitmapStream.Width, firstPageInBitmapStream.Height, 1280, 640);
        //var pdfThumbnail = await ThumbnailUtils.ExtractThumbnailAsync(firstPageInBitmapStream, imageWidth, imageHeight);

        //// Turn the output bitmap into stream to send it out.
        //var outputStream = new MemoryStream();
        //await pdfThumbnail.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);

        //return outputStream;
    }


}
