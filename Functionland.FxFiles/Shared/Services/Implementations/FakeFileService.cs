using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Services.Implementations
{
    public class FakeFileService : IFileService
    {

        private ConcurrentBag<FsArtifact> _files = new ConcurrentBag<FsArtifact>();

        public FakeFileService(IEnumerable<FsArtifact> files)
        {
            foreach (var file in files)
            {
                _files.Add(file);
            }
        }


        public Task CopyArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
        {
            if (path is null) throw new Exception();
            var originDevice = $"{Environment.MachineName}-{Environment.UserName}";

            var artifact = new FsArtifact
            {
                Name = Path.GetFileName(path),
                FullPath = path,
                FileExtension = Path.GetExtension(path),
                OriginDevice = originDevice,
                ThumbnailPath = path,
                ContentHash = stream.GetHashCode().ToString(),
                ProviderType = FsFileProviderType.InternalMemory,
                LastModifiedDateTime = DateTimeOffset.Now.ToUniversalTime()
            };
            _files.Add(artifact);
            return Task.FromResult(artifact);
        }

        public Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null)
        {
            var originDevice = $"{Environment.MachineName}-{Environment.UserName}";
            var addedFiles = new List<FsArtifact>();
            foreach (var artifact in from file in files
                                     let artifact = new FsArtifact
                                     {
                                         Name = Path.GetFileName(file.path),
                                         FullPath = file.path,
                                         FileExtension = Path.GetExtension(file.path),
                                         OriginDevice = originDevice,
                                         ThumbnailPath = file.path,
                                         ContentHash = file.stream.GetHashCode().ToString(),
                                         ProviderType = FsFileProviderType.InternalMemory,
                                         LastModifiedDateTime = DateTimeOffset.Now.ToUniversalTime()
                                     }
                                     select artifact)
            {
                _files.Add(artifact);
                addedFiles.Add(artifact);
            }

            return Task.FromResult(addedFiles);
        }

        public Task<FsArtifact> CreateFolderAsync(string path, string folderName, CancellationToken? cancellationToken = null)
        {
            if (path is null) throw new Exception();
            var originDevice = $"{Environment.MachineName}-{Environment.UserName}";

            var fileName = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar));
            var artifact = new FsArtifact
            {
                Name = fileName,
                FullPath = path,
                OriginDevice = originDevice,
                ProviderType = FsFileProviderType.InternalMemory,
                LastModifiedDateTime = DateTimeOffset.Now.ToUniversalTime()
            };
            _files.Add(artifact);
            return Task.FromResult(artifact);
        }

        public Task DeleteArtifactsAsync(FsArtifact[] artifacts, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, string? searchText = null, CancellationToken? cancellationToken = null)
        {
            IEnumerable<FsArtifact> files = _files;
            if (path is not null)
                files = files.Where(f => f.FullPath.StartsWith(path));

            if (searchText is not null)
                files = files.Where(f => f.Name.Contains(searchText));

            foreach(var file in files)
            {
                yield return file;
            }

        }

        public Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public Task MoveArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }
    }
}
