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
        public List<string> PinnedPaths { get; set; }

        public PinService(List<string> pinnedPaths)
        {
            PinnedPaths = pinnedPaths;

        }

        public async Task InitializeAsync()
        {
            var pinnedArrtifact = await FxLocalDbService.GetPinnedArticatInfos();
            //todo: check All pinned data changes
            //todo:register on changed event
        }

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
            var pinnedArrtifact = PinnedPaths;
            var artifacts =  FileService.GetArtifactsAsync(fullPath);
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
