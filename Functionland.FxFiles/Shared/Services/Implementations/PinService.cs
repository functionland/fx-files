using Functionland.FxFiles.Shared.Models;
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
        [AutoInject] IFileWatchService FileWatchService { get; set; } = default!;
        public SubscriptionToken ArtifactChangeSubscription { get; set; }

        public List<string> PinnedPathsCatche { get; set; }

        public PinService(List<string> pinnedPathsCache)
        {
            PinnedPathsCatche = pinnedPathsCache;

        }

        public async Task InitializeAsync()
        {
            var pinnedArtifact = await FxLocalDbService.GetPinnedArticatInfos();

            //todo: check All pinned data changes
            //todo:register on changed event

            ArtifactChangeSubscription = EventAggregator
                    .GetEvent<ArtifactChangeEventArgs>()
                    .SubscribeAsync(
                        HandleChangedArtifacts,
                        ThreadOption.UIThread, keepSubscriberReferenceAlive: true);
        }
        private async Task HandleChangedArtifacts(ArtifactChangeEventArgs a)
        {
            if (a.ChangeType == FsArtifactChangesType.Delete)
            {
                await FxLocalDbService.RemovePinAsync(a.FsArtifact.FullPath);
                DeteteFromPinCache(a.FsArtifact.FullPath);
            }
            else if (a.ChangeType == FsArtifactChangesType.Modify)
            {
                if (ImageExtensions.Contains(a.FsArtifact.FileExtension.ToUpperInvariant()))
                {
                    //todo://store thumbnail photo
                    //update pin cache
                }
            }
        }

        private void DeteteFromPinCache(string fullPath)
        {
            if (PinnedPathsCatche.Any(p => string.Equals(p, fullPath, StringComparison.CurrentCultureIgnoreCase)))
            {
                PinnedPathsCatche.Remove(fullPath);
            }
        }

        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPEG", ".JPE", ".BMP", ".GIF", ".PNG" };
        public async Task SetArtifactPinAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
        {
            //todo://store thumbnail photo
            await FxLocalDbService.AddPinAsync(artifact);
        }

        public async Task SetArtifactUnPinAsync(string path, CancellationToken? cancellationToken = null)
        {
            await FxLocalDbService.RemovePinAsync(path);
        }
        public async IAsyncEnumerable<FsArtifact> GetPinnedArtifactsAsync(string fullPath)
        {
            var pinnedArrtifact = PinnedPathsCatche;
            var artifacts = FileService.GetArtifactsAsync(fullPath);
            await foreach (var artifact in artifacts)
            {
                var isPinned = pinnedArrtifact.Any(p => string.Equals(p, artifact.FullPath, StringComparison.CurrentCultureIgnoreCase));
                if (isPinned)
                {
                    artifact.IsPinned = true;
                    yield return artifact;
                }

            }

        }
    }
}
