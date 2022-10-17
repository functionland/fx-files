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

    public async Task<string?> GetOrCreateThumbnailAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
    {
        var uniqueName = GetUniqueName(artifact);
        return await GetOrCreateThumbnailAsync(CacheCategoryType.Artifact, uniqueName,
            async () => await FileServcie.GetFileContentAsync(artifact.FullPath, cancellationToken), cancellationToken);
    }

    private static string GetUniqueName(FsArtifact fsArtifact)
    {
        var imagePath = fsArtifact.FullPath;
        var extension = Path.GetExtension(imagePath);
        var lastModifiedDateTimeTicksStr = fsArtifact.LastModifiedDateTime.UtcTicks.ToString();
        var fullPath = Path.Combine(
            Path.GetDirectoryName(imagePath) ?? string.Empty,
            Path.GetFileName(imagePath) + lastModifiedDateTimeTicksStr,
            extension
            );

        var fullPathHash = MakeHashData.ComputeSha256Hash(fullPath);
        return $"{fullPathHash}.{extension}";
    }
}
