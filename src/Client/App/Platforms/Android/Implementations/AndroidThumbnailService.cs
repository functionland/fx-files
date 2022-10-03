using Functionland.FxFiles.Client.Shared.Models;
using Microsoft.Maui.Graphics.Platform;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations
{
    public class AndroidThumbnailService : LocalThumbnailService
    {
        public override async Task<string> MakeThumbnailAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
        {
            var thumbPath = GetThumbnailFullPath(fsArtifact);

            if (File.Exists(thumbPath)) return thumbPath;

            IImage image;
            using var streamReader = new StreamReader(fsArtifact.FullPath);

            using var stream = streamReader.BaseStream;
            image = PlatformImage.FromStream(stream);

            if (image != null)
            {
                IImage newImage = image.Downsize(210, 114, true);
                using MemoryStream memStream = new MemoryStream();
                using var fileStream = new FileStream(thumbPath, FileMode.Create);
                newImage.Save(memStream);
                memStream.Seek(0, SeekOrigin.Begin);
                await memStream.CopyToAsync(fileStream);
            }

            return thumbPath;
        }

        public override string GetAppCacheDirectory()
        {
            return MauiApplication.Current.CacheDir.Path;
        }
    }
}
