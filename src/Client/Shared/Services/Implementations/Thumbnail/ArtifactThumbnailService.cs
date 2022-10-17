using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class ArtifactThumbnailService<TFileService> : ThumbnailService
    where TFileService : IFileService
{
    TFileService FileServcie { get; set; }
    public ArtifactThumbnailService(
        IFileCacheService fileCacheService,
        IThumbnailPlugin[] thumbnailPlugins,
        TFileService fileService
        )
        : base(fileCacheService, thumbnailPlugins)
    {
        FileServcie = fileService;
    }

    protected async Task<string?> GetOrCreateThumbnailAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
    {
        var uniqueName = GetUniqueName(artifact);
        return await GetOrCreateThumbnailAsync("Artifacts", uniqueName, async () => await FileServcie.GetFileContentAsync(artifact.FullPath));
    }

    private string GetUniqueName(FsArtifact fsArtifact)
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
