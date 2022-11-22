namespace Functionland.FxFiles.Client.Shared.Services.Implementations.OfflineAvailability;

public partial class FakeOfflineAvailabilityServiceFactory
{
    public FakeOfflineAvailabilityService CreateFsArtifacts(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        return new FakeOfflineAvailabilityService(
            new List<FsArtifact>
            {
                CreateFolder("/NewFolder1", true),
                CreateFolder("/NewFolder1/Picture", true),
                CreateFolder("/Document", true),
                CreateFile("/NewFolder1/image.jpg", true),
                CreateFile("/Document/douc.pdf", true),
            },
            new List<FsArtifact>
            {
                CreateFolder("/NewFolder2"),
                CreateFolder("/NewFolder3"),
                CreateFolder("/NewFolder4/SubFolder"),
                CreateFile("/NewFolder2/image1.jpg"),
            },
            actionLatency,
            enumerationLatency);
    }
    public FakeOfflineAvailabilityService CreateOfflineAvailabilityFsArtifacts(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var fsArtifacts = new List<FsArtifact>();

        return new FakeOfflineAvailabilityService(
            new List<FsArtifact>
            {
                CreateFolder("/Video", true),
                CreateFolder("/Audio", true),
                CreateFolder("/Photos", true),
                CreateFolder("/Docs", true),
                CreateFile("/Photos/image11.jpg", true),
                CreateFile("/Photos/image12.jpg", true),
                CreateFile("/Docs/File.txt", true),
                CreateFile("/Docs/proposal.pdf", true),
            },
            fsArtifacts,
            actionLatency,
            enumerationLatency);
    }
    public FakeOfflineAvailabilityService CreateOnlineAvailabilityFsArtifacts(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var fsArtifacts = new List<FsArtifact>();

        return new FakeOfflineAvailabilityService(
            fsArtifacts,
            new List<FsArtifact>
            {
                CreateFolder("/MyPicture"),
                CreateFolder("/MyDocument"),
                CreateFile("/MyDocument/Proposal/Proposal.pdf"),
                CreateFile("/MyDocument/Thesis/Thesis.pdf"),
                CreateFile("/MyPicture/Uni/image1"),
                CreateFile("/MyPicture/Uni/image2"),
            },
            actionLatency,
            enumerationLatency);
    }

    public FakeOfflineAvailabilityService CreateTypical(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        return new FakeOfflineAvailabilityService(
            new List<FsArtifact>
            {
                CreateFolder("/DCIM", true),
                CreateFolder("/DCIM/Camers", true),
                CreateFolder("/DCIM/Telegram", true),
                CreateFolder("/DCIM/DCIM/Camera", true),
                CreateFolder("/DCIM/DCIM/Screenshots", true),
                CreateFolder("/DCIM/DCIM/Camera/firstday", true),
                CreateFolder("/DCIM/DCIM/Camera/firstday/firsthour", true),
                CreateFolder("/DCIM/DCIM/Camera/firstday/firsthour/firstminute", true),
                CreateFolder("/DCIM/DCIM/Camera/firstday/firsthour/firstminute/firstsecond", true),
                CreateFolder("/DCIM/DCIM/Camera/firstday/firsthour/firstminute/firstsecond/firstmillisecond", true),
                CreateFolder("/DCIM/DCIM/Screenshots/firstday", true),
                CreateFolder("/DCIM/DCIM/Screenshots/firstday/firsthour", true),
                CreateFolder("/DCIM/DCIM/Screenshots/firstday/firsthour/firstminute", true),
                CreateFolder("/DCIM/DCIM/Screenshots/firstday/firsthour/firstminute/firstsecond", true),
                CreateFolder("/DCIM/DCIM/Screenshots/firstday/firsthour/firstminute/firstsecond/firstmillisecond", true),
                CreateFile("/DCIM/DCIM/Screenshots/firstday/firsthour/firstminute/firstsecond/firstmillisecond/image.jpg", true),
                CreateFile("/DCIM/DCIM/Screenshots/firstday/firsthour/firstminute/firstsecond/firstmillisecond/image family.jpg", true),
            },
            new List<FsArtifact>
            {
                CreateFolder("/Downloads"),
                CreateFolder("/Downloads/Telegram Desktop"),
                CreateFolder("/Downloads/Telegram Desktop/firstweek"),
                CreateFolder("/Downloads/Telegram Desktop/firstweek/firstday"),
                CreateFolder("/Downloads/Telegram Desktop/firstweek/firstday/firsthour"),
                CreateFolder("/Downloads/Telegram Desktop/firstweek/firstday/firsthour/firstminute"),
                CreateFolder("/Documents"),
                CreateFolder("/Documents/winter"),
                CreateFolder("/Documents/winter/firstweek"),
                CreateFolder("/Documents/winter/firstweek/firstday"),
                CreateFile("/Documents/winter/firstweek/firstday/Thesis V1.pdf"),
                CreateFile("/Documents/winter/firstweek/firstday/Thesis V2.pdf"),
                CreateFile("/Documents/winter/firstweek/firstday/Thesis V3.pdf"),
            },
            actionLatency,
            enumerationLatency);
    }

    private static FsArtifact CreateFile(string filePath, bool isAvailableOffline = false)
    {
        var fileName = Path.GetFileName(filePath);
        var extension = Path.GetExtension(filePath);

        return new FsArtifact(filePath, fileName, FsArtifactType.File, FsFileProviderType.InternalMemory)
        {
            FileExtension = extension,
            IsAvailableOfflineRequested = isAvailableOffline
        };
    }

    private static FsArtifact CreateFolder(string folderPath, bool isAvailableOffline = false)
    {
        var folderName = Path.GetFileName(folderPath);

        return new FsArtifact(folderPath, folderName, FsArtifactType.Folder, FsFileProviderType.InternalMemory)
        {
            ParentFullPath = folderPath.Replace($"/{folderName}", ""),
            IsAvailableOfflineRequested = isAvailableOffline
        };
    }
}
