using Functionland.FxFiles.Client.Shared.Extensions;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.OfflineAvailability
{
    public class FakeOfflineAvailabilityService : IOfflineAvailabilityService
    {
        private readonly List<FsArtifact> _FsArtifacts;
        private readonly List<FsArtifact>? _AllFulaFsArtifacts;
        public TimeSpan? ActionLatency { get; set; }
        public TimeSpan? EnumerationLatency { get; set; }
        public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

        public FakeOfflineAvailabilityService(IEnumerable<FsArtifact> fsArtifacts, IEnumerable<FsArtifact>? allFulaFsArtifacts = null, TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            _FsArtifacts = fsArtifacts.ToList();
            _AllFulaFsArtifacts = allFulaFsArtifacts?.ToList() ?? new List<FsArtifact>();

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
            if (ActionLatency != null)
            {
                await Task.Delay(ActionLatency.Value);
            }

            _FsArtifacts.Add(artifact);
        }

        public async Task RemoveAvailableOfflineAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
        {
            var lowerCaseArtifact = AppStrings.Artifact.ToLowerText();

            if (ActionLatency != null)
            {
                await Task.Delay(ActionLatency.Value);
            }

            if (artifact is null)
                throw new ArtifactDoseNotExistsException(StringLocalizer.GetString(AppStrings.ArtifactDoseNotExistsException, artifact?.ArtifactType.ToString() ?? lowerCaseArtifact));

            _FsArtifacts.Remove(artifact);
        }

        public async Task<bool> IsAvailableOfflineAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
        {
            if (ActionLatency != null)
            {
                await Task.Delay(ActionLatency.Value);
            }

            if (artifact.IsAvailableOffline == true)
                return true;

            return false;
        }
    }
}
