using Functionland.FxFiles.Shared.Utils;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Functionland.FxFiles.App.Platforms.Windows.Implementations
{
    public partial class WindowsThumbnailService : LocalThumbnailService
    {
        [AutoInject] public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

        public override async Task<string> MakeThumbnailAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
        {
            var thumbPath = GetThumbnailFullPath(fsArtifact);

            if (File.Exists(thumbPath)) return thumbPath;

            const int thumbnailSize = 150;
            var image = System.Drawing.Image.FromFile(fsArtifact.FullPath);

            var imageHeight = image.Height;
            var imageWidth = image.Width;
            if (imageHeight > imageWidth)
            {
                imageWidth = (int)(((float)imageWidth / (float)imageHeight) * thumbnailSize);
                imageHeight = thumbnailSize;
            }
            else
            {
                imageHeight = (int)(((float)imageHeight / (float)imageWidth) * thumbnailSize);
                imageWidth = thumbnailSize;
            }

            var thumb = image.GetThumbnailImage(imageWidth, imageHeight, () => false, IntPtr.Zero);
            thumb.Save(thumbPath);

            return thumbPath;
        }        

        private string GetThumbnailFullPath(FsArtifact fsArtifact)
        {
            var imagePath = fsArtifact.FullPath;
            var lastModifiedDateTimeTicksStr = fsArtifact.LastModifiedDateTime.UtcTicks.ToString();
            var finalName = imagePath + lastModifiedDateTimeTicksStr;

            var imagePathHash = MakeHashData.ComputeSha256Hash(finalName);
            var destinationDirectory = Path.Combine(FileSystem.CacheDirectory, "FxThumbFolder");

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            var thumbPath = Path.Combine(destinationDirectory, Path.ChangeExtension(imagePathHash, "Jpeg"));

            return thumbPath;
        }
    }
}
