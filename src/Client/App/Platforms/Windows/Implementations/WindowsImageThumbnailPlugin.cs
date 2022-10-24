using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Utils;
using System.Drawing;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations
{
    public class WindowsImageThumbnailPlugin : ImageThumbnailPlugin
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
                    var imageStream = stream ?? fileStream;
                    if (imageStream is null)
                        throw new InvalidOperationException("No stream available for the image.");

                    var image = System.Drawing.Image.FromStream(imageStream);
                    image = CorrectRotation(image);

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

        private static System.Drawing.Image? CorrectRotation(System.Drawing.Image? image)
        {
            if (image != null && Array.IndexOf(image.PropertyIdList, 274) > -1)
            {
                var orientationByte = image.GetPropertyItem(274)?.Value?[0];
                var orientation = orientationByte == null ? 0 : (int)orientationByte;

                switch (orientation)
                {
                    case 1:
                        // No rotation required.
                        break;
                    case 2:
                        image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
                image.RemovePropertyItem(274);
            }

            return image;
        }
    }
}
