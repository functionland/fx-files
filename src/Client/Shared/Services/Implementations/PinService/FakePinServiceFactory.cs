namespace Functionland.FxFiles.Client.Shared.Services.Implementations.PinService;

public partial class FakePinServiceFactory
{
    public FakePinService CreateFsArtifacts(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        return new FakePinService(
            new List<FsArtifact>
            {
                CreateFolder("/Downloads"),
                CreateFolder("/MyFiles"),
                CreateFolder("/MyFiles/Document"),
                CreateFolder("/Downloads/Telegram Desktop"),
                CreateFolder("/MyFiles/Document/NewFolder"),
                CreateFile("/MyFiles/Document/NewFolder/Paper V1[size:10mb][latency:10ms].pdf"),
                CreateFile("/MyFiles/Document/NewFolder/Paper V2[size:20mb][latency:15ms].pdf")
            },
            new List<FsArtifact>
            {
                CreateFile("/MyFiles/Document/NewFolder/Paper V1[size:10mb][latency:10ms].pdf"),
                CreateFile("/MyFiles/Document/NewFolder/Paper V2[size:20mb][latency:15ms].pdf")
            },
            actionLatency,
            enumerationLatency);
    }

    public FakePinService AllFsArtifacts(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        return new FakePinService(
            new List<FsArtifact>
            {
                CreateFolder("Music"),
                CreateFolder("/MyFiles"),
                CreateFolder("/MyFiles/Document"),
                CreateFolder("/MyFiles/Picture"),
                CreateFolder("/MyFiles/Audio"),
                CreateFolder("/MyFiles/Document/NewFolder"),
                CreateFile("/MyFiles/Document/NewFolder/Paper V1[size:15mb][latency:100ms].pdf"),
                CreateFile("/MyFiles/Document/NewFolder/Paper V2[size:30mb][latency:500ms].pdf"),
                CreateFile("/MyFiles/Picture/image[size:2mb][latency:100ms].jpg"),
                CreateFile("/MyFiles/Picture/flower[size:16mb][latency:150ms].jpg")
            },
            new List<FsArtifact>(),
            actionLatency,
            enumerationLatency);
    }


    private static FsArtifact CreateFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var extension = Path.GetExtension(filePath);

        return new FsArtifact(filePath, fileName, FsArtifactType.File, FsFileProviderType.InternalMemory)
        {
            FileExtension = extension
        };
    }

    private static FsArtifact CreateFolder(string folderPath)
    {
        var folderName = Path.GetFileName(folderPath);

        return new FsArtifact(folderPath, folderName, FsArtifactType.Folder, FsFileProviderType.InternalMemory)
        {
            ParentFullPath = folderPath.Replace($"/{folderName}", "")
        };
    }
}
