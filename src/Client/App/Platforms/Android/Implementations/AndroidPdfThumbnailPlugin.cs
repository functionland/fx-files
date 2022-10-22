using Android.Graphics;
using Android.Media;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Utils;
using iTextSharp.text.pdf;
using Java.Security;
using Spire.Pdf;
using Spire.Pdf.OPC;
using System.IO;
using System.Reflection.PortableExecutable;
using static Android.Provider.DocumentsContract;
using Bitmap = Android.Graphics.Bitmap;
using PdfDocument = iTextSharp.text.pdf.PdfDocument;
using Stream = System.IO.Stream;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public class AndroidPdfThumbnailPlugin : PdfThumbnailPlugin
{
    public override bool IsJustFilePathSupported => false;
    protected override async Task<Stream> OnCreateThumbnailAsync(
        Stream? inputStream,
        string? filePath,
        ThumbnailScale thumbnailScale,
        CancellationToken? cancellationToken = null)
    {
        //if (inputStream is not null)
        //    throw new InvalidOperationException($"Stream is not supported by this plugin.");

        //if (filePath is null)
        //{
        //    throw new InvalidOperationException("FilePath should be provided for this plugin.");
        //}

        // Get the first page of the Pdf in bytes out of the stream.
        var inputPdf = new PdfReader(inputStream);
        var pageContent = inputPdf.GetPageContent(1);

        // Convert the first page from bytes to bitmap.
        var firstPageStream = new MemoryStream(pageContent);
        var firstPageInBitmapStream =  BitmapFactory.DecodeStream(firstPageStream);

        var firstPageInBitmapByte = await BitmapFactory.DecodeByteArrayAsync(pageContent, 0, pageContent.Length);

        //var fileStream = new FileStream()
        //var fileStreamBitmap = BitmapFactory.DecodeStream(fileStream);
        //var pdfFileThumbnail = await ThumbnailUtils.ExtractThumbnailAsync(fileStreamBitmap, 1280, 640);

        //var javaFile = new Java.IO.File("/storage/emulated/0/javaStream1stPage.pdf");

        // Ready to extract the thumbnail.
        (int imageWidth, int imageHeight) = ImageUtils.ScaleImage(firstPageInBitmapStream.Width, firstPageInBitmapStream.Height, 1280, 640);
        var pdfThumbnail = await ThumbnailUtils.ExtractThumbnailAsync(firstPageInBitmapStream, imageWidth, imageHeight);

        // Turn the output bitmap into stream to send it out.
        var outputStream = new MemoryStream();
        await pdfThumbnail.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);

        return outputStream;
    }
}
