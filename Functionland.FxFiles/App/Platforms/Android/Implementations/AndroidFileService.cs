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
    public partial class AndroidFileService : LocalDeviceFileService
    {
        private async Task<List<FsArtifact>> GetDrivesAsync()
        {
            var storageManager = MauiApplication.Current.GetSystemService(Context.StorageService) as StorageManager;
            if (storageManager is null)
            {
                throw new DomainLogicException(StringLocalizer[nameof(AppStrings.UnableToLoadStorageManager)]);
            }

            var storageVolumes = storageManager.StorageVolumes.ToList();

            var drives = new List<FsArtifact>();
            foreach (var storage in storageVolumes)
            {
                if (storage.IsPrimary) //TODO: investigate to use IsPrimary or IsRemoveable
                {
                    var time = storage.Directory.LastModified();
                    //ToDo: Fill FsArtifact extral fields 
                    drives.Add(new FsArtifact()
                    {
                        Name = "internal",
                        ArtifactType = FsArtifactType.Drive,
                        ProviderType = FsFileProviderType.InternalMemory,
                        FullPath = storage.Directory.Path,
                        Capacity = storage.Directory.UsableSpace, //TODO : investigate to use UsableSpace or freeSpace
                        Size = storage.Directory.TotalSpace,
                        //LastModifiedDateTime = storage.Directory.LastModified() //TODO: convert long to dateTime
                    });
                }
                else
                {
                    drives.Add(new FsArtifact()
                    {
                        Name = $"SD Card ({storage.MediaStoreVolumeName})",
                        ArtifactType = FsArtifactType.Drive,
                        ProviderType = FsFileProviderType.ExternalMemory,
                        FullPath = storage.Directory.Path,
                        Capacity = storage.Directory.UsableSpace,//TODO : investigate to use UsableSpace or freeSpace
                        Size = storage.Directory.TotalSpace,
                        //LastModifiedDateTime = storage.Directory.LastModified() //TODO: convert long to dateTime
                    });
                }

                //ToDo: Hanlde when drive is Blox
            }

            return drives;
        }

        private async Task<FsArtifactType> GetFsArtifactTypeAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new DomainLogicException(StringLocalizer[nameof(AppStrings.PathIsNull)]);
            }
            else if (Directory.Exists(path))
            {
                return FsArtifactType.Folder;
            }
            else if (File.Exists(path))
            {
                return FsArtifactType.File;
            }
            else
            {
                return FsArtifactType.Drive;
            }
        }

        private async Task<bool> TryGetPermissionAsync(string filePath)
        {
            if (!await HasPermissionAsync(filePath))
            {
                //MauiApplication.Current.ApplicationContext.
                //await ((MainActivity)MauiAppCompatActivity).GetSDcardStoragePermission();
                //return await HasPermissionAsync(filePath);
            }

            return true;
        }

        private async Task<bool> HasPermissionAsync(string path)
        {
            var provider = await GetFsFileProviderType(path);
            if (provider != FsFileProviderType.ExternalMemory)
            {
                return true;
            }

            var permission = MauiApplication.Current.ContentResolver.PersistedUriPermissions.ToList();
            var permissionList = new List<SDpermission>();
            foreach (var p in permission)
            {
                var result = System.IO.Path.Combine(p.Uri.PathSegments.Skip(1).Select(r => r.TrimEnd(':')).ToArray());
                if (result.Contains('/'))
                {
                    result = ReverseString(result);
                    var index = result.IndexOf('/');
                    result = ReverseString(result);
                    result = result.Substring(result.Length - index);
                }
                else if (result.Contains(':'))
                {
                    var index = result.IndexOf(":");
                    result = result.Substring(index + 1);
                }
                permissionList.Add(new SDpermission()
                {
                    Permission = p,
                    Pathsegment = result
                });
            };
            var check = permissionList.Any(r => r.Permission.IsWritePermission && path.Contains(r.Pathsegment));
            return check;
        }

        private static string ReverseString(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private DocumentFile GetDocumentFileIfAllowedToWrite(Java.IO.File file, Context context)
        {
            List<UriPermission> permissionUris = context.ContentResolver.PersistedUriPermissions.ToList();

            foreach (UriPermission permissionUri in permissionUris)
            {
                android.Net.Uri treeUri = permissionUri.Uri;
                DocumentFile rootDocFile = DocumentFile.FromTreeUri(context, treeUri);

                string rootDocFilePath = GetFullPathFromTreeUri(treeUri, GetSDCardPath());

                if (file.AbsolutePath.StartsWith(rootDocFilePath))
                {

                    var pathInRootDocParts = new List<string>();
                    while (file != null && !rootDocFilePath.Equals(file.AbsolutePath))
                    {
                        pathInRootDocParts.Add(file.Name);
                        file = file.ParentFile;
                    }

                    DocumentFile docFile = null;

                    if (pathInRootDocParts.Count == 0)
                    {
                        docFile = DocumentFile.FromTreeUri(context, rootDocFile.Uri);
                    }
                    else
                    {
                        for (int i = pathInRootDocParts.Count - 1; i >= 0; i--)
                        {
                            if (docFile == null)
                            {
                                docFile = rootDocFile.FindFile(pathInRootDocParts[i]);
                            }
                            else
                            {
                                docFile = docFile.FindFile(pathInRootDocParts[i]);
                            }
                        }
                    }
                    if (docFile != null && docFile.CanWrite())
                    {
                        return docFile;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            return null;
        }

        private string GetSDCardPath()
        {
            var storageManager = MauiApplication.Current.GetSystemService(Context.StorageService) as StorageManager;

            var volume = storageManager.StorageVolumes.ToList().FirstOrDefault(r => !r.IsPrimary);

            if (volume != null) return $@"/storage/{volume.Uuid}/";

            return string.Empty;
        }

        private string GetFullPathFromTreeUri(android.Net.Uri treeUri, string volumeBasePath)
        {
            if (treeUri == null)
            {
                return null;
            }
            if (volumeBasePath == null)
            {
                return Java.IO.File.Separator;
            }
            String volumePath = volumeBasePath;
            if (volumePath.EndsWith(Java.IO.File.Separator))
            {
                volumePath = volumePath.Substring(0, volumePath.Length - 1);
            }

            String documentPath = GetDocumentPathFromTreeUri(treeUri);
            if (documentPath.EndsWith(Java.IO.File.Separator))
            {
                documentPath = documentPath.Substring(0, documentPath.Length - 1);
            }

            if (documentPath.Length > 0)
            {
                if (documentPath.StartsWith(Java.IO.File.Separator))
                {
                    return volumePath + documentPath;
                }
                else
                {
                    return volumePath + Java.IO.File.Separator + documentPath;
                }
            }
            else
            {
                return volumePath;
            }
        }

        private String GetDocumentPathFromTreeUri(android.Net.Uri treeUri)
        {
            string docId = DocumentsContract.GetTreeDocumentId(treeUri);
            String[] split = docId.Split(":");
            if ((split.Length >= 2) && (split[1] != null))
            {
                return split[1];
            }
            else
            {
                return Java.IO.File.Separator;
            }
        }

        private class SDpermission
        {
            public UriPermission Permission { get; set; }
            public string Pathsegment { get; set; }
        }

        public override Task CopyArtifactsAsync(FsArtifact[] artifacts, string destination, CancellationToken? cancellationToken = null)
        {
            return base.CopyArtifactsAsync(artifacts, destination, cancellationToken);
        }

        public override async Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new DomainLogicException(StringLocalizer[nameof(AppStrings.PathIsNull)]);
            }

            if (!await TryGetPermissionAsync(path))
            {
                throw new NotImplementedException();
            }

            DocumentFile destinationDirectory = null;
            var fileDirectry = Path.GetDirectoryName(path);
            var fileName = Path.GetFileNameWithoutExtension(path);

            var provider = await GetFsFileProviderType(path);
            if (provider == FsFileProviderType.InternalMemory)
            {
                var file = new Java.IO.File(fileDirectry);
                destinationDirectory = DocumentFile.FromFile(file);
            }
            else if(provider == FsFileProviderType.ExternalMemory)
            {
                var filePath = Path.GetDirectoryName(path);
                destinationDirectory = GetDocumentFileIfAllowedToWrite(new Java.IO.File(filePath), MauiApplication.Context);
            }

            var mimeType = GetMimeTypeForFileExtension(path);
            DocumentFile destinationFile = destinationDirectory.CreateFile(mimeType, fileName);

            using var outStream = MauiApplication.Current.ContentResolver?.OpenOutputStream(destinationFile.Uri);
            await stream.CopyToAsync(outStream);

            var newFsArtifact = new FsArtifact
            {
                Name = fileName,
                FullPath = path,
                ArtifactType = FsArtifactType.File,
                FileExtension = Path.GetExtension(path),
                Size = (int)outStream.Length,
                ProviderType = await GetFsFileProviderType(path),
                LastModifiedDateTime = DateTimeOffset.Now
            };

            return newFsArtifact;
        }

        public override Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null)
        {
            return base.CreateFilesAsync(files, cancellationToken);
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

            await foreach (var artifact in base.GetArtifactsAsync(path, searchText, cancellationToken))
            {
                yield return artifact;
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

        public override async Task<FsFileProviderType> GetFsFileProviderType(string filePath)
        {
            var drives = await GetDrivesAsync();

            // ToDo: How to get it from the path
            if (IsFsFileProviderInternal(filePath, drives))
            {
                return FsFileProviderType.InternalMemory;
            }
            else if (IsFsFileProviderExternal(filePath, drives))
            {
                return FsFileProviderType.ExternalMemory;
            }
            else
                throw new Exception($"Unknown file provider for path: {filePath}");
        }

        private bool IsFsFileProviderInternal(string filePath, List<FsArtifact> drives)
        {
            var internalDrive = drives?.FirstOrDefault(d => d.ProviderType == FsFileProviderType.InternalMemory);
            if (string.IsNullOrWhiteSpace(filePath) || internalDrive?.FullPath == null)
            {
                return false;
            }
            else if (filePath.StartsWith(internalDrive.FullPath))
            {
                return true;
            }

            return false;
        }

        private bool IsFsFileProviderExternal(string filePath, List<FsArtifact> drives)
        {
            var externalDrives = drives?.Where(d => d.ProviderType == FsFileProviderType.ExternalMemory).ToList();
            if (string.IsNullOrWhiteSpace(filePath) || externalDrives is null || !externalDrives.Any())
            {
                return false;
            }

            foreach (var externalDrive in externalDrives)
            {
                if (externalDrive?.FullPath != null && filePath.StartsWith(externalDrive.FullPath))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
