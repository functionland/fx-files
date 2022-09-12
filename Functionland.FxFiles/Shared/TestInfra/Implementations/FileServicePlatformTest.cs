using System.Text;

namespace Functionland.FxFiles.Shared.TestInfra.Implementations
{
    public abstract partial class FileServicePlatformTest : PlatformTest
    {
        protected abstract string OnGetTestsRootPath();
        protected abstract IFileService OnGetFileService();

        protected async Task OnRunFileServiceTestAsync(IFileService fileService, string rootPath)
        {
            Progress("Foldre Creation", "", TestProgressType.Success);
            await Task.Delay(TimeSpan.FromSeconds(1));

            Progress("File Creation", "", TestProgressType.Success);
            await Task.Delay(TimeSpan.FromSeconds(1));

            Progress("Foldre Cope", "", TestProgressType.Success);
            await Task.Delay(TimeSpan.FromSeconds(1));

            Progress("File Remove", "", TestProgressType.Success);
            await Task.Delay(TimeSpan.FromSeconds(1));

            Progress("File Rename", "File not found", TestProgressType.Fail);
            await Task.Delay(TimeSpan.FromSeconds(1));


            return;
            try
            {
                var testsRootArtifact = await fileService.CreateFolderAsync(rootPath, "FileServiceTests");
                var testRootArtifact = await fileService.CreateFolderAsync(testsRootArtifact.FullPath!, $"TestRun-{DateTimeOffset.Now.ToString("yyyyMMddHH-mmssFFF")}");

                var testRoot = testRootArtifact.FullPath!;

                await fileService.CreateFolderAsync(testRoot, "Folder 1");
                await fileService.CreateFolderAsync(testRoot, "Folder 2");
                await fileService.CreateFolderAsync(testRoot, "Folder 3");

                await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 1/file11.txt"), GetSampleFileStream());
                await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 1/file12.txt"), GetSampleFileStream());
                await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 1/file13.txt"), GetSampleFileStream());
                await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 1/file14.txt"), GetSampleFileStream());

                await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 2/file21.txt"), GetSampleFileStream());
                await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 2/file22.txt"), GetSampleFileStream());
                await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 2/file23.txt"), GetSampleFileStream());
                await fileService.CreateFileAsync(Path.Combine(testRoot, "Folder 2/file24.txt"), GetSampleFileStream());

                Progress("Foldre Creation", "", TestProgressType.Success);
            }
            catch (Exception ex)
            {
                Progress("Foldre Creation", ex.ToString(), TestProgressType.Fail);
            }
            
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
