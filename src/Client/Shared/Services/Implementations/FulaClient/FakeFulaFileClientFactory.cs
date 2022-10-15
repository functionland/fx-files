using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;
public partial class FakeFulaFileClientFactory
{
    [AutoInject] IStringLocalizer<AppStrings> StringLocalizer { get; set; }
    //TODO: Make sure for adding activity to fsartifact
    public FakeFulaFileClient CreateSyncScenario01()
    {
        var fsArtifacts = new Dictionary<FulaUser, List<KeyValuePair<FsArtifact, Stream?>>>();

        fsArtifacts.Add(new FulaUser("x"), new List<KeyValuePair<FsArtifact, Stream?>>
        {
            new KeyValuePair<FsArtifact, Stream?>(CreateFolder(FulaConvention.FulaFilesRootPath),null),
            new KeyValuePair<FsArtifact, Stream?>(CreateFolder(FulaConvention.FulaSharedRootPath),null),
            new KeyValuePair<FsArtifact, Stream?>(CreateFolder($"{FulaConvention.FulaFilesRootPath}/Document"),null),
            new KeyValuePair<FsArtifact, Stream?>(CreateFile($"{FulaConvention.FulaFilesRootPath}/Document"),CreateSimpleStream()),
        });
        return new FakeFulaFileClient(fsArtifacts, StringLocalizer);
    }

    public FakeFulaFileClient CreateSyncScenario02()
    {
        return new FakeFulaFileClient(null, StringLocalizer);
    }

    public static FsArtifact CreateFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var extension = Path.GetExtension(filePath);

        return new FsArtifact(filePath, fileName, FsArtifactType.File, FsFileProviderType.InternalMemory)
        {
            FileExtension = extension
        };
    }

    public static FsArtifact CreateFolder(string folderPath)
    {
        var folderName = Path.GetFileName(folderPath);

        return new FsArtifact(folderPath, folderName, FsArtifactType.Folder, FsFileProviderType.InternalMemory)
        {
            ParentFullPath = folderPath.Replace($"/{folderName}", "")
        };
    }

    private static Stream CreateSimpleStream()
    {
        using FileStream fs = File.Open("/Files/fake-pic.jpg", FileMode.Open);
        return fs;
    }

}

