using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeLocalDbArtifactServiceFactory
{
    public FakeLocalDbArtifactService CreateSyncScenario01()
    {
        var fsArtifacts = new List<FsArtifact>();

        return new FakeLocalDbArtifactService(fsArtifacts);
    }

    public FakeLocalDbArtifactService CreateSyncScenario02()
    {
        var fsArtifacts = new List<FsArtifact>()
        {
            CreateFolderScenario01(FulaConvention.FulaRootPath),
            CreateFolderScenario01(FulaConvention.FulaFilesRootPath),
            CreateFolderScenario01("\\MyFiles\\Documents")
            //CreateFolderScenario01(FulaConvention.FulaSharedRootPath),
            //CreateFolderScenario01($"{FulaConvention.FulaFilesRootPath}\\Documents"),
            //CreateFolderScenario01($"{FulaConvention.FulaFilesRootPath}\\Pictures"),

        };

        foreach (var fsArtifact in fsArtifacts)
        {
            fsArtifact.ContentHash += fsArtifact.Name;
        }

        return new FakeLocalDbArtifactService(fsArtifacts);
    }

    public static FsArtifact CreateFileScenario01(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var extension = Path.GetExtension(filePath);

        return new FsArtifact(filePath, fileName, FsArtifactType.File, FsFileProviderType.Fula)
        {
            ParentFullPath = Path.GetDirectoryName(filePath),
            FileExtension = extension,
            ContentHash = filePath + DateTimeOffset.UtcNow.ToString(),
            LastModifiedDateTime = DateTimeOffset.UtcNow,
            CreateDateTime = DateTimeOffset.UtcNow,
            LocalFullPath = GetLocalPathBasedOnFulaPath(GetLocalRootPath(), FulaConvention.FulaRootPath, filePath)
        };
    }

    public static FsArtifact CreateFolderScenario01(string folderPath)
    {
        var folderName = Path.GetFileName(folderPath);

        return new FsArtifact(folderPath, folderName, FsArtifactType.Folder, FsFileProviderType.Fula)
        {
            ParentFullPath = Path.GetDirectoryName(folderPath),
            ContentHash = folderPath + DateTimeOffset.UtcNow.ToString(),
            LastModifiedDateTime = DateTimeOffset.UtcNow,
            CreateDateTime = DateTimeOffset.UtcNow,
            LocalFullPath = GetLocalPathBasedOnFulaPath(GetLocalRootPath(), folderPath)
        };
    }

    private static string GetLocalPathBasedOnFulaPath(string rootLocalPath, string fulaPath)
    {
        return Path.Combine(rootLocalPath, fulaPath.TrimStart(Path.DirectorySeparatorChar));
    }

    private static string GetLocalRootPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    }
}
