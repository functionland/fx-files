using Functionland.FxFiles.Client.Shared.Services.Common;
using Prism.Events;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public abstract partial class FileWatchService : IFileWatchService
    {
        [AutoInject] public IEventAggregator EventAggregator { get; set; } = default!;

        private ConcurrentDictionary<string, FileSystemWatcher> WatcherDictionary = new();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public virtual void WatchArtifact(FsArtifact fsArtifact)
        {
            if (WatcherDictionary.TryGetValue(fsArtifact.FullPath, out var _))
            {
                return;
            }

            var path = fsArtifact.ArtifactType == FsArtifactType.File ? fsArtifact.ParentFullPath : fsArtifact.FullPath;

            FileSystemWatcher watcher = new()
            {
                Path = path,

                NotifyFilter = NotifyFilters.Attributes |
                                   NotifyFilters.CreationTime |
                                   NotifyFilters.DirectoryName |
                                   NotifyFilters.FileName |
                                   NotifyFilters.LastAccess |
                                   NotifyFilters.LastWrite |
                                   NotifyFilters.Security |
                                   NotifyFilters.Size,

                Filter = "*.*",

                EnableRaisingEvents = true
            };

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);

            WatcherDictionary.TryAdd(fsArtifact.FullPath, watcher);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public virtual void UnWatchArtifact(FsArtifact fsArtifact)
        {
            if (WatcherDictionary.TryGetValue(fsArtifact.FullPath, out var watcher))
            {
                watcher.Dispose();
                WatcherDictionary.TryRemove(fsArtifact.FullPath, out _);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private void OnChanged(object source, FileSystemEventArgs e)
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
