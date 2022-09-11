using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.App.Platforms.Android.Implementations
{
    public class AndroidFileService : LocalDeviceFileService
    {
        private async Task<List<FsArtifact>> GetDrivesAsync()
        {
            // ToDo: Get the right drives
            var drives = new List<FsArtifact>
            {
                new FsArtifact()
                {
                    Name = "internal",
                    ArtifactType = FsArtifactType.Drive,
                    ProviderType = FsFileProviderType.InternalMemory
                }
            };

            if (true)
            {
                drives.Add(
                new FsArtifact()
                {
                    Name = "sdcard01",
                    ArtifactType = FsArtifactType.Drive,
                    ProviderType = FsFileProviderType.ExternalMemory
                });
            }

            return drives;
        }
        private FsFileProviderType GetProviderTypeFromPath(string path)
        {
            // ToDo: How to get it from the path
            if (path.StartsWith("intenal"))
            {
                return FsFileProviderType.InternalMemory;
            }
            else if (path.StartsWith("sdcard"))
            {
                return FsFileProviderType.ExternalMemory;
            }
            else
                throw new Exception($"Unknown file provider for path: {path}");
            
        }

        public override Task CopyArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null)
        {
            return base.CopyArtifactsAsync(artifacts, destination, cancellationToken);
        }

        public override Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
        {
            return base.CreateFileAsync(path, stream, cancellationToken);
        }

        public override Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null)
        {
            return base.CreateFilesAsync(files, cancellationToken);
        }

        public override Task<FsArtifact> CreateFolderAsync(string path, string folderName, CancellationToken? cancellationToken = null)
        {
            return base.CreateFolderAsync(path, folderName, cancellationToken);
        }

        public override Task DeleteArtifactsAsync(FsArtifact[] artifacts, CancellationToken? cancellationToken = null)
        {
            return base.DeleteArtifactsAsync(artifacts, cancellationToken);
        }

        public override async IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, string? searchText = null, CancellationToken? cancellationToken = null)
        {
            if (path is null)
            {
                var drives = await GetDrivesAsync();
                foreach (var drive in drives)
                {
                    yield return drive;
                }
                yield break;
            }

            var provider = GetProviderTypeFromPath(path);
            if (provider == FsFileProviderType.InternalMemory)
            {
                // ToDo: Get from internal memory properly.
                await foreach(var item in base.GetArtifactsAsync(path, searchText, cancellationToken))
                {
                    yield return item;
                }
            }
            else if (provider == FsFileProviderType.ExternalMemory)
            {

            }
        }

        public override Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null)
        {
            return base.GetFileContentAsync(filePath, cancellationToken);
        }

        public override Task MoveArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null)
        {
            return base.MoveArtifactsAsync(artifacts, destination, cancellationToken);
        }

        public override Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null)
        {
            return base.RenameFileAsync(filePath, newName, cancellationToken);
        }

        public override Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null)
        {
            return base.RenameFolderAsync(folderPath, newName, cancellationToken);
        }
    }
}
