using Functionland.FxFiles.Client.Shared.Services.Common;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Prism.Events;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class LocalDevicePinService : ILocalDevicePinService
{
    [AutoInject] IFxLocalDbService FxLocalDbService { get; set; } = default!;
    [AutoInject] ILocalDeviceFileService FileService { get; set; } = default!;
    [AutoInject] public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;
    [AutoInject] public IEventAggregator EventAggregator { get; set; } = default!;
    [AutoInject] public IArtifactThumbnailService<ILocalDeviceFileService> ArtifactThumbnailService { get; set; } = default!;
    [AutoInject] public IFileWatchService FileWatchService { get; set; } = default!;
    [AutoInject] public IExceptionHandler ExceptionHandler { get; set; } = default!;
    public SubscriptionToken ArtifactChangeSubscription { get; set; }
    public ConcurrentDictionary<string, PinnedArtifact> PinnedPathsCache { get; set; } = new();

    private TaskCompletionSource? _tcs { get; set; }

    public async Task InitializeAsync(CancellationToken? cancellationToken = null)
    {
        if (PinnedPathsCache.Any()) return;
        _tcs ??= new TaskCompletionSource();

        try
        {
            ArtifactChangeSubscription = EventAggregator
                           .GetEvent<ArtifactChangeEvent>()
                           .Subscribe(
                               HandleChangedArtifacts,
                               ThreadOption.BackgroundThread, keepSubscriberReferenceAlive: true);

            var pinnedArtifacts = await FxLocalDbService.GetPinnedArticatInfos();
            if (pinnedArtifacts.Count == 0) return;

            var pinnedArtifactPaths = pinnedArtifacts.Select(p => p.FullPath).ToList();
            var existPins = await FileService.CheckPathExistsAsync(pinnedArtifactPaths);

            foreach (var changedPinnedArtifact in existPins)
            {
                if (changedPinnedArtifact.ArtifactFullPath == null) throw new ArtifactPathNullException(StringLocalizer[nameof(AppStrings.PathIsNull)]);

                if (changedPinnedArtifact.FsArtifactChangesType == FsArtifactChangesType.Delete)
                {
                    await SetArtifactsUnPinAsync(new string[] { changedPinnedArtifact.ArtifactFullPath });
                }
                else if (changedPinnedArtifact.IsPathExist == true)
                {
                    var pinnedArticat = pinnedArtifacts
                        .Where(p => string.Equals(p.FullPath, changedPinnedArtifact.ArtifactFullPath, StringComparison.CurrentCultureIgnoreCase))
                        .FirstOrDefault();

                    if (pinnedArticat != null && pinnedArticat.ProviderType != FsFileProviderType.Fula && DateTimeOffset.TryParse(pinnedArticat.ContentHash, out var LastModyDatetime))
                    {
                        if (changedPinnedArtifact.LastModifiedDateTime > LastModyDatetime)
                        {
                            var artifact = await GetPinnedFsArtifact(pinnedArticat);

                            var edditedPinArtfact = new PinnedArtifact
                            {
                                ArtifactName = pinnedArticat.ArtifactName,
                                FsArtifactType = pinnedArticat.FsArtifactType,
                                FullPath = changedPinnedArtifact.ArtifactFullPath,
                                ContentHash = changedPinnedArtifact.LastModifiedDateTime.ToString(),
                                PinEpochTime = pinnedArticat.PinEpochTime,
                                ProviderType = pinnedArticat.ProviderType,
                                ThumbnailPath = artifact.ThumbnailPath
                            };
                            await UpdatePinnedArtifactAsyn(edditedPinArtfact);
                        }
                        else
                        {
                            PinnedPathsCache.TryAdd(pinnedArticat.FullPath, pinnedArticat);
                        }

                    }
                    else
                    {
                        PinnedPathsCache.TryAdd(pinnedArticat.FullPath, pinnedArticat);
                    }
                    WatchParnetFolder(await GetPinnedFsArtifact(pinnedArticat));
                }
            }
        }
        finally
        {
            _tcs.SetResult();
        }

    }

    public async Task SetArtifactsPinAsync(IEnumerable<FsArtifact> artifacts, CancellationToken? cancellationToken = null)
    {
        foreach (var artifact in artifacts)
        {
            if (PinnedPathsCache.Any(p => string.Equals(p.Key, artifact.FullPath, StringComparison.CurrentCultureIgnoreCase)))
            {
                return;
            }
           
            await FxLocalDbService.AddPinAsync(artifact);

            var newPinnedArtifact = new PinnedArtifact
            {
                FullPath = artifact.FullPath,
                ContentHash = artifact.LastModifiedDateTime.ToString(),
                PinEpochTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                ProviderType = artifact.ProviderType,
                ThumbnailPath = artifact.ThumbnailPath,
                ArtifactName = artifact.Name,
                FsArtifactType = artifact.ArtifactType

            };
            PinnedPathsCache.TryAdd(artifact.FullPath, newPinnedArtifact);
            WatchParnetFolder(artifact);
        }
    }

    public async Task SetArtifactsUnPinAsync(IEnumerable<string> paths, CancellationToken? cancellationToken = null)
    {
        foreach (var path in paths)
        {
            await FxLocalDbService.RemovePinAsync(path);
            DeteteFromPinCache(path);
            UnWatchParent(new FsArtifact(path, Path.GetFileName(path), FsArtifactType.File, FsFileProviderType.InternalMemory));
        }
    }

    public async Task<List<FsArtifact>> GetPinnedArtifactsAsync(CancellationToken? cancellationToken = null)
    {
        await EnsureInitializedAsync();
        var pinnedArtifacts = PinnedPathsCache.Select(c => c.Value).ToList();

        var artifacts = new List<FsArtifact>();
        foreach (var pinnedArtifact in pinnedArtifacts)
        {
            var fsArtifact =  await GetPinnedFsArtifact(pinnedArtifact);

            artifacts.Add(fsArtifact);
        }
        return artifacts;
    }

    public async Task<bool> IsPinnedAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
    {
        await EnsureInitializedAsync();
        if (PinnedPathsCache.Any(c => string.Equals(c.Key, fsArtifact.FullPath, StringComparison.CurrentCultureIgnoreCase)))
            return true;
        return false;
    }

    public async Task EnsureInitializedAsync(CancellationToken? cancellationToken = null)
    {
        if (_tcs == null) return;
        await _tcs.Task;
    }

    public static readonly List<string> ImageExtensions = new() { ".JPG", ".JPEG", ".JPE", ".BMP", ".GIF", ".PNG" };

    private async Task UpdatePinnedArtifactAsyn(PinnedArtifact edditedPinArtfact)
    {
        if (edditedPinArtfact.FullPath == null) throw new ArtifactPathNullException(StringLocalizer[nameof(AppStrings.PathIsNull)]);
        await FxLocalDbService.UpdatePinAsync(edditedPinArtfact);
        DeteteFromPinCache(edditedPinArtfact.FullPath);
        PinnedPathsCache.TryAdd(edditedPinArtfact.FullPath, edditedPinArtfact);

    }

    private async void HandleChangedArtifacts(ArtifactChangeEvent artifactChangeEvent)
    {
        try
        {
            if (artifactChangeEvent.FsArtifact == null) throw new ArtifactPathNullException(StringLocalizer[nameof(AppStrings.ArtifactDoseNotExistsException)]);

            if (!PinnedPathsCache.ContainsKey(artifactChangeEvent.FsArtifact.FullPath) && artifactChangeEvent.Description == null) return;
            if (artifactChangeEvent.Description != null && !PinnedPathsCache.ContainsKey(artifactChangeEvent.Description))  return;

            if (artifactChangeEvent.ChangeType == FsArtifactChangesType.Delete)
            {
                await SetArtifactsUnPinAsync(new string[] { artifactChangeEvent.FsArtifact.FullPath });
            }
            else if (artifactChangeEvent.ChangeType == FsArtifactChangesType.Modify || artifactChangeEvent.ChangeType == FsArtifactChangesType.Rename)
            {
                var editedArtifact = new PinnedArtifact
                {
                    ProviderType = artifactChangeEvent.FsArtifact.ProviderType,
                    FullPath = artifactChangeEvent.FsArtifact.FullPath,
                    FsArtifactType = artifactChangeEvent.FsArtifact.ArtifactType,
                    ArtifactName = artifactChangeEvent.FsArtifact.Name,
                    ContentHash = artifactChangeEvent.FsArtifact.LastModifiedDateTime.ToString()
                };

                await FxLocalDbService.UpdatePinAsync(editedArtifact, artifactChangeEvent.Description);
                DeteteFromPinCache(artifactChangeEvent.Description != null ? artifactChangeEvent.Description : editedArtifact.FullPath);
                PinnedPathsCache.TryAdd(editedArtifact.FullPath, editedArtifact);
   
            }
        }
        catch (Exception exp)
        {
            ExceptionHandler.Handle(exp);
        }
    }

    private void WatchParnetFolder(FsArtifact artifact)
    {
        string? parentPath = GetParentPath(artifact);
        var fileName = Path.GetFileName(parentPath);

        FileWatchService.WatchArtifact(new FsArtifact(parentPath, fileName, FsArtifactType.Folder, providerType: (FsFileProviderType)artifact.ProviderType));
    }

    private string? GetParentPath(FsArtifact artifact)
    {
        if (artifact.FullPath is null)
            throw new ArtifactPathNullException(StringLocalizer[nameof(AppStrings.ArtifactPathIsNull)]);
        var path = artifact.FullPath?.TrimEnd(Path.DirectorySeparatorChar);

        var parentPath = Directory.GetParent(path)?.FullName;
        return parentPath;
    }

    private void UnWatchParent(FsArtifact artifact)
    {
        string? parentPath = GetParentPath(artifact);
        var fileName = Path.GetFileName(parentPath);

        FileWatchService.UnWatchArtifact(new FsArtifact(parentPath, fileName, FsArtifactType.Folder, providerType: (FsFileProviderType)artifact.ProviderType));

    }

    private void DeteteFromPinCache(string fullPath)
    {
        PinnedPathsCache.TryGetValue(fullPath, out var item);
        if (item != null)
            PinnedPathsCache.TryRemove(fullPath, out _);

    }

    private async Task<FsArtifact> GetPinnedFsArtifact(PinnedArtifact pinnedArtifact)
    {
        if (pinnedArtifact.FullPath == null || pinnedArtifact.ArtifactName == null || pinnedArtifact.FsArtifactType == null || pinnedArtifact.ProviderType == null)
        {
            throw new ArtifactDoseNotExistsException(StringLocalizer[nameof(AppStrings.ArtifactDoseNotExistsException)]);
        }

        var fsArtifact = await FileService.GetArtifactAsync(pinnedArtifact.FullPath);
        fsArtifact.IsPinned = true;
        return fsArtifact;
    }

}
