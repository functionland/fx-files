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


        public async Task CopyArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null)
        {
            foreach(var artifact in artifacts)
            {
                var newPath = Path.Combine(destination, artifact.Name);

                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                if (_files.Any(f => comparer.Compare(f.FullPath, newPath) != 0 ))
                {
                    var newArtifact = CreateArtifact(newPath, artifact.ContentHash);
                    _files.Add(artifact);
                }
            }
        }

        public Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
        {
            if (path is null) throw new Exception();
            FsArtifact artifact = CreateArtifact(path, stream.GetHashCode().ToString());
            _files.Add(artifact);
            return Task.FromResult(artifact);
        }

        private static FsArtifact CreateArtifact(string path, string? contentHash)
        {
            var originDevice = $"{Environment.MachineName}-{Environment.UserName}";
            return new FsArtifact
            {
                Name = Path.GetFileName(path),
                FullPath = path,
                FileExtension = Path.GetExtension(path),
                OriginDevice = originDevice,
                ThumbnailPath = path,
                ContentHash = contentHash,
                ProviderType = FsFileProviderType.InternalMemory,
                LastModifiedDateTime = DateTimeOffset.Now.ToUniversalTime()
            };
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

        public async Task DeleteArtifactsAsync(FsArtifact[] artifacts, CancellationToken? cancellationToken = null)
        {
            var tempBag = new List<FsArtifact>();
            var finalBag = new ConcurrentBag<FsArtifact>();

            foreach (var artifact in artifacts)
            {
                foreach (var file in _files)
                {
                    finalBag.Add(file);
                }
                while (!finalBag.IsEmpty)
                {
                    _ = finalBag.TryTake(result: out FsArtifact? currentItem);
                    if (currentItem != null && string.Equals(currentItem.FullPath, artifact.FullPath, StringComparison.CurrentCultureIgnoreCase))
                    {
                        break;
                    }
                    if(currentItem != null)
                        tempBag.Add(currentItem);
                }
                foreach (var item in tempBag)
                {
                    finalBag.Add(item);
                }
            }

            _files = finalBag;
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
            string streamPath;
            if (Path.GetExtension(filePath).ToLower() == ".jpg" || 
                Path.GetExtension(filePath).ToLower() == ".png" || 
                Path.GetExtension(filePath).ToLower() == ".jpeg"
                )
            {
                streamPath = "/Files/fake-pic.jpg";
            }
            else
            {
                streamPath = "/Files/test.txt";
            }
            

            using FileStream fs = File.Open(streamPath, FileMode.Open);
            return fs;

        }

        public async Task MoveArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null)
        {
            await Task.WhenAll(
                CopyArtifactsAsync(artifacts, destination, cancellationToken),
                DeleteArtifactsAsync(artifacts, cancellationToken));
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
