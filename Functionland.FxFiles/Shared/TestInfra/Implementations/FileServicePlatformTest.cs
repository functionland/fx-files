using System.Text;

namespace Functionland.FxFiles.Shared.TestInfra.Implementations
{
    public abstract partial class FileServicePlatformTest : PlatformTest
    {
        protected abstract string OnGetTestsRootPath();
        protected abstract IFileService OnGetFileService();

        protected async Task OnRunFileServiceTestAsync(IFileService fileService, string rootPath)
        {

            FsArtifact? testsRootArtifact = null;
            try
            {
                try
                {
                    testsRootArtifact = await fileService.CreateFolderAsync(rootPath, "FileServiceTestsFolder");
                }
                catch (DomainLogicException ex) when (ex.Message == "The folder already exists exception") //TODO: use AppStrings for exception
                {
                    var rootArtifacts = await GetArtifactsAsync(fileService, rootPath);
                    testsRootArtifact = rootArtifacts.FirstOrDefault(rootArtifact => rootArtifact.FullPath == Path.Combine(rootPath, "FileServiceTestsFolder"));
                }

                var testRootArtifact = await fileService.CreateFolderAsync(testsRootArtifact.FullPath!, $"TestRun-{DateTimeOffset.Now:yyyyMMddHH-mmssFFF}");
                var testRoot = testRootArtifact.FullPath!;


                var artifacts = await GetArtifactsAsync(fileService, testRoot);

                Assert.AreEqual(0, artifacts.Count, "new folder must be empty");

                await fileService.CreateFolderAsync(testRoot, "Folder 1");
                await fileService.CreateFolderAsync(Path.Combine(testRoot, "Folder 1"), "Folder 11");
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

                #region Create files 1

                var file2 = await fileService.CreateFileAsync(Path.Combine(testRoot, "file2.txt"), GetSampleFileStream());
                artifacts = await GetArtifactsAsync(fileService, testRoot);
                Assert.AreEqual(3, artifacts.Count, "Create file in root");

                #endregion

                #region Copying files 1

                var copyingFiles = new[] { file2 };

                await fileService.CopyArtifactsAsync(copyingFiles, Path.Combine(testRoot, "Folder 2"));
                artifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 2"));
                Assert.AreEqual(2, artifacts.Count, "Copy a file to a folder. Created on destination");

                #endregion

                #region Deleting files 1

                var deletingFiles = new[] { file2 };

                artifacts = await GetArtifactsAsync(fileService, testRoot);
                Assert.AreEqual(3, artifacts.Count, "Before deleting operation.");

                await fileService.DeleteArtifactsAsync(deletingFiles);
                artifacts = await GetArtifactsAsync(fileService, testRoot);
                Assert.AreEqual(2, artifacts.Count, "Delete a file.");

                #endregion

                #region Check folder path exist

                var folder3 = await fileService.CreateFolderAsync(testRoot, "Folder 3");
                artifacts = artifacts = await GetArtifactsAsync(fileService, testRoot);
                Assert.AreEqual(3, artifacts.Count, "Create a folder in root");

                var fsArtifactsChanges = await fileService.CheckPathExistsAsync(new List<string?>() { folder3.FullPath });
                var fsArtifactChanges = fsArtifactsChanges.FirstOrDefault();
                var isExist = fsArtifactChanges?.IsPathExist ?? false;
                Assert.AreEqual<bool>(true, isExist, "Check folder exist");

                #endregion

                #region Renaming folders 1

                await fileService.RenameFolderAsync(folder3.FullPath, "Folder 4");
                fsArtifactsChanges = await fileService.CheckPathExistsAsync(new List<string?>() { Path.Combine(testRoot, "Folder 4") });
                fsArtifactChanges = fsArtifactsChanges.FirstOrDefault();
                var isRenamed = fsArtifactChanges?.IsPathExist ?? false;
                Assert.AreEqual<bool>(true, isExist, "Rename a folder");

                #endregion

                #region Renameing files 1

                await fileService.RenameFileAsync(Path.Combine(testRoot, "Folder 2/file1.txt"), "file22");
                fsArtifactsChanges = await fileService.CheckPathExistsAsync(new List<string?>() { Path.Combine(testRoot, "Folder 2/file22.txt") });
                fsArtifactChanges = fsArtifactsChanges.FirstOrDefault();
                isRenamed = fsArtifactChanges?.IsPathExist ?? false;
                Assert.AreEqual<bool>(true, isExist, "Rename a file");

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
