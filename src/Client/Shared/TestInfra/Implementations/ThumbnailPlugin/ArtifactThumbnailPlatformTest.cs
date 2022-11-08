using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

public abstract class ArtifactThumbnailPlatformTest<TFileService> : PlatformTest
    where TFileService : IFileService
{
    public IArtifactThumbnailService<TFileService> ArtifactThumbnailService { get; set; }
    TFileService FileService { get; set; }

    public ArtifactThumbnailPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService, TFileService fileService)
    {
        FileService = fileService;
        ArtifactThumbnailService = artifactThumbnailService;
    }

    protected override async Task OnRunAsync()
    {
        var rootPath = OnGetRootPath();
        FsArtifact? testsRootArtifact = null;

        try
        {
            try
            {
                testsRootArtifact = await FileService.CreateFolderAsync(rootPath, "ThumbnailTestsFolder");
            }
            catch (ArtifactAlreadyExistsException ex)
            {
                var rootArtifacts = await FileService.GetArtifactsAsync(rootPath).ToListAsync();
                testsRootArtifact = rootArtifacts.FirstOrDefault(rootArtifact => rootArtifact.FullPath == Path.Combine(rootPath, "ThumbnailTestsFolder"));
            }

            var testRootArtifact = await FileService.CreateFolderAsync(testsRootArtifact.FullPath!, $"TestRun-{DateTimeOffset.Now:yyyyMMddHH-mmssFFF}");
            var testRoot = testRootArtifact.FullPath!;


            var artifacts = await FileService.GetArtifactsAsync(testRoot).ToListAsync();

            Assert.AreEqual(0, artifacts.Count, "new folder must be empty");

            var fileNameWithoutExtension = Guid.NewGuid().ToString();

            var generalArtifact = await GetArtifactAsync(testRoot, fileNameWithoutExtension);
            if (generalArtifact is not null)
            {
                var thumbnailPath = await ArtifactThumbnailService.GetOrCreateThumbnailAsync(generalArtifact, ThumbnailScale.Medium);

                Assert.IsNotNull(thumbnailPath, "Thumbnail created");
            }

            await TestPluginAsync(testRoot);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private async Task TestPluginAsync(string testRoot, CancellationToken? cancellationToken = null)
    {
        await OnPluginSpecificTestAsync(testRoot, cancellationToken);
    }


    protected virtual Task OnPluginSpecificTestAsync(string testRoot, CancellationToken? cancellationToken = null)
    {
        return Task.CompletedTask;
    }

    private async Task<FsArtifact?> GetArtifactAsync(string testRoot, string fileNameWithoutExtension, CancellationToken? cancellationToken = null)
    {
        return await OnGetArtifactAsync(testRoot, fileNameWithoutExtension, cancellationToken);
    }

    protected virtual Task<FsArtifact?> OnGetArtifactAsync(string testRoot, string fileNameWithoutExtension, CancellationToken? cancellationToken = null)
    {
        return Task.FromResult<FsArtifact?>(null);
    }

    protected abstract string OnGetRootPath();
}
