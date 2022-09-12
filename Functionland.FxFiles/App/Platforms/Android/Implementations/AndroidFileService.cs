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
using Java.Interop;

namespace Functionland.FxFiles.App.Platforms.Android.Implementations
{
    public class AndroidFileService : LocalDeviceFileService
    {
        private static android.Net.Uri contentUri = MediaStore.Files.GetContentUri("external");
        private async Task<List<FsArtifact>> GetDrivesAsync()
        {
            var storageManager = (StorageManager)MauiApplication.Current.GetSystemService(Context.StorageService);
            var storageVolumes = storageManager.StorageVolumes.ToList();

            var drives = new List<FsArtifact>();
            foreach (var storage in storageVolumes)
            {
                if (storage.IsPrimary)
                {
                    //ToDo: Fill FsArtifact extral fields 
                    drives.Add(new FsArtifact()
                    {
                        Name = "internal",
                        ArtifactType = FsArtifactType.Drive,
                        ProviderType = FsFileProviderType.InternalMemory,
                        FullPath = storage.Directory.Path,
                        Capacity = storage.Directory.FreeSpace
                    });
                }
                else
                {
                    drives.Add(new FsArtifact()
                    {
                        Name = storage.MediaStoreVolumeName,
                        ArtifactType = FsArtifactType.Drive,
                        ProviderType = FsFileProviderType.ExternalMemory,
                        FullPath = storage.Directory.Path,
                        Capacity = storage.Directory.FreeSpace
                    });
                }

                //ToDo: Hanlde when drive is Blox
            }

            return drives;
        }

        private async IAsyncEnumerable<FsArtifact> GetFilesAsync(Bundle queryArge)
        {
            string[] projection = {
                    MediaStore.IMediaColumns.BucketId,
                    MediaStore.IMediaColumns.MimeType,
                    MediaStore.IMediaColumns.DisplayName,
                    MediaStore.IMediaColumns.Size,
                    IBaseColumns.Id,
                    MediaStore.IMediaColumns.Data,//TODO: use relative path ,volume_name and display name
                    MediaStore.IMediaColumns.DateAdded,
                    MediaStore.IMediaColumns.DateModified
            };

            using var cursor = MauiApplication.Current.ContentResolver?.Query(
                        contentUri,
                        projection,
                        queryArge,
                        null
                        );

            if (cursor is not null && cursor.MoveToFirst())
            {
                do
                {
                    var parentId = cursor.GetInt(0);
                    var mimeType = cursor.GetString(1);
                    var name = cursor.GetString(2);
                    var size = cursor.GetLong(3);
                    var id = cursor.GetLong(4);
                    var data = cursor.GetString(5);
                    var dateAdded = DateTimeOffset.FromUnixTimeSeconds(cursor.GetLong(6));
                    var dateModifiedUnixFormat = cursor.GetLong(7);
                    DateTimeOffset dateModified = dateModifiedUnixFormat == 0 ? dateAdded : DateTimeOffset.FromUnixTimeSeconds(dateModifiedUnixFormat);

                    yield return new FsArtifact
                    {
                        ParentId = parentId,
                        MimeType = mimeType,
                        Name = name,
                        Size = size,
                        Id = id,
                        FullPath = data,
                        FileExtension = Path.GetExtension(data),
                        LastModifiedDateTime = dateModified
                    };
                }
                while (cursor.MoveToNext());
            }
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

            var provider = GetFsFileProviderType(path);
            if (provider == FsFileProviderType.InternalMemory)
            {
                // ToDo: Get from internal memory properly.
                await foreach (var artifact in base.GetArtifactsAsync(path, searchText, cancellationToken))
                {
                    yield return artifact;
                }
            }
            else if (provider == FsFileProviderType.ExternalMemory)
            {
                string selection = $@"({MediaStore.IMediaColumns.Data} = '{path}')"; //TODO: use relative path ,volume_name and display name

                if (String.IsNullOrWhiteSpace(searchText))
                {
                    selection = selection + $@"AND ( {MediaStore.IMediaColumns.DisplayName} like '%{searchText}%' )";
                }

                Bundle bundle = new Bundle();
                bundle.PutString(ContentResolver.QueryArgSqlSelection, selection);
                await foreach (var artifact in GetFilesAsync(bundle))
                {
                    yield return artifact;
                };
            }
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

        public override FsFileProviderType GetFsFileProviderType(string filePath)
        {
            //var drives = await GetDrivesAsync();       

            // ToDo: How to get it from the path
            if (IsFsFileProviderInternal(filePath))
            {
                return FsFileProviderType.InternalMemory;
            }
            else if (IsFsFileProviderExternal(filePath))
            {
                return FsFileProviderType.ExternalMemory;
            }
            else if (IsFsFileProviderBlox(filePath))
            {
                return FsFileProviderType.Blox;
            }
            else
                throw new Exception($"Unknown file provider for path: {filePath}");
        }

        private bool IsFsFileProviderBlox(string filePath)
        {
            throw new NotImplementedException();
        }

        private bool IsFsFileProviderExternal(string filePath)
        {
            throw new NotImplementedException();
        }

        private bool IsFsFileProviderInternal(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
