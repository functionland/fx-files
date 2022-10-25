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

            var createdArtifact = await CreateArtifactAsync(testRoot);

            var thumbnailPath = await ArtifactThumbnailService.GetOrCreateThumbnailAsync(createdArtifact, ThumbnailScale.Medium);

            Assert.IsNotNull(thumbnailPath, "Image thumbnail created");

            var imageThumbnailArtifact = await FileService.GetArtifactAsync(thumbnailPath);


            Assert.IsNotNull(imageThumbnailArtifact, "Image thumbnail artifact founded!");
        }
        catch (Exception ex)
        {
            throw;
        }
    }


    protected static string GetSampleFileLocalPath() =>
       Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "_content/Functionland.FxFiles.Client.Shared", "images", "Files");
    protected abstract Task<FsArtifact> CreateArtifactAsync(string testRoot, CancellationToken? cancellationToken = null);

    protected abstract string OnGetRootPath();
}
