using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Services.Implementations
{
    public abstract class LocalDeviceFileService : IFileService
    {
        public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

        public abstract FsFileProviderType GetFsFileProviderType(string filePath);

        public virtual async Task CopyArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null)
        {
            foreach (var artifact in artifacts)
            {
                if (artifact.FullPath == null) continue;

                CopyAll(new DirectoryInfo(artifact.FullPath), new DirectoryInfo(destination));
            }
        }

        public virtual async Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
        {
            FsArtifact newFsArtifact = new();
            var fileName = Path.GetFileNameWithoutExtension(path);

            try
            {
                if (System.IO.File.Exists(path))
                    throw new DomainLogicException(StringLocalizer[nameof(AppStrings.CreateFileFailed)]);

                using FileStream outPutFileStream = new(path, FileMode.Create);
                await stream.CopyToAsync(outPutFileStream);

                newFsArtifact = new FsArtifact
                {
                    Name = fileName,
                    FullPath = path,
                    ArtifactType = FsArtifactType.File,
                    FileExtension = Path.GetExtension(path),
                    Size = (int)outPutFileStream.Length,
                    ProviderType = GetFsFileProviderType(path),
                    LastModifiedDateTime = DateTimeOffset.Now
                };
            }
            catch
            {
                // ToDo : Handle exceptions
                throw;
            }

            return newFsArtifact;
        }

        public virtual async Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null)
        {
            List<FsArtifact> fsArtifacts = new();

            foreach (var (path, stream) in files)
            {
                fsArtifacts.Add(await CreateFileAsync(path, stream, cancellationToken));
            }

            return fsArtifacts;
        }

        public virtual async Task<FsArtifact> CreateFolderAsync(string path, string folderName, CancellationToken? cancellationToken = null)
        {
            FsArtifact newFsArtifact = new();

            if (!string.IsNullOrWhiteSpace(folderName))
            {
                var newPath = Path.Combine(path, folderName);                

                try
                {
                    Directory.CreateDirectory(newPath);
                    newFsArtifact = new FsArtifact()
                    {
                        Name = folderName,
                        FullPath = newPath,
                        ArtifactType = FsArtifactType.Folder,
                        ProviderType = GetFsFileProviderType(newPath) 
                    };
                }
                catch (Exception)
                {
                    throw;
                    // ToDo : Handle exception
                };
            }
            else
            {
                // ToDo : Throw exception
            }

            return newFsArtifact;
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

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }

            foreach (DirectoryInfo subDirectory in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDirectory = target.CreateSubdirectory(subDirectory.Name);
                CopyAll(subDirectory, nextTargetSubDirectory);
            }
        }
    }
}
