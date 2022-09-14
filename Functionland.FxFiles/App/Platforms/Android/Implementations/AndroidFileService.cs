using Android.Content;
using Android.OS;
using Android.Provider;
using android = Android;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using AndroidX.DocumentFile.Provider;
using Android.OS.Storage;
using Android.Media;
using Stream = System.IO.Stream;

namespace Functionland.FxFiles.App.Platforms.Android.Implementations
{
    public partial class AndroidFileService : LocalDeviceFileService
    {
        public override Task CopyArtifactsAsync(FsArtifact[] artifacts, string destination, bool beOverWritten = false, CancellationToken? cancellationToken = null)
        {
            return base.CopyArtifactsAsync(artifacts, destination, beOverWritten, cancellationToken);
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

        public override IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, string? searchText = null, CancellationToken? cancellationToken = null)
        {
            return base.GetArtifactsAsync(path, searchText, cancellationToken);
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

        public override Task<FsFileProviderType> GetFsFileProviderTypeAsync(string filePath)
        {
            throw new NotImplementedException();
        }

    }
}
