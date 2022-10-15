using Functionland.FxFiles.Client.Shared.Extensions;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeShareService : IFulaShareService, ILocalDeviceShareService
{
    private readonly List<ArtifactPermissionInfo> _sharedByMeArtifactPermissionInfos = new();
    private readonly List<ArtifactPermissionInfo> _sharedWithMeArtifactPermissionInfos = new();
    public IFulaFileService FulaFileService { get; set; }
    public TimeSpan? ActionLatency { get; set; }
    public TimeSpan? EnumerationLatency { get; set; }
    public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

    public FakeShareService(IFulaFileService FulaFileService,
                            IEnumerable<ArtifactPermissionInfo>? sharedByMeArtifactPermissionInfos = null,
                            IEnumerable<ArtifactPermissionInfo>? sharedWithMeArtifactPermissionInfos = null,
                            TimeSpan? actionLatency = null,
                            TimeSpan? enumerationLatency = null)
    {
        this.FulaFileService = FulaFileService;
        _sharedByMeArtifactPermissionInfos.Clear();
        _sharedWithMeArtifactPermissionInfos.Clear();

        ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
        EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(10);

        if (sharedByMeArtifactPermissionInfos is not null)
        {
            foreach (var sharedByMeArtifact in sharedByMeArtifactPermissionInfos)
            {
                _sharedByMeArtifactPermissionInfos.Add(sharedByMeArtifact);
            }
        }
        else
        {
            _sharedByMeArtifactPermissionInfos = new List<ArtifactPermissionInfo>();
        }

        if (sharedWithMeArtifactPermissionInfos is not null)
        {
            foreach (var sharedWithMeArtifact in sharedWithMeArtifactPermissionInfos)
            {
                _sharedWithMeArtifactPermissionInfos.Add(sharedWithMeArtifact);
            }
        }
        else
        {
            _sharedWithMeArtifactPermissionInfos = new List<ArtifactPermissionInfo>();
        }
    }
    public Task InitAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task EnsureInitializedAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public async IAsyncEnumerable<FsArtifact> GetSharedByMeArtifactsAsync(CancellationToken? cancellationToken = null)
    {
        if (_sharedByMeArtifactPermissionInfos is null || !_sharedByMeArtifactPermissionInfos.Any()) yield break;

        foreach (var artifactPermissionInfo in _sharedByMeArtifactPermissionInfos)
        {
            if (EnumerationLatency is not null)
            {
                await Task.Delay(EnumerationLatency.Value);
            }

            var fsArtifacts = FulaFileService.GetArtifactAsync(artifactPermissionInfo.FullPath).GetAwaiter().GetResult();

            yield return fsArtifacts;

        }
    }

    public async IAsyncEnumerable<FsArtifact> GetSharedWithMeArtifactsAsync(CancellationToken? cancellationToken = null)
    {
        if (_sharedWithMeArtifactPermissionInfos is null || !_sharedByMeArtifactPermissionInfos.Any()) yield break;

        foreach (var artifactPermissionInfo in _sharedWithMeArtifactPermissionInfos)
        {
            if (EnumerationLatency is not null)
            {
                await Task.Delay(EnumerationLatency.Value);
            }

            var fsArtifacts = FulaFileService.GetArtifactAsync(artifactPermissionInfo.FullPath).GetAwaiter().GetResult();

            yield return fsArtifacts;

        }
    }

    public async Task<bool> IsSahredByMeAsync(string path, CancellationToken? cancellationToken = null)
    {
        if (ActionLatency != null)
        {
            await Task.Delay(ActionLatency.Value);
        }

        var artifact = _sharedByMeArtifactPermissionInfos?.FirstOrDefault(a => a.FullPath == path);

        if (artifact is not null)
            return true;

        return false;
    }

    public async Task<List<ArtifactPermissionInfo>> GetArtifactSharesAsync(string path, CancellationToken? cancellationToken = null)
    {
        if (ActionLatency != null)
        {
            await Task.Delay(ActionLatency.Value);
        }

        var lowerCaseArtifact = AppStrings.Artifact.ToLowerFirstChar();
        var artifact = FulaFileService.GetArtifactsAsync(path);

        if (artifact is null)
            throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, lowerCaseArtifact));


        var permissionedUsers = _sharedByMeArtifactPermissionInfos?.Where(a => a.FullPath == path).ToList();
        return permissionedUsers ?? new List<ArtifactPermissionInfo>();
    }

    public async Task SetPermissionArtifactsAsync(IEnumerable<ArtifactPermissionInfo> permissionInfos, CancellationToken? cancellationToken = null)
    {

        foreach (var permissionInfo in permissionInfos)
        {
            var sharedItem = _sharedByMeArtifactPermissionInfos?
                .Where(c => c.FullPath == permissionInfo.FullPath && c.DId == permissionInfo.DId)
                .FirstOrDefault();

            if (sharedItem == null)
            {
                if (permissionInfo.PermissionLevel == ArtifactPermissionLevel.None)
                    throw new HasNotBeenSharedException(StringLocalizer.GetString(AppStrings.HasNotBeenSharedException));

                sharedItem = new ArtifactPermissionInfo
                {
                    DId = permissionInfo.DId,
                    FullPath = permissionInfo.FullPath,
                    PermissionLevel = permissionInfo.PermissionLevel
                };

                _sharedByMeArtifactPermissionInfos?.Add(sharedItem);
            }
            else
            {
                if (permissionInfo.PermissionLevel == ArtifactPermissionLevel.None)
                {
                    _sharedByMeArtifactPermissionInfos?.Remove(sharedItem);
                }
                else
                {
                    sharedItem.PermissionLevel = permissionInfo.PermissionLevel;
                }
            }

            var artifact = FulaFileService.GetArtifactAsync(permissionInfo.FullPath).GetAwaiter().GetResult();

            artifact.PermissionedUsers = await GetArtifactSharesAsync(permissionInfo.FullPath, cancellationToken);
        }
    }
}
