using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations
{
    public class WindowsImageThumbnailPlugin : ImageThumbnailPlugin
    {
        protected override async Task<Stream> OnCreateThumbnailAsync(Stream input, CancellationToken? cancellationToken = null)
        {
            return await Task.Run(async () =>
            {
                var image = System.Drawing.Image.FromStream(input);
                image = CorrectRotation(image);

                (int imageWidth, int imageHeight) = ImageUtils.ScaleImage(image.Width, image.Height, 252, 146);

                var thumb = image.GetThumbnailImage(imageWidth, imageHeight, () => false, IntPtr.Zero);
                var memoryStream = new MemoryStream();

                thumb.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                return memoryStream;

            }, cancellationToken ?? CancellationToken.None);
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
