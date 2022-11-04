namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class AvatarThumbnailService : ThumbnailService, IAvatarThumbnailService
{
    IIdentityService IdentityService { get; set; }
    public AvatarThumbnailService(
        IFileCacheService fileCacheService,
        IEnumerable<IThumbnailPlugin> thumbnailPlugins,
        IIdentityService identityService) : base(fileCacheService, thumbnailPlugins)
    {
        IdentityService = identityService;
    }

    public Task<string?> GetOrCreateThumbnailAsync(string dId, CancellationToken? cancellationToken = null)
    {
        var uniqueKey = dId;
        return GetOrCreateThumbnailAsync(CacheCategoryType.FulaAvatars, ThumbnailScale.Small, uniqueKey, async () => await IdentityService.GetAvatarAsync(dId, cancellationToken), null, cancellationToken);
    }

    protected override IThumbnailPlugin? GetRelatedPlugin(string uri)
    {
        return ThumbnailPlugins.FirstOrDefault(plugin => plugin.IsSupported("jpg"));
    }

}
