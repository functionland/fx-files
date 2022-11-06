using Functionland.FxFiles.Client.Shared.Enums;
using System.Drawing;
using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Resources;
using Functionland.FxFiles.Client.Shared.Utils;
using AppKit;
using Foundation;

namespace Functionland.FxFiles.Client.App.Platforms.MacCatalyst.Implementations
{
    public class MacImageThumbnailPlugin : ImageThumbnailPlugin
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
                var imageStream = stream ?? fileStream;
                if (imageStream is null)
                    throw new InvalidOperationException("No stream available for the image.");

                var imageStreamDate = NSData.FromStream(imageStream);
                var image = new NSImage(imageStreamDate);


                //  var image = System.Drawing.Image.FromStream(imageStream);
                // image = CorrectRotation(image);

                (int imageWidth, int imageHeight) = ImageUtils.ScaleImage((int)image.Size.Width, (int)image.Size.Height, thumbnailScale);

                var thumb = new NSImage(new CoreGraphics.CGSize(imageWidth, imageHeight));

                thumb.LockFocus();
                image.Draw(new CoreGraphics.CGRect(0, 0, imageWidth, imageHeight));
                thumb.UnlockFocus();

                var thumbData = thumb.AsTiff();

                return thumbData.AsStream();


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
