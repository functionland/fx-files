using Functionland.FxFiles.Client.Shared.Models;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.OfflineAvailability
{
    public class FakeOfflineAvailabilityService : IOfflineAvailabilityService
    {
        private readonly List<FsArtifact> _FsArtifacts;
        private readonly List<FsArtifact> _AllFulaFsArtifacts;
        public TimeSpan? ActionLatency { get; set; }
        public TimeSpan? EnumerationLatency { get; set; }

        public FakeOfflineAvailabilityService(IEnumerable<FsArtifact> fsArtifacts, IEnumerable<FsArtifact> allFulaFsArtifacts, TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            _FsArtifacts = fsArtifacts.ToList();
            _AllFulaFsArtifacts= allFulaFsArtifacts.ToList();

            ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
            EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(10);
        }
        public FakeOfflineAvailabilityService(IEnumerable<FsArtifact> fsArtifacts, TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            _FsArtifacts = fsArtifacts.ToList();

            ActionLatency = actionLatency ?? TimeSpan.FromSeconds(2);
            EnumerationLatency = enumerationLatency ?? TimeSpan.FromMilliseconds(10);
        }
        public async Task InitAsync(CancellationToken? cancellationToken = null)
        {

        }
        
        public async Task EnsureInitializedAsync(CancellationToken? cancellationToken = null)
        {

        }

        public async Task MakeAvailableOfflineAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
        {
            _FsArtifacts.Add(artifact);
        }

        public async Task RemoveAvailableOfflineAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
        {
            if (artifact is null) return; //TODO

            _FsArtifacts.Remove(artifact);
        }

        public async Task<bool> IsAvailableOfflineAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
        {
            if (artifact.IsAvailableOffline == true)
                return true;

            return false;
        }
    }
}
