using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;
public partial class FakeFulaFileClientFactory
{
    [AutoInject] IStringLocalizer<AppStrings> StringLocalizer { get; set; }
    //TODO: Make sure for adding activity to fsartifact
    public FakeFulaFileClient CreateSyncScenario01()
    {
        var fsArtifacts = new Dictionary<FulaUser, List<KeyValuePair<FsArtifact, Stream?>>>
        {
            {
                new FulaUser("x"),
                new List<KeyValuePair<FsArtifact, Stream?>>
                {
                    CreateFolder($"{FulaConvention.FulaRootPath}"),
                    CreateFolder($"{FulaConvention.FulaRootPath}{FulaConvention.FulaFilesRootPath}"),
                    CreateFolder($"{FulaConvention.FulaRootPath}{FulaConvention.FulaSharedRootPath}"),

                    CreateFolder("\\MyFiles\\Documents"),
                    CreateFolder("\\MyFiles\\Music"),
                    CreateFolder("\\MyFiles\\Documents\\Work"),
                    CreateFolder("\\MyFiles\\Documents\\Home"),
                    CreateFile("\\MyFiles\\Documents\\fileD1.txt"),

                    CreateFolder("\\MyFiles\\Pictures"),
                    CreateFile("\\MyFiles\\Pictures\\p1.jpg"),
                    CreateFolder("\\MyFiles\\Pictures\\Winter"),
                    CreateFolder("\\MyFiles\\Prictures\\Spring"),
                    CreateFile("\\MyFiles\\Pictures\\Winter\\w1.jpg"),
                    CreateFile("\\MyFiles\\Pictures\\Spring\\s1.jpg"),
                }
            }
        };
        return new FakeFulaFileClient(fsArtifacts, StringLocalizer);
    }

    public FakeFulaFileClient CreateSyncScenario02()
    {
        return new FakeFulaFileClient(null, StringLocalizer);
    }

    //ToDo: Cover create root folder and Shared/MyFiles.
    private static KeyValuePair<FsArtifact, Stream?> CreateFolder(string folderPath)
    {
        var folderName = Path.GetFileName(folderPath);
        //var finalFolderPath = FulaConvention.FulaFilesRootPath + folderPath;

        var fsArtifact = new FsArtifact(folderPath, folderName, FsArtifactType.Folder, FsFileProviderType.Fula)
        {
            ParentFullPath = Path.GetDirectoryName(folderPath),
            ContentHash = DateTimeOffset.UtcNow.ToString(),
            LastModifiedDateTime = DateTimeOffset.UtcNow,
            CreateDateTime = DateTimeOffset.UtcNow
        };

        return new KeyValuePair<FsArtifact, Stream?>(fsArtifact, CreateSimpleStream());
    }

    private static KeyValuePair<FsArtifact, Stream?> CreateFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var extension = Path.GetExtension(filePath);

        //var finalFilePath = FulaConvention.FulaFilesRootPath + filePath;
        var fsArtifact = new FsArtifact(filePath, fileName, FsArtifactType.File, FsFileProviderType.Fula)
        {
            ParentFullPath = Path.GetDirectoryName(filePath),
            FileExtension = extension,
            ContentHash = DateTimeOffset.UtcNow.ToString(),
            LastModifiedDateTime = DateTimeOffset.UtcNow,
            CreateDateTime = DateTimeOffset.UtcNow
        };

        return new KeyValuePair<FsArtifact, Stream?>(fsArtifact, CreateSimpleStream());
    }

    private static Stream CreateSimpleStream()
    {
        //var outPutDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "_content/Functionland.FxFiles.Client.Shared", "images", "Files");
        //var a = File.ReadAllText(Path.Combine(outPutDirectory, "test.txt"));

        var tempRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        using FileStream fs = File.Open(Path.Combine(tempRoot, "fake-pic.png"), FileMode.Open);
        return fs;
    }

}

