using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Services.Implementations
{
    public abstract class LocalDeviceFileService : IFileService
    {
        public virtual Task CopyArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task<FsArtifact> CreateFolderAsync(string path, string folderName, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteArtifactsAsync(FsArtifact[] artifacts, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public virtual IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, string? searchText = null, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task MoveArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }
    }
}
