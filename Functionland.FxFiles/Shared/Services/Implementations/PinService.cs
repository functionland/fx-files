using Functionland.FxFiles.Shared.Models;
using Functionland.FxFiles.Shared.Services.Contracts;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Services.Implementations
{
    public partial class PinService : IPinService
    {
        [AutoInject] IFxLocalDbService FxLocalDbService { get; set; } = default!;
        [AutoInject] IFileService FileService { get; set; } = default!;
        [AutoInject] public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;
        [AutoInject] public IEventAggregator EventAggregator { get; set; } = default!;

        public SubscriptionToken ArtifactChangeSubscription { get; set; }


        public List<PinnedArtifact> PinnedPathsCatche { get; set; } = new List<PinnedArtifact>();

        public async Task InitializeAsync()
        {
            var pinnedArtifacts = await FxLocalDbService.GetPinnedArticatInfos();
            if (pinnedArtifacts.Count > 0)
            {
                var pinnedArtifactPaths = pinnedArtifacts.Select(p => p.FullPath).ToList();
                var existPins = await FileService.CheckPathExistsAsync(pinnedArtifactPaths);
                foreach (var pin in existPins)
                {
                    if (pin.ArtifactFullPath == null) throw new DomainLogicException(StringLocalizer[nameof(AppStrings.PathIsNull)]);
                    if (pin.FsArtifactChangesType == FsArtifactChangesType.Delete)
                    {
                        await SetArtifactsUnPinAsync(new string[] { pin.ArtifactFullPath });
                    }
                    else
                    {
                        var pinnedArticat = pinnedArtifacts.Where(p => string.Equals(p.FullPath, pin.ArtifactFullPath, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                        if (pinnedArticat != null && DateTimeOffset.TryParse(pinnedArticat.ContentHash, out var LastModyDatetime))
                        {
                            var fileExtention = Path.GetExtension(pin.ArtifactFullPath);
                            if (!string.IsNullOrWhiteSpace(fileExtention) &&
                                LastModyDatetime != pin.LastModifiedDateTime &
                                ImageExtensions.Contains(fileExtention.ToUpperInvariant()))
                            {
                                //todo getThumbnail photo address
                                var edditedPinArtfact = new PinnedArtifact
                                {
                                    FullPath = pin.ArtifactFullPath,
                                    ContentHash = LastModyDatetime.ToString(),
                                    PinEpochTime = pinnedArticat.PinEpochTime,
                                    ProviderType = pinnedArticat.ProviderType,
                                    //ThumbnailPath todo: tofill
                                };
                                await UpdatePinnedArticatAsyn(edditedPinArtfact);


                            }
                        }
                    }
                }
            }

            ArtifactChangeSubscription = EventAggregator
                    .GetEvent<ArtifactChangeEvent>()
                    .SubscribeAsync(
                        HandleChangedArtifacts,
                        ThreadOption.UIThread, keepSubscriberReferenceAlive: true);
        }

        private async Task UpdatePinnedArticatAsyn(PinnedArtifact edditedPinArtfact)
        {
            if(edditedPinArtfact.FullPath == null) throw new DomainLogicException(StringLocalizer[nameof(AppStrings.PathIsNull)]);
            await FxLocalDbService.UpdatePinAsync(edditedPinArtfact);
            DeteteFromPinCache(edditedPinArtfact.FullPath);
            PinnedPathsCatche.Add(edditedPinArtfact);

        }

        private async Task HandleChangedArtifacts(ArtifactChangeEvent artifactChangeEvent)
        {
            if (artifactChangeEvent.FsArtifact == null) throw new DomainLogicException(StringLocalizer[nameof(AppStrings.ArtifactDoseNotExistsException)]);
            if (artifactChangeEvent.ChangeType == FsArtifactChangesType.Delete)
            {
                await SetArtifactsUnPinAsync(new string[] { artifactChangeEvent.FsArtifact.FullPath });
            }
            else if (artifactChangeEvent.ChangeType == FsArtifactChangesType.Modify)
            {
                if (artifactChangeEvent.FsArtifact.FileExtension != null && ImageExtensions.Contains(artifactChangeEvent.FsArtifact.FileExtension.ToUpperInvariant()))
                {
                    //todo://store thumbnail photo
                    //update pin cache
                }
            }
        }

        private void DeteteFromPinCache(string fullPath)
        {
            var item = PinnedPathsCatche.FirstOrDefault(p => string.Equals(p.FullPath, fullPath, StringComparison.CurrentCultureIgnoreCase));
            if (item != null)
                PinnedPathsCatche.Remove(item);

        }


        public async Task SetArtifactsPinAsync(FsArtifact[] artifacts, CancellationToken? cancellationToken = null)
        {
            foreach (var artifact in artifacts)
            {
                //todo://store thumbnail photo
                await FxLocalDbService.AddPinAsync(artifact);
                PinnedPathsCatche.Add(new PinnedArtifact
                {
                    FullPath = artifact.FullPath,
                    ContentHash = artifact.LastModifiedDateTime.ToString(),
                    PinEpochTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    ProviderType = artifact.ProviderType,
                    //ThumbnailPath
                });
            }
        }

        public async Task SetArtifactsUnPinAsync(string[] paths, CancellationToken? cancellationToken = null)
        {
            foreach (var path in paths)
            {
                await FxLocalDbService.RemovePinAsync(path);
                DeteteFromPinCache(path);
            }
        }
        public async IAsyncEnumerable<FsArtifact> GetPinnedArtifactsAsync(string? fullPath)
        {
            var pinnedArrtifact = PinnedPathsCatche;
            var artifacts = FileService.GetArtifactsAsync(fullPath);
            await foreach (var artifact in artifacts)
            {
                var pinnedArtifact = pinnedArrtifact.Where(p => string.Equals(p.FullPath, artifact.FullPath, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (pinnedArtifact is not null)
                {
                    artifact.IsPinned = true;
                    artifact.ThumbnailPath = pinnedArtifact.ThumbnailPath;
                    yield return artifact;
                }

            }

        }

        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPEG", ".JPE", ".BMP", ".GIF", ".PNG" };
    }
}
