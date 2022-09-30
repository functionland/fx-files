using Functionland.FxFiles.Client.Shared.Models;
using Prism.Events;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public partial class PinService : IPinService
    {
        [AutoInject] IFxLocalDbService FxLocalDbService { get; set; } = default!;
        [AutoInject] IFileService FileService { get; set; } = default!;
        [AutoInject] public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;
        [AutoInject] public IEventAggregator EventAggregator { get; set; } = default!;
        [AutoInject] public IThumbnailService ThumbnailService { get; set; } = default!;
        [AutoInject] public IFileWatchService FileWatchService { get; set; } = default!;
        [AutoInject] public IExceptionHandler ExceptionHandler { get; set; } = default!;
        public SubscriptionToken ArtifactChangeSubscription { get; set; }
        public ConcurrentDictionary<string, PinnedArtifact> PinnedPathsCatche { get; set; } = new();

        public async Task InitializeAsync()
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

                    if (pinnedArticat != null && pinnedArticat.ProviderType != FsFileProviderType.Blox && DateTimeOffset.TryParse(pinnedArticat.ContentHash, out var LastModyDatetime))
                    {
                        if (changedPinnedArtifact.LastModifiedDateTime > LastModyDatetime)
                        {
                            var artifact = GetPinnedFsArtifact(pinnedArticat);
                            if (ArtifactIsImage(artifact))
                            {
                                var thumbnailAddress = await ThumbnailService.MakeThumbnailAsync(artifact);
                                artifact.ThumbnailPath = thumbnailAddress;
                            }

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
                            PinnedPathsCatche.TryAdd(pinnedArticat.FullPath, pinnedArticat);
                        }

                    }
                    else
                    {
                        PinnedPathsCatche.TryAdd(pinnedArticat.FullPath, pinnedArticat);
                    }
                    WatchParnetFolder(GetPinnedFsArtifact(pinnedArticat));
                }
            }
        }

        private async Task UpdatePinnedArtifactAsyn(PinnedArtifact edditedPinArtfact)
        {
            if (edditedPinArtfact.FullPath == null) throw new ArtifactPathNullException(StringLocalizer[nameof(AppStrings.PathIsNull)]);
            await FxLocalDbService.UpdatePinAsync(edditedPinArtfact);
            DeteteFromPinCache(edditedPinArtfact.FullPath);
            PinnedPathsCatche.TryAdd(edditedPinArtfact.FullPath, edditedPinArtfact);

        }

        private async void HandleChangedArtifacts(ArtifactChangeEvent artifactChangeEvent)
        {
            try
            {
                if (artifactChangeEvent.FsArtifact == null) throw new ArtifactPathNullException(StringLocalizer[nameof(AppStrings.ArtifactDoseNotExistsException)]);

                if (artifactChangeEvent.ChangeType == FsArtifactChangesType.Delete)
                {
                    await SetArtifactsUnPinAsync(new string[] { artifactChangeEvent.FsArtifact.FullPath });
                }
                else if (artifactChangeEvent.ChangeType == FsArtifactChangesType.Modify)
                {
                    var editedArtifact = new PinnedArtifact
                    {
                        ProviderType = artifactChangeEvent.FsArtifact.ProviderType,
                        FullPath = artifactChangeEvent.FsArtifact.FullPath,
                        FsArtifactType = artifactChangeEvent.FsArtifact.ArtifactType,
                        ArtifactName = artifactChangeEvent.FsArtifact.Name,
                        ContentHash = artifactChangeEvent.FsArtifact.ProviderType == FsFileProviderType.Blox ? artifactChangeEvent.FsArtifact.ContentHash : artifactChangeEvent.FsArtifact.LastModifiedDateTime.ToString()
                    };

                    if (ArtifactIsImage(artifactChangeEvent.FsArtifact))
                    {
                        var thumbnailAddress = await ThumbnailService.MakeThumbnailAsync(artifactChangeEvent.FsArtifact);
                        editedArtifact.ThumbnailPath = thumbnailAddress;
                        artifactChangeEvent.FsArtifact.ThumbnailPath = thumbnailAddress;

                    }
                    await FxLocalDbService.UpdatePinAsync(editedArtifact, artifactChangeEvent.Description);
                    DeteteFromPinCache(artifactChangeEvent.Description != null ? artifactChangeEvent.Description : editedArtifact.FullPath);
                    PinnedPathsCatche.TryAdd(editedArtifact.FullPath, editedArtifact);

                    if (artifactChangeEvent.Description != null)
                        UnWatchParent(new FsArtifact(artifactChangeEvent.Description, Path.GetFileName(artifactChangeEvent.Description), (FsArtifactType)editedArtifact.FsArtifactType, (FsFileProviderType)editedArtifact.ProviderType));

                    WatchParnetFolder(artifactChangeEvent.FsArtifact);
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
            PinnedPathsCatche.TryGetValue(fullPath, out var item);
            if (item != null)
                PinnedPathsCatche.TryRemove(fullPath, out _);

        }


        public async Task SetArtifactsPinAsync(FsArtifact[] artifacts, CancellationToken? cancellationToken = null)
        {
            foreach (var artifact in artifacts)
            {

                if (PinnedPathsCatche.Any(p => string.Equals(p.Key, artifact.FullPath, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return;
                }
                if (ArtifactIsImage(artifact))
                {
                    var thumbnailAddress = await ThumbnailService.MakeThumbnailAsync(artifact);
                    artifact.ThumbnailPath = thumbnailAddress;

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
                PinnedPathsCatche.TryAdd(artifact.FullPath, newPinnedArtifact);
                WatchParnetFolder(artifact);
            }
        }

        public async Task SetArtifactsUnPinAsync(string[] paths, CancellationToken? cancellationToken = null)
        {
            foreach (var path in paths)
            {
                await FxLocalDbService.RemovePinAsync(path);
                DeteteFromPinCache(path);
                UnWatchParent(new FsArtifact(path, Path.GetFileName(path), FsArtifactType.File, FsFileProviderType.InternalMemory));
            }
        }


        public async Task<List<FsArtifact>> GetPinnedArtifactsAsync()
        {
            var pinnedArtifacts = PinnedPathsCatche.Select(c => c.Value).ToList();

            var artifacts = new List<FsArtifact>();
            foreach (var artifact in pinnedArtifacts)
            {
                FsArtifact currentFile = null;
                if (artifact.FsArtifactType == FsArtifactType.File)
                {

                    var files = FileService.GetArtifactsAsync(artifact.FullPath);
                    await foreach (var file in files)
                        currentFile = file;

                }
                var fsArtifact = currentFile == null ? GetPinnedFsArtifact(artifact) : currentFile;

                if (ArtifactIsImage(fsArtifact) && artifact.ThumbnailPath != null)
                {

                    var result = (await FileService.CheckPathExistsAsync(new List<string?> { artifact.ThumbnailPath })).FirstOrDefault();
                    if (result != null && result.IsPathExist == false)
                    {

                        await CreateNewThumbnailAsync(artifact, fsArtifact);
                        await FxLocalDbService.UpdatePinAsync(artifact);
                    }
                }
                artifacts.Add(fsArtifact);
            }
            return artifacts;
        }

        private async Task CreateNewThumbnailAsync(PinnedArtifact artifact, FsArtifact fsArtifact)
        {
            var newThumbnailPath = await ThumbnailService.MakeThumbnailAsync(fsArtifact);
            artifact.ThumbnailPath = newThumbnailPath;

        }

        private FsArtifact GetPinnedFsArtifact(PinnedArtifact artifact)
        {
            if (artifact.FullPath == null || artifact.ArtifactName == null || artifact.FsArtifactType == null || artifact.ProviderType == null)
            {
                throw new ArtifactDoseNotExistsException(StringLocalizer[nameof(AppStrings.ArtifactDoseNotExistsException)]);
            }
            var fsArtifact = new FsArtifact(artifact.FullPath, artifact.ArtifactName, artifact.FsArtifactType.Value, artifact.ProviderType.Value) { ThumbnailPath = artifact.ThumbnailPath };
            return fsArtifact;
        }
        private bool ArtifactIsImage(FsArtifact fsArtifact)
        {
            fsArtifact.FileExtension ??= Path.GetExtension(fsArtifact.FullPath);
            if (fsArtifact.FileExtension != null && ImageExtensions.Contains(fsArtifact.FileExtension.ToUpperInvariant()))
                return true;
            return false;
        }
        public bool IsPinned(FsArtifact fsArtifact)
        {
            if (PinnedPathsCatche.Any(c => string.Equals(c.Key, fsArtifact.FullPath, StringComparison.CurrentCultureIgnoreCase)))
                return true;
            return false;
        }

        public static readonly List<string> ImageExtensions = new() { ".JPG", ".JPEG", ".JPE", ".BMP", ".GIF", ".PNG" };
    }
}
