using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class ArtifactThumbnailService<TFileService> : ThumbnailService, IArtifactThumbnailService<TFileService>
    where TFileService : IFileService
{
    TFileService FileServcie { get; set; }
    public ArtifactThumbnailService(
        IFileCacheService fileCacheService,
        IEnumerable<IThumbnailPlugin> thumbnailPlugins,
        TFileService fileService) : base(fileCacheService, thumbnailPlugins)
    {
        FileServcie = fileService;
    }

    public async Task<string?> GetOrCreateThumbnailAsync(FsArtifact artifact, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null)
    {
        if (artifact.ProviderType == FsFileProviderType.Fula && artifact.IsAvailableOfflineRequested != true) return null;

        var uniqueName = GetUniqueName(artifact, thumbnailScale);

        var getStreamFunc = async () => await FileServcie.GetFileContentAsync(artifact.FullPath, cancellationToken);

        return await GetOrCreateThumbnailAsync(
            CacheCategoryType.Artifact,
            thumbnailScale,
            uniqueName,
            getStreamFunc,
            artifact.LocalFullPath,
            cancellationToken);
    }

    private static string GetUniqueName(FsArtifact fsArtifact, ThumbnailScale thumbnailScale)
    {
        var imagePath = fsArtifact.FullPath;
        var extension = Path.GetExtension(imagePath);
        var lastModifiedDateTimeTicksStr = fsArtifact.LastModifiedDateTime.UtcTicks.ToString();
        var fullPath = Path.Combine(
            Path.GetDirectoryName(imagePath) ?? string.Empty,
            Path.GetFileName(imagePath) + lastModifiedDateTimeTicksStr + thumbnailScale,
            extension
            );

        var fullPathHash = MakeHashData.ComputeSha256Hash(fullPath);
        return Path.ChangeExtension(fullPathHash, extension);
    }
}
