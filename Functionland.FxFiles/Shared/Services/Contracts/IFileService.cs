using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Services.Contracts
{
    public interface IFileService
    {
        Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null);
        Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null);
        Task<FsArtifact> CreateFolderAsync(string path, string folderName, CancellationToken? cancellationToken = null);
        Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null);
        Task MoveArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null);
        Task CopyArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null);
        Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null);
        Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null);
        Task DeleteArtifactsAsync(FsArtifact[] artifacts, CancellationToken? cancellationToken = null);
        IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, string? searchText = null, CancellationToken? cancellationToken = null);
    }
}
