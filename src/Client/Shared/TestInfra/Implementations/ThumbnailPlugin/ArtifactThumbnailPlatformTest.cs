using Functionland.FxFiles.Client.Shared.Enums;
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

    //Attention: To run this test, you need an internet connection on your device in order to get the sample files from the internet.
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

            var testRootArtifact = await FileService.CreateFolderAsync(testsRootArtifact?.FullPath!, $"TestRun-{DateTimeOffset.Now:yyyyMMddHH-mmssFFF}");
            var testRoot = testRootArtifact.FullPath!;

            var artifacts = await FileService.GetArtifactsAsync(testRoot).ToListAsync();
            Assert.AreEqual(0, artifacts.Count, "Test root is empty");

            await PerPluginTestAsync(testRoot);

            Assert.Success("All tests passed!");
        }
        catch (Exception ex)
        {
            try
            {
                Assert.Fail("Test failed!", ex.Message);
            }
            catch { }
        }
    }


    private async Task PerPluginTestAsync(string testRoot, CancellationToken? cancellationToken = null)
    {
        await OnPluginSpecificTestAsync(testRoot, cancellationToken);
    }

    protected virtual Task OnPluginSpecificTestAsync(string testRoot, CancellationToken? cancellationToken = null)
    {
        return Task.CompletedTask;
    }

    protected virtual Task<FsArtifact?> OnGetArtifactAsync(string testRoot, string fileNameWithoutExtension, CancellationToken? cancellationToken = null)
    {
        return Task.FromResult<FsArtifact?>(null);
    }

    protected abstract string OnGetRootPath();

    protected abstract (int width, int height) GetArtifactWidthAndHeight(string imagePath);
}
