namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public class FakeThumbnailService : IThumbnailService
    {
        public async Task<string> MakeThumbnailAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
        {
            return "/Files/fake-pic.jpg";
        }
    }
}
