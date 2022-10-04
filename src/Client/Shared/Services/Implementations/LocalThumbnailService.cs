using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class LocalThumbnailService : IThumbnailService
{
    public abstract Task<string> MakeThumbnailAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null);

    public abstract string GetAppCacheDirectory();

    public virtual string GetThumbnailFullPath(FsArtifact fsArtifact)
    {
        var imagePath = fsArtifact.FullPath;
        var lastModifiedDateTimeTicksStr = fsArtifact.LastModifiedDateTime.UtcTicks.ToString();
        var finalName = imagePath + lastModifiedDateTimeTicksStr;

        var imagePathHash = MakeHashData.ComputeSha256Hash(finalName);
        var destinationDirectory = Path.Combine(GetAppCacheDirectory(), "FxThumbFolder");

        if (!Directory.Exists(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory);
        }

        var thumbPath = Path.Combine(destinationDirectory, Path.ChangeExtension(imagePathHash, "Jpeg"));

        return thumbPath;
    }       
}
