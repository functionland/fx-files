using Functionland.FxFiles.Client.Shared.Utils;
using System;
using System.Text;

namespace Functionland.FxFiles.Client.Shared.TestInfra.Implementations
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
                catch (ArtifactAlreadyExistsException ex)
                {
                    var rootArtifacts = await GetArtifactsAsync(fileService, rootPath);
                    testsRootArtifact = rootArtifacts.FirstOrDefault(rootArtifact => rootArtifact.FullPath == Path.Combine(rootPath, "FileServiceTestsFolder"));
                }

                var testRootArtifact = await fileService.CreateFolderAsync(testsRootArtifact.FullPath!, $"TestRun-{DateTimeOffset.Now:yyyyMMddHH-mmssFFF}");
                var testRoot = testRootArtifact.FullPath!;


                var artifacts = await GetArtifactsAsync(fileService, testRoot);

                Assert.AreEqual(0, artifacts.Count, "new folder must be empty");

                await fileService.CreateFolderAsync(testRoot, "Folder 1");
                var folder11 = await fileService.CreateFolderAsync(Path.Combine(testRoot, "Folder 1"), "Folder 11");
                var file1 = await fileService.CreateFileAsync(Path.Combine(testRoot, "file1[size=5mb].txt"), GetSampleFileStream(FsArtifactUtils.ConvertToByte("5", "mb")));
                var file11 = await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 1/file11[size=5mb].txt"), GetSampleFileStream(FsArtifactUtils.ConvertToByte("5", "mb")));

                artifacts = await GetArtifactsAsync(fileService, testRoot);
                Assert.AreEqual(2, artifacts.Count, "Create folder and file in root");

                artifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 1"));
                Assert.AreEqual(2, artifacts.Count, "Create folder and file in sub directory");


                //Expecting exceptions
                await Assert.ShouldThrowAsync<ArtifactAlreadyExistsException>(async () =>
                {
                    await fileService.CreateFolderAsync(testRoot, "Folder 1");
                }, "The folder already exists exception");

                await Assert.ShouldThrowAsync<ArtifactAlreadyExistsException>(async () =>
                {
                    await fileService.CreateFileAsync(Path.Combine(testRoot, "file1[size=5mb].txt"), GetSampleFileStream(FsArtifactUtils.ConvertToByte("5", "mb")));
                }, "The file already exists exception");


                await Assert.ShouldThrowAsync<ArtifactNameNullException>(async () =>
                {
                    await fileService.CreateFolderAsync(testRoot, "");
                }, "The folder name is null");

                await Assert.ShouldThrowAsync<ArtifactNameNullException>(async () =>
                {
                    await fileService.CreateFileAsync(Path.Combine(testRoot, ".txt"), GetSampleFileStream(FsArtifactUtils.ConvertToByte("5", "mb")));
                }, "The file name is null");


                //1. move a file
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


                //2. Move a folder
                var folder12 = await fileService.CreateFolderAsync(Path.Combine(testRoot, "Folder 1"), "Folder 12");
                var movingFolders = new[] { folder12 };

                var srcArtifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 1"));
                Assert.AreEqual(3, srcArtifacts.Count, "Create a file in source, in order to move.");

                await fileService.MoveArtifactsAsync(movingFolders, testRoot);
                srcArtifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 1"));
                Assert.AreEqual(2, srcArtifacts.Count, "Folder removed from source");

                var desArtifacts = await GetArtifactsAsync(fileService, testRoot);
                Assert.AreEqual(3, desArtifacts.Count, "Folder moved to destination");


                //3. Move multiple folders and files
                var folder3 = await fileService.CreateFolderAsync(testRoot, "Folder 3");

                var file31 = await fileService.CreateFileAsync(Path.Combine(folder3.FullPath, "file31[size=5mb].txt"), GetSampleFileStream(FsArtifactUtils.ConvertToByte("5", "mb")));
                var file32 = await fileService.CreateFileAsync(Path.Combine(folder3.FullPath, "file32[size=5mb].txt"), GetSampleFileStream(FsArtifactUtils.ConvertToByte("5", "mb")));
                var folder31 = await fileService.CreateFolderAsync(folder3.FullPath, "Folder 31");

                var movingItems = new[] { file31, file32, folder31 };

                await fileService.MoveArtifactsAsync(movingItems, testRoot);
                srcArtifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 3"));
                Assert.AreEqual(0, srcArtifacts.Count, "Move folders & files. All removed from source.");

                desArtifacts = await GetArtifactsAsync(fileService, testRoot);
                Assert.AreEqual(7, desArtifacts.Count, "Move folders & files. All moved to destination");


                //4. Move combined items: files and a folder which contains multiple files
                var file311 = await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 31/file311[size=5mb].txt"), GetSampleFileStream(FsArtifactUtils.ConvertToByte("5", "mb")));
                var file312 = await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 31/file312[size=5mb].txt"), GetSampleFileStream(FsArtifactUtils.ConvertToByte("5", "mb")));
                artifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 31"));
                Assert.AreEqual(2, artifacts.Count, "Create files in a folder which is about to move.");

                Array.Clear(movingItems);
                artifacts = await GetArtifactsAsync(fileService, testRoot);

                file31 = artifacts.Where(f => f.Name == "file31[size=5mb].txt").FirstOrDefault();
                Assert.IsNotNull(file32, "File exists in source, before move.");

                file32 = artifacts.Where(f => f.Name == "file32[size=5mb].txt").FirstOrDefault();
                Assert.IsNotNull(file32, "File exists in source, before move.");

                folder31 = artifacts.Where(f => f.Name == "Folder 31").FirstOrDefault();
                Assert.IsNotNull(file32, "Folder exists in source, before move.");

                //Nullability of these items have already been checked in lines 136, 140, 143. No worries then.
                movingItems = new[] { file31, file32, folder31 };

                await fileService.MoveArtifactsAsync(movingItems, Path.Combine(testRoot, "Folder 2"));
                srcArtifacts = await GetArtifactsAsync(fileService, testRoot);
                Assert.AreEqual(4, srcArtifacts.Count, "Move Files & folders. All removed from source.");
                desArtifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 2"));
                Assert.AreEqual(4, desArtifacts.Count, "Move Files & folders. All moved to destination.");


                //5. Rename a folder which contains multiple files
                await fileService.RenameFolderAsync(Path.Combine(testRoot, "Folder 2/Folder 31"), "Folder 21");
                var fsArtifactsChanges = await fileService.CheckPathExistsAsync(new List<string?>() { Path.Combine(testRoot, "Folder 2/Folder 21"),
                                                                                                      Path.Combine(testRoot, "Folder 2/Folder 31") });
                var newFsArtifactExists = fsArtifactsChanges.ElementAtOrDefault(0)?.IsPathExist ?? false;
                var oldFsArtifactExists = fsArtifactsChanges.ElementAtOrDefault(1)?.IsPathExist ?? false;

                var isRenamed = newFsArtifactExists && !oldFsArtifactExists;
                Assert.AreEqual<bool>(true, isRenamed, "Rename a folder with contaning files.");

                artifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 2/Folder 21"));
                Assert.AreEqual(2, artifacts.Count, "Check for the files inside the renamed folder.");


                //6. Rename a file to an already existed folder name
                await fileService.RenameFileAsync(Path.Combine(testRoot, "Folder 2/file31[size=5mb].txt"), "file21[size=5mb]");

                fsArtifactsChanges = await fileService.CheckPathExistsAsync(new List<string?>() { Path.Combine(testRoot, "Folder 2/file21[size=5mb].txt"),
                                                                                                  Path.Combine(testRoot, "Folder 2/file31[size=5mb].txt")});
                newFsArtifactExists = fsArtifactsChanges.ElementAtOrDefault(0)?.IsPathExist ?? false;
                oldFsArtifactExists = fsArtifactsChanges.ElementAtOrDefault(1)?.IsPathExist ?? false;

                isRenamed = newFsArtifactExists && !oldFsArtifactExists;
                Assert.AreEqual<bool>(true, isRenamed, "Rename a file");


                //Rename a file to a duplicate file name
                await Assert.ShouldThrowAsync<ArtifactAlreadyExistsException>(async () =>
                {
                    await fileService.RenameFileAsync(Path.Combine(testRoot, "Folder 2/file21[size=5mb].txt"), "file1[size=5mb]");
                }, "The file already exists exception");


                //7.Copy files and folders
                var copyingItems = new[] { folder11, file11 };

                await fileService.CopyArtifactsAsync(copyingItems, testRoot);
                desArtifacts = await GetArtifactsAsync(fileService, testRoot);
                Assert.AreEqual(6, desArtifacts.Count, "Copy files and folder. All Copyied to destination.");


                //8. Move files and folders in case of duplications. (overWrite = true)
                var file111 = await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 11/file111[size=5mb].txt"), GetSampleFileStream(FsArtifactUtils.ConvertToByte("5", "mb")));
                var file112 = await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 11/file112[size=5mb].txt"), GetSampleFileStream(FsArtifactUtils.ConvertToByte("5", "mb")));

                var file113 = await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 1/Folder 11/file113[size=5mb].txt"), GetSampleFileStream(FsArtifactUtils.ConvertToByte("5", "mb")));
                var file114 = await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 1/Folder 11/file114[size=5mb].txt"), GetSampleFileStream(FsArtifactUtils.ConvertToByte("5", "mb")));

                await fileService.MoveArtifactsAsync(copyingItems, testRoot, true);

                srcArtifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 1"));
                Assert.AreEqual(0, srcArtifacts.Count, "Move items, including duplicate folder. All removed from source.");

                desArtifacts = await GetArtifactsAsync(fileService, testRoot);
                Assert.AreEqual(6, desArtifacts.Count, "Move items, including duplicate folder. All moved to destination.");

                desArtifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 11"));
                Assert.AreEqual(4, desArtifacts.Count, "OverWrite duplicate folder. Extra files added in duplicate folder.");

                fsArtifactsChanges = await fileService.CheckPathExistsAsync(new List<string?>() { Path.Combine(testRoot, "Folder 1/Folder 11/file113[size=5mb].txt"),
                                                                                                  Path.Combine(testRoot, "Folder 1/Folder 11/file114[size=5mb].txt")});

                var isFile113Exists = fsArtifactsChanges.ElementAtOrDefault(0)?.IsPathExist ?? false;
                var isFile114Exists = fsArtifactsChanges.ElementAtOrDefault(1)?.IsPathExist ?? false;

                var isAllFileRemoved = !isFile113Exists && !isFile114Exists;
                Assert.AreEqual<bool>(true, isAllFileRemoved, "Move duplicate items. All removed from dulicate source sub directory.");


                //9. Copy files and folders in case of duplications. (overWrite = true)
                artifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 2/Folder 21"));
                Assert.AreEqual(2, artifacts.Count, "Files exist in sub directory. All ready to copy.");

                file311 = artifacts.Where(f => f.Name == "file311[size=5mb].txt").FirstOrDefault();
                Assert.IsNotNull(file311, "File exists in source (sub directory), before copy.");

                file312 = artifacts.Where(f => f.Name == "file312[size=5mb].txt").FirstOrDefault();
                Assert.IsNotNull(file312, "File exists in source (sub directory), before copy.");

                artifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 2"));
                var folder21 = artifacts.Where(f => f.Name == "Folder 21").FirstOrDefault();
                Assert.IsNotNull(folder21, "Folder exists. All set to copy.");

                copyingItems = new[] { folder21 };

                await fileService.CopyArtifactsAsync(copyingItems, Path.Combine(testRoot, "Folder 3"));

                desArtifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 3"));
                Assert.AreEqual(1, desArtifacts.Count, "Copy folder with files inside. Folder copyied in destination.");

                desArtifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 3/Folder 21"));
                Assert.AreEqual(2, desArtifacts.Count, "Copy folder with files inside. All files copyied in sub folder");

                await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 2/Folder 21/file313[size=5mb].txt"), GetSampleFileStream(FsArtifactUtils.ConvertToByte("5", "mb")));
                srcArtifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 2/Folder 21"));
                Assert.AreEqual(3, srcArtifacts.Count, "Create file in sub directory, before move the whole sub directory.");

                artifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 2"));
                folder21 = artifacts.Where(f => f.Name == "Folder 21").FirstOrDefault();
                Assert.IsNotNull(folder21, "Folder exists. All set to copy.");

                copyingItems = new[] { folder21 };

                await fileService.CopyArtifactsAsync(copyingItems, Path.Combine(testRoot, "Folder 3"), true);
                desArtifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 3/Folder 21"));
                Assert.AreEqual(3, desArtifacts.Count, "Copy folder with files inside. All files including duplicate one copyied in sub folder");


                //10. Delete files and folders
                artifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 2"));
                var file21 = artifacts.Where(f => f.Name == "file21[size=5mb].txt").FirstOrDefault();
                Assert.IsNotNull(file21, "File exists in source (sub directory), before delete.");

                var deleteingItems = new[] { folder21, file21 };
                await fileService.DeleteArtifactsAsync(deleteingItems);

                srcArtifacts = await GetArtifactsAsync(fileService, Path.Combine(testRoot, "Folder 2"));
                Assert.AreEqual(2, srcArtifacts.Count, "Delete a file and a folder. Both deleted from source.");


                Assert.Success("Test passed!");
            }
            catch (Exception ex)
            {
                try
                {
                    Assert.Fail("Test failed", ex.Message);
                }
                catch { }
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
        private Stream GetSampleFileStream(long streamSize)
        {
            var sampleText = "Hello streamer!" ;
            byte[]  charArray = new byte[streamSize];

            byte[] byteArray = Encoding.ASCII.GetBytes(sampleText).Concat(charArray).ToArray();
            MemoryStream stream = new(byteArray);
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
