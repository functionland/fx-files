using Functionland.FxFiles.Client.Shared.Services.Implementations.FileService;

namespace Functionland.FxFiles.Client.Shared.Services
{
    public partial class FakeFileServiceFactory
    {
        [AutoInject] public IServiceProvider ServiceProvider { get; set; }

        public FakeFileService CreateSimpleFileListOnRoot(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            return new FakeFileService(ServiceProvider,
                new List<FsArtifact>
                {
                    CreateFile("/image summer[size=12kb].jpg"),
                    CreateFile("/image germany[size=30kb]..jpg"),
                    CreateFile("/proposal v1-2[size=120b]..pdf"),
                },
                actionLatency,
                enumerationLatency
                );
        }

        public FakeFileService CreateFolders(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            return new FakeFileService(
                ServiceProvider,
                new List<FsArtifact>
                {
                    CreateFolder("/images"),
                    CreateFolder("/docs"),
                    CreateFile("/images/image summer[size=120mb].jpg"),
                    CreateFile("/images/image germany[size=5mb].jpg"),
                    CreateFile("/docs/proposal v1-2[size=50mb].pdf"),
                },
                actionLatency,
                enumerationLatency
            );
        }

        public FakeFileService CreateNeste4dFolders(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            return new FakeFileService(
                ServiceProvider,
                new List<FsArtifact>
                {
                    CreateFolder("/images"),
                    CreateFolder("/images/summer"),
                    CreateFolder("/images/winter"),
                    CreateFolder("/docs"),
                    CreateFile("/images/summer/image summer[size=12mb].jpg"),
                    CreateFile("/images/summer/image germany[size=30mb].jpg"),
                    CreateFile("/images/winter/image summer[size=44mb].jpg"),
                    CreateFile("/images/winter/image germany[size=53mb].jpg"),
                    CreateFile("/docs/proposal v1-2[size=70mb].pdf"),
                },
                actionLatency,
                enumerationLatency
            );
        }

        public FakeFileService CreateTypical(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            return new FakeFileService(ServiceProvider,
                new List<FsArtifact>
                {
                    CreateFolder("/images"),
                    CreateFolder("/images/summer"),
                    CreateFolder("/images/winter"),
                    CreateFolder("/images/winter/firstweek"),
                    CreateFolder("/images/winter/firstweek/firstday"),
                    CreateFolder("/images/winter/firstweek/firstday/firsthour"),
                    CreateFolder("/images/winter/firstweek/firstday/firsthour/firstminute"),
                    CreateFolder("/images/winter/firstweek/firstday/firsthour/firstminute/firstsecond"),
                    CreateFolder("/images/winter/firstweek/firstday/firsthour/firstminute/firstsecond/firstmillisecond"),
                    CreateFolder("/docs"),
                    CreateFile("/images/summer/image summer[size=120mb].jpg"),
                    CreateFile("/images/summer/image germany[size=120mb].jpg"),
                    CreateFile("/images/winter/image summer[size=120mb].jpg"),
                    CreateFile("/images/winter/image germany[size=120mb].jpg"),
                    CreateFile("/images/winter/firstweek/introduce australia[size=5mb].txt"),
                    CreateFile("/images/winter/firstweek/image australia[size=50mb].jpg"),
                    CreateFile("/images/winter/firstweek/firstday/personal stuff[size=55mb].txt"),
                    CreateFile("/images/winter/firstweek/firstday/image family[size=5mb].jpg"),
                    CreateFile("/images/winter/firstweek/firstday/firsthour/image workshop[size=56mb].jpg"),
                    CreateFile("/images/winter/firstweek/firstday/firsthour/firstminute/meetingdoc[size=5mb].txt"),
                    CreateFile("/images/winter/firstweek/firstday/firsthour/firstminute/firstsecond/cv[size=5mb].txt"),
                    CreateFile("/images/winter/firstweek/firstday/firsthour/firstminute/firstsecond/image workplace[size=500mb].jpg"),
                    CreateFile("/docs/proposal v1-2[size=5mb].pdf"),
                },
                actionLatency,
                enumerationLatency);
        }
        public FakeFileService CreateIsSharedWithMeArtifacts(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            return new FakeFileService(
                ServiceProvider,
                new List<FsArtifact>
                {
                    CreateFolder("/shared"),
                    CreateFolder("/shared/Document"),
                    CreateFolder("/shared/pictures"),
                    CreateFile("/shared/Document/docs[size=5mb].pdf"),
                    CreateFile("/shared/Document/docs[size=5mb].txt"),
                    CreateFile("/shared/Document/Thesis[size=5mb].pdf"),
                },
                actionLatency,
                enumerationLatency
            );
        }
        public FakeFileService CreateIsSharedByMeArtifacts(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
        {
            return new FakeFileService(
                ServiceProvider,
                new List<FsArtifact>
                {
                    CreateFolder("/Files"),
                    CreateFolder("/Files/video"),
                    CreateFolder("/Files/Audio"),
                    CreateFolder("/Files/Document"),
                    CreateFolder("/Files/others"),
                    CreateFile("/Files/Document/docs[size=5mb].pdf"),
                },
                actionLatency,
                enumerationLatency
            );
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

        public static FsArtifact CreateDrive(string drivePath)
        {
            var driveName = drivePath;

            return new FsArtifact(drivePath, driveName, FsArtifactType.Drive, FsFileProviderType.InternalMemory);
        }
    }
}
