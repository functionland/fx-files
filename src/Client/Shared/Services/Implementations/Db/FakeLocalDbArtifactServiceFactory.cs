using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeLocalDbArtifactServiceFactory
{
    public FakeLocalDbArtifactService CreateSyncScenario01()
    {
        var fsArtifacts = new List<FsArtifact>() 
        {
            CreateFolder(FulaConvention.FulaFilesRootPath),
            CreateFolder(FulaConvention.FulaSharedRootPath),
            CreateFolder($"{FulaConvention.FulaFilesRootPath}/Documents"),
            CreateFolder($"{FulaConvention.FulaFilesRootPath}/Pictures"),

        };

        return new FakeLocalDbArtifactService(fsArtifacts);
    }

    public FakeLocalDbArtifactService CreateSyncScenario02()
    {
        return new FakeLocalDbArtifactService(null);
    }

    public static FsArtifact CreateFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var extension = Path.GetExtension(filePath);

        return new FsArtifact(filePath, fileName, FsArtifactType.File, FsFileProviderType.InternalMemory)
        {
            ParentFullPath = filePath.Replace($"/{fileName}", ""),
            FileExtension = extension,
            ContentHash = DateTimeOffset.UtcNow.ToString(),
            LastModifiedDateTime = DateTimeOffset.UtcNow,
            CreateDateTime = DateTimeOffset.UtcNow,
            LocalFullPath = filePath,

        };
    }

    public static FsArtifact CreateFolder(string folderPath)
    {
        var folderName = Path.GetFileName(folderPath);

        return new FsArtifact(folderPath, folderName, FsArtifactType.Folder, FsFileProviderType.InternalMemory)
        {
            ParentFullPath = folderPath.Replace($"/{folderName}", ""),
            ContentHash = DateTimeOffset.UtcNow.ToString(),
            LastModifiedDateTime = DateTimeOffset.UtcNow,
            CreateDateTime = DateTimeOffset.UtcNow,
            LocalFullPath = folderPath,

        };
    }

}
