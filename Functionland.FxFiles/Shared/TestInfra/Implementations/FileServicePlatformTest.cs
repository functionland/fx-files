using System.Text;

namespace Functionland.FxFiles.Shared.TestInfra.Implementations
{
    public abstract partial class FileServicePlatformTest : PlatformTest
    {
        protected abstract string OnGetTestsRootPath();
        protected abstract IFileService OnGetFileService();

        protected async Task OnRunFileServiceTestAsync(IFileService fileService, string rootPath)
        {
            try
            {
                var testsRootArtifact = new FsArtifact { FullPath = Path.Combine(rootPath, "FileServiceTestsFolder") };
                var rootArtifact = await GetArtifactsAsync(fileService, Path.Combine(rootPath, "FileServiceTestsFolder"));
                if (rootArtifact is null || rootArtifact.Count == 0)
                {
                    testsRootArtifact = await fileService.CreateFolderAsync(rootPath, "FileServiceTestsFolder");
                }
               
                var testRootArtifact = await fileService.CreateFolderAsync(testsRootArtifact.FullPath!, $"TestRun-{DateTimeOffset.Now:yyyyMMddHH-mmssFFF}");
                var testRoot = testRootArtifact.FullPath!;


                var artifacts = await GetArtifactsAsync(fileService, testRoot);

                Assert.AreEqual(0, artifacts.Count, "new folder must be empty");

                await fileService.CreateFolderAsync(testRoot, "Folder 1");
                await fileService.CreateFolderAsync(testRoot, "Folder 1/Folder 11");
                var file1 = await fileService.CreateFileAsync(Path.Combine(testRoot, "file1.txt"), GetSampleFileStream());
                var file11 = await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 1/file11.txt"), GetSampleFileStream());

                artifacts = await GetArtifactsAsync(fileService, testRoot);
                Assert.AreEqual(2, artifacts.Count, "Create folder and file in root");

                artifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 1"));
                Assert.AreEqual(2, artifacts.Count, "Create folder and file in sub directory");

                #region Moving files 1

                var movingFiles = new[] { file1 };
                await fileService.CreateFolderAsync(testRoot, "Folder 2");

                artifacts = await GetArtifactsAsync(fileService, testRoot);
                Assert.AreEqual(3, artifacts.Count, "Before moveing operation.");

                await fileService.MoveArtifactsAsync(movingFiles, Path.Combine(testRoot, "Folder 2"));
                artifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 2"));
                Assert.AreEqual(1, artifacts.Count, "Move a file to a folder. Created on destination");

                artifacts = await GetArtifactsAsync(fileService, testRoot);
                Assert.AreEqual(2, artifacts.Count, "Move a file to a folder. Removed from source");
                artifacts.Clear();

                #endregion

                Assert.Success("Test passed!");
            }
            catch (Exception ex)
            {
                Assert.Fail("Test failed", ex.Message);
            }

        }

        private static async Task<List<FsArtifact>> GetArtifactsAsync(IFileService fileService, string testRoot)
        {
            List<FsArtifact> emptyRootFolderArtifacts = new();
            await foreach (var item in fileService.GetArtifactsAsync(testRoot))
            {
                emptyRootFolderArtifacts.Add(item);
            }

            return emptyRootFolderArtifacts;
        }
        private Stream GetSampleFileStream()
        {
            var sampleText = "Hello streamer!";
            byte[] byteArray = Encoding.ASCII.GetBytes(sampleText);
            MemoryStream stream = new MemoryStream(byteArray);
            return stream;
        }

        protected override async Task OnRunAsync()
        {
            var root = OnGetTestsRootPath();
            var fileService = OnGetFileService();
            await OnRunFileServiceTestAsync(fileService, root);
        }
    }
}
