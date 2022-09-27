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
                    CreateFile("/image summer.jpg"),
                    CreateFile("/image germany.jpg"),
                    CreateFile("/proposal v1-2.pdf"),
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
                    CreateFile("/images/image summer.jpg"),
                    CreateFile("/images/image germany.jpg"),
                    CreateFile("/docs/proposal v1-2.pdf"),
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
                    CreateFile("/images/summer/image summer.jpg"),
                    CreateFile("/images/summer/image germany.jpg"),
                    CreateFile("/images/winter/image summer.jpg"),
                    CreateFile("/images/winter/image germany.jpg"),
                    CreateFile("/docs/proposal v1-2.pdf"),
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
                    CreateFile("/images/summer/image summer.jpg"),
                    CreateFile("/images/summer/image germany.jpg"),
                    CreateFile("/images/winter/image summer.jpg"),
                    CreateFile("/images/winter/image germany.jpg"),
                    CreateFile("/images/winter/firstweek/introduce australia.txt"),
                    CreateFile("/images/winter/firstweek/image australia.jpg"),
                    CreateFile("/images/winter/firstweek/firstday/personal stuff.txt"),
                    CreateFile("/images/winter/firstweek/firstday/image family.jpg"),
                    CreateFile("/images/winter/firstweek/firstday/firsthour/image workshop.jpg"),
                    CreateFile("/images/winter/firstweek/firstday/firsthour/firstminute/meetingdoc.txt"),
                    CreateFile("/images/winter/firstweek/firstday/firsthour/firstminute/firstsecond/cv.txt"),
                    CreateFile("/images/winter/firstweek/firstday/firsthour/firstminute/firstsecond/image workplace.jpg"),
                    CreateFile("/docs/proposal v1-2.pdf"),
                },
                actionLatency,
                enumerationLatency);
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

            return new FsArtifact(folderPath, folderName, FsArtifactType.Folder, FsFileProviderType.InternalMemory);
        }

        public static FsArtifact CreateDrive(string drivePath)
        {
            var driveName = drivePath;

            return new FsArtifact(drivePath, driveName, FsArtifactType.Drive, FsFileProviderType.InternalMemory);
        }
    }
}
