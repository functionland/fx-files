using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Utils;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations
{
    public class WindowsPdfThumbnailPlugin : PdfThumbnailPlugin
    {
        protected override async Task<Stream> OnCreateThumbnailAsync(Stream? stream, string? filePath, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null)
        {
            if (stream == null && filePath == null)
                throw new InvalidOperationException($"Both can not be null: {nameof(stream)},{nameof(filePath)}");

            Stream? fileStream = null;
            if (stream is null && filePath != null)
            {
                fileStream = File.OpenRead(filePath);
            }

            try
            {
                var outStream = await Task.Run(() =>
                {
                    PdfDocument document = new();

                    try
                    {
                        // This try-catch is because of the limitaion of reading the 10 pages of pdf file due to free edition.
                        // And this will convert only 3 pages of those 10 pages to image.
                        document.LoadFromStream(stream);
                    }
                    catch { }

                    System.Drawing.Image image = document.SaveAsImage(0);

                    (int imageWidth, int imageHeight) = ImageUtils.ScaleImage(image.Width, image.Height, thumbnailScale);

                    var thumb = image.GetThumbnailImage(imageWidth, imageHeight, () => false, IntPtr.Zero);

                    var memoryStream = new MemoryStream();

                    thumb.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                    return Task.FromResult(memoryStream);

                }, cancellationToken ?? CancellationToken.None);

                return outStream;
            }
            finally
            {
                if (fileStream is not null)
                {
                    await fileStream.DisposeAsync().AsTask();
                }
            }
        }
    }
}
