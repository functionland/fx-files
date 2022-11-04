using Android.Content;
using Android.OS.Storage;
using Android.Provider;
using Android.Webkit;
using AndroidX.DocumentFile.Provider;
using Functionland.FxFiles.Client.App.Platforms.Android.PermissionsUtility;
using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Resources;
using Functionland.FxFiles.Client.Shared.Services.Implementations;
using Java.IO;
using Java.Nio.Channels;
using Microsoft.Extensions.Localization;
using Microsoft.Maui.Animations;
using System.Diagnostics;
using android = Android;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

public partial class Android10FileService : AndroidFileService
{
    private List<FsArtifact>? _allDrives = null;

    public override List<FsArtifact> GetDrives()
    {
        LazyInitializer.EnsureInitialized(ref _allDrives, LoadDrives);
        return _allDrives;
    }

    private List<FsArtifact> LoadDrives()
    {
        var storageManager = MauiApplication.Current.GetSystemService(Context.StorageService) as StorageManager;
        if (storageManager is null)
        {
            throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToLoadStorageManager));
        }

        var storageVolumes = storageManager.StorageVolumes.ToList();

        var drives = new List<FsArtifact>();
        foreach (var storage in storageVolumes)
        {

            if (storage is null || !string.Equals(storage.State, android.OS.Environment.MediaMounted, StringComparison.InvariantCultureIgnoreCase))
                continue;

            Java.IO.File storageDirectory;


            if (storage.IsPrimary)
            {
                storageDirectory = android.OS.Environment.ExternalStorageDirectory;
            }
            else
            {
                storageDirectory = new Java.IO.File($@"/storage/{storage.Uuid}");
            }


            var lastModifiedUnixFormat = storageDirectory.LastModified();
            var lastModifiedDateTime = lastModifiedUnixFormat == 0
                                                        ? DateTimeOffset.Now
                                                        : DateTimeOffset.FromUnixTimeMilliseconds(lastModifiedUnixFormat);
            var fullPath = storageDirectory.Path;
            var capacity = storageDirectory.FreeSpace;
            var size = storageDirectory.TotalSpace;

            if (storage.IsPrimary)
            {
                var internalFileName = StringLocalizer.GetString(AppStrings.InternalStorageName);
                drives.Add(new FsArtifact(fullPath, internalFileName, FsArtifactType.Drive, FsFileProviderType.InternalMemory)
                {
                    Capacity = capacity,
                    Size = size,
                    LastModifiedDateTime = lastModifiedDateTime,
                });
            }
            else
            {
                string sdCardName = storage.Uuid ?? string.Empty;

                var externalFileName = StringLocalizer.GetString(AppStrings.SDCardName, sdCardName);
                drives.Add(new FsArtifact(fullPath, externalFileName, FsArtifactType.Drive, FsFileProviderType.ExternalMemory)
                {
                    Capacity = capacity,
                    Size = size,
                    LastModifiedDateTime = lastModifiedDateTime,
                });
            }
        }

        return drives;
    }

    protected override async Task GetPermission(string path = null)
    {
        if (!await PermissionUtils.CheckStoragePermissionAsync(path))
        {
            await PermissionUtils.RequestStoragePermission(path);

            var StoragePermissionResult = await PermissionUtils.GetPermissionTask!.Task;
            if (!StoragePermissionResult || !await PermissionUtils.CheckStoragePermissionAsync(path))
            {
                throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
            }
        }
    }

    protected override async Task GetPermission(IEnumerable<string> paths = null)
    {
        if (paths == null || !paths.Any())
        {
            await GetPermission(String.Empty);
        }

        foreach (var path in paths)
        {
            await GetPermission(path);
        }
    }

    protected override void LocalStorageDeleteFile(string path)
    {
        Delete(path);
    }

    protected override void LocalStorageDeleteDirectory(string path)
    {
        Delete(path);
    }

    protected override void LocalStorageCreateDirectory(string newPath)
    {
        var path = Directory.GetParent(newPath).FullName;
        var directoryName = System.IO.Path.GetFileName(newPath);
        var documentFile = GetDocumentFileIfAllowedToWrite(path);
        using var newdirectory = documentFile.CreateDirectory(directoryName);

        if (newdirectory is null)
        {
            throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
        }
    }

    protected override void LocalStorageRenameFile(string filePath, string newPath)
    {
        var documentFile = GetDocumentFileIfAllowedToWrite(filePath);
        var newName = Path.GetFileName(newPath);
        var canRename = documentFile.RenameTo(newName);

        if (!canRename)
        {
            throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
        }
    }

    protected override void LocalStorageRenameDirectory(string folderPath, string newPath)
    {
        var documentFile = GetDocumentFileIfAllowedToWrite(folderPath);
        var newName = Path.GetFileName(newPath);
        var canRename = documentFile.RenameTo(newName);
        
        if (!canRename)
        {
            throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
        }
    }

    protected override async Task LocalStorageCreateFile(string path, Stream stream)
    {
        var parent = Directory.GetParent(path).FullName;
        var filename = System.IO.Path.GetFileNameWithoutExtension(path);
        var fileExtention = Path.GetExtension(path);
        var mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(fileExtention.ToLower().TrimStart('.'));
        var documentFile = GetDocumentFileIfAllowedToWrite(parent);

        if (System.IO.File.Exists(path))
        {
            LocalStorageDeleteFile(path);
        }

        using var newfile = documentFile.CreateFile(mimeType, filename);

        if (newfile is null)
        {
            throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
        }

        using var outStream = MauiApplication.Context.ContentResolver.OpenOutputStream(newfile.Uri);

        stream.CopyTo(outStream);

    }

    protected override void LocalStorageMoveFile(string filePath, string newPath)
    {
        LocalStorageCopyFile(filePath, newPath);
        LocalStorageDeleteFile(filePath);
    }

    protected override async void LocalStorageCopyFile(string sourceFile, string destinationFile)
    {
        try
        {
            using var inStream = System.IO.File.OpenRead(sourceFile);
            await LocalStorageCreateFile(destinationFile, inStream);
        }
        catch (Exception ex)
        {
            ExceptionHandler.Handle(ex);
            throw;
        }
        
    }

    private void Delete(string path)
    {
        var documentFile = GetDocumentFileIfAllowedToWrite(path);

        if (documentFile is null)
        {
            throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
        }

        var canDelete = documentFile.Delete();

        if (!canDelete)
        {
            throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
        }
    }

    private DocumentFile GetDocumentFileIfAllowedToWrite(string path)
    {
        var file = new Java.IO.File(path);
        
        if (IsInternalFilePath(path))
        {
            return DocumentFile.FromFile(file);
        }

        List<UriPermission> permissionUris = MauiApplication.Current.ContentResolver.PersistedUriPermissions.ToList();

        foreach (UriPermission permissionUri in permissionUris)
        {
            var treeUri = permissionUri.Uri;
            DocumentFile rootDocFile = DocumentFile.FromTreeUri(MauiApplication.Current, treeUri);

            string rootDocFilePath = GetFullPathFromTreeUri(treeUri, GetSDCardPath(path));

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
                    docFile = DocumentFile.FromTreeUri(MauiApplication.Current, rootDocFile.Uri);
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
                    throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
                }

            }
        }

        throw new UnableAccessToStorageException(StringLocalizer.GetString(AppStrings.UnableToAccessToStorage));
    }
    private string GetSDCardPath(string filePath)
    {
        var sdcardname = filePath.Split('/')[2];
        return $@"/storage/{sdcardname}/";
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
                return Path.Combine(volumePath, documentPath);
            }
        }
        else
        {
            return volumePath;
        }
    }

    private bool IsInternalFilePath(string filepath)
    {
        var storage = android.OS.Environment.ExternalStorageDirectory;
        return filepath.StartsWith(storage.AbsolutePath);
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
}
