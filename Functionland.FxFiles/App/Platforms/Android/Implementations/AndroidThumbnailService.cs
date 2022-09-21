using Functionland.FxFiles.Shared.Utils;
using Path = System.IO.Path;
using Microsoft.Maui.Graphics.Platform;
using System.Reflection;
using Microsoft.Maui.Graphics;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace Functionland.FxFiles.App.Platforms.Android.Implementations
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
                IImage newImage = image.Downsize(150, true);
                using MemoryStream memStream = new MemoryStream();
                using var fileStream = new FileStream(thumbPath, FileMode.Create);
                newImage.Save(memStream);
                memStream.Seek(0, SeekOrigin.Begin);
                await memStream.CopyToAsync(fileStream); 
            }

            return thumbPath;
        }

        private string GetThumbnailFullPath(FsArtifact fsArtifact)
        {
            var imagePath = fsArtifact.FullPath;
            var lastModifiedDateTimeTicksStr = fsArtifact.LastModifiedDateTime.UtcTicks.ToString();
            var finalName = imagePath + lastModifiedDateTimeTicksStr;

            var imagePathHash = MakeHashData.ComputeSha256Hash(finalName);
            var destinationDirectory = Path.Combine(MauiApplication.Current.CacheDir.Path, "FxThumbFolder");

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            var thumbPath = Path.Combine(destinationDirectory, Path.ChangeExtension(imagePathHash, "Jpeg"));

            return thumbPath;
        }
    }
}
