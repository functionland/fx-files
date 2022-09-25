using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Resources;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations
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

        public override string GetAppCacheDirectory()
        {
            return FileSystem.CacheDirectory;
        }
    }
}
