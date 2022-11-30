using Functionland.FxFiles.Client.Shared.Services.Common;

using Prism.Events;

using System.Xml.Linq;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public abstract partial class FileWatchService : IFileWatchService
    {
        [AutoInject] public IEventAggregator EventAggregator { get; set; } = default!;
        [AutoInject] public ILocalDeviceFileService FileService { get; set; } = default!;

        private readonly ConcurrentDictionary<string, (FileSystemWatcher Watcher, int WatchCount)> WatcherDictionary = new();
        public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;


        public virtual void WatchArtifact(FsArtifact fsArtifact)
        {
            if (fsArtifact?.FullPath is null)
                return;

            if (WatcherDictionary.TryGetValue(fsArtifact.FullPath, out var watchedArtifact))
            {
                WatcherDictionary[fsArtifact.FullPath] = (watchedArtifact.Watcher, watchedArtifact.WatchCount += 1);
                return;
            }

            var path = fsArtifact.ArtifactType == FsArtifactType.File ? fsArtifact.ParentFullPath : fsArtifact.FullPath;
            if (path == null) throw new ArtifactPathNullException(StringLocalizer.GetString(AppStrings.ArtifactPathIsNull, "folder"));
            AddWatcher(path);
        }

        private void AddWatcher(string path)
        {
            if (path is null)
                return;
            try
            {
                FileSystemWatcher watcher = new()
                {
                    Path = path,

                    NotifyFilter = NotifyFilters.Attributes |
                                           NotifyFilters.CreationTime |
                                           NotifyFilters.DirectoryName |
                                           NotifyFilters.FileName |
                                           NotifyFilters.LastWrite |
                                           NotifyFilters.Security |
                                           NotifyFilters.DirectoryName |
                                           NotifyFilters.FileName |
                                           NotifyFilters.Size,

                    Filter = "*.*",

                    EnableRaisingEvents = true
                };

                watcher.Changed += new FileSystemEventHandler(OnChanged);
                watcher.Created += new FileSystemEventHandler(OnAdded);
                watcher.Deleted += new FileSystemEventHandler(OnDeleted);
                watcher.Renamed += new RenamedEventHandler(OnRenamed);

                WatcherDictionary.TryAdd(path, (watcher, 1));
            }
            catch
            {
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public virtual void UnWatchArtifact(FsArtifact fsArtifact)
        {
            if (WatcherDictionary.TryGetValue(fsArtifact.FullPath, out var watcher))
            {
                if (watcher.WatchCount == 1)
                {
                    watcher.Watcher.Dispose();
                    WatcherDictionary.TryRemove(fsArtifact.FullPath, out _);
                }
                else
                {
                    var currentArtifactWatch = WatcherDictionary[fsArtifact.FullPath];
                    WatcherDictionary[fsArtifact.FullPath] = (currentArtifactWatch.Watcher, currentArtifactWatch.WatchCount -= 1);
                }
            }
        }
        public virtual void UpdateFileWatchCatch(string newPath, string oldPath)
        {
            if (WatcherDictionary.TryGetValue(oldPath, out var watchedDirectory))
            {
                watchedDirectory.Watcher.Dispose();
                WatcherDictionary.TryRemove(oldPath, out _);
                AddWatcher(newPath);
            }

        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            var fsArtifactChangesType = FsArtifactChangesType.Rename;
            var artifact = FileService.GetArtifactAsync(e.FullPath).GetAwaiter().GetResult();

            EventAggregator.GetEvent<ArtifactChangeEvent>().Publish(new ArtifactChangeEvent()
            {
                ChangeType = fsArtifactChangesType,
                FsArtifact = artifact,
                Description = e.OldFullPath
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (e is null) return;

            var isFileExist = File.Exists(e.FullPath);
            DateTimeOffset lastModifiedDateTime;
            FsArtifactType artifactType;
            var name = Path.GetFileName(e.FullPath);
            long size = 0;

            if (isFileExist)
            {
                artifactType = FsArtifactType.File;
                lastModifiedDateTime = File.GetLastWriteTime(e.FullPath);
                var fileInfo = new FileInfo(e.FullPath);
                size = fileInfo.Length;
            }
            else
            {
                artifactType = FsArtifactType.Folder;
                lastModifiedDateTime = Directory.GetLastWriteTime(e.FullPath);
            }

            EventAggregator.GetEvent<ArtifactChangeEvent>().Publish(new ArtifactChangeEvent()
            {
                ChangeType = FsArtifactChangesType.Modify,
                FsArtifact = new FsArtifact(e.FullPath, name, artifactType, FsFileProviderType.InternalMemory)
                {
                    LastModifiedDateTime = lastModifiedDateTime,
                    ParentFullPath = Path.GetDirectoryName(e.FullPath),
                    Size = size
                }
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            if (e is null) return;

            var name = Path.GetFileName(e.FullPath);

            EventAggregator.GetEvent<ArtifactChangeEvent>().Publish(new ArtifactChangeEvent()
            {
                ChangeType = FsArtifactChangesType.Delete,
                FsArtifact = new FsArtifact(e.FullPath, name, FsArtifactType.File, FsFileProviderType.InternalMemory)
            });


        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private void OnAdded(object sender, FileSystemEventArgs e)
        {
            if (e is null) return;

            var artifact = FileService.GetArtifactAsync(e.FullPath).GetAwaiter().GetResult();

            EventAggregator.GetEvent<ArtifactChangeEvent>().Publish(new ArtifactChangeEvent()
            {
                ChangeType = FsArtifactChangesType.Add,
                FsArtifact = artifact
            });
        }
    }
}
