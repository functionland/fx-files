using Prism.Events;
using System.Security.AccessControl;

namespace Functionland.FxFiles.App.Platforms.Windows.Implementations
{
    public partial class WindowsFileWatchService : FileWatchService
    {
        [AutoInject] public IEventAggregator EventAggregator { get; set; } = default!;

        public override async Task InitialyzeAsync()
        {
            var drives = Directory.GetLogicalDrives(); 

            foreach (var drive in drives)
            {
                var driveInfo = new DriveInfo(drive);

                if (!driveInfo.IsReady) continue;

                FileSystemWatcher watcher = new()
                {
                    Path = drive,
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.Attributes |
                                       NotifyFilters.CreationTime |
                                       NotifyFilters.DirectoryName |
                                       NotifyFilters.FileName |
                                       NotifyFilters.LastAccess |
                                       NotifyFilters.LastWrite |
                                       NotifyFilters.Security |
                                       NotifyFilters.Size,

                    Filter = "*.*"
                };

                watcher.Changed += new FileSystemEventHandler(OnChanged);
                watcher.Created += new FileSystemEventHandler(OnChanged);
                watcher.Deleted += new FileSystemEventHandler(OnChanged);

                watcher.EnableRaisingEvents = true;
            }
        }

        public void OnChanged(object source, FileSystemEventArgs e)
        {
            if (e is null) return;

            FsArtifactChangesType? fsArtifactChangesType = null;
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    fsArtifactChangesType = FsArtifactChangesType.Add;
                    break;
                case WatcherChangeTypes.Deleted:
                    fsArtifactChangesType = FsArtifactChangesType.Delete;
                    break;
                case WatcherChangeTypes.Changed:
                    fsArtifactChangesType = FsArtifactChangesType.Modify;
                    break;
            }

            var isFileExist = File.Exists(e.FullPath);
            DateTimeOffset lastModifiedDateTime;
            FsArtifactType artifactType;
            var name = Path.GetFileName(e.FullPath);

            if (isFileExist)
            {
                artifactType = FsArtifactType.File;
                lastModifiedDateTime = File.GetLastWriteTime(e.FullPath);
            }
            else
            {
                artifactType = FsArtifactType.Folder;
                lastModifiedDateTime = Directory.GetLastWriteTime(e.FullPath);
            }

            EventAggregator.GetEvent<ArtifactChangeEvent>().Publish(new ArtifactChangeEvent()
            {
                ChangeType = fsArtifactChangesType,
                FsArtifact = new FsArtifact(e.FullPath, name, artifactType, FsFileProviderType.InternalMemory)
                {
                    LastModifiedDateTime = lastModifiedDateTime
                }
            });
        }
    }
}
