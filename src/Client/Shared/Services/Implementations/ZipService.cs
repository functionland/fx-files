using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Extensions;
using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Utils;
using Microsoft.VisualBasic;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class ZipService : IZipService
{
    [AutoInject] public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

    [AutoInject] public ILocalDeviceFileService LocalDeviceFileService { get; set; }


    public virtual async Task<List<FsArtifact>> GetAllArtifactsAsync(
        string zipFilePath,
        string? password = null,
        CancellationToken? cancellationToken = null)
    {
        var extension = Path.GetExtension(zipFilePath);
        try
        {
            var artifacts = extension switch
            {
                ".rar" => await GetRarArtifactsAsync(zipFilePath, password),
                ".zip" => await GetZipArtifactsAsync(zipFilePath, password),
                // ToDo: move string to resources.
                _ => throw new InvalidOperationException($"Zip file not supported: {extension}")
            };

            return artifacts;
        }
        catch (InvalidFormatException ex) when (ex.Message.StartsWith("Unknown Rar Header:"))
        {
            throw new PasswordDidNotMatchedException(StringLocalizer.GetString(AppStrings.PasswordDidNotMatchedException));
        }
        catch (CryptographicException ex) when (ex.Message == "No password supplied for encrypted zip.")
        {
            throw new InvalidPasswordException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch (Exception ex) when (ex.Message == "bad password")
        {
            throw new InvalidPasswordException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch (CryptographicException ex) when (ex.Message == "Encrypted Rar archive has no password specified.")
        {
            throw new InvalidPasswordException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch
        {
            throw new DomainLogicException(StringLocalizer.GetString(AppStrings.TheOpreationFailedMessage));
        }
    }

    public virtual async Task<int> ExtractZippedArtifactAsync(
        string zipFullPath,
        string destinationPath,
        string destinationFolderName,
        IEnumerable<FsArtifact>? fsArtifacts = null,
        bool overwrite = false,
        string? password = null,
        Func<ProgressInfo, Task>? onProgress = null,
        CancellationToken? cancellationToken = null)
    {
        var duplicateCount = 0;
        var newPath = Path.Combine(destinationPath, destinationFolderName);
        var zipFileExtension = Path.GetExtension(zipFullPath);

        if (!Directory.Exists(newPath))
        {
            Directory.CreateDirectory(newPath);
        }

        try
        {
            if (fsArtifacts is null || !fsArtifacts.Any())
            {
                return zipFileExtension switch
                {
                    ".zip" => await ExtractZipAsync(zipFullPath, newPath, null, password, overwrite, onProgress, cancellationToken),
                    ".rar" => await ExtractRarAsync(zipFullPath, newPath, null, password, overwrite, onProgress, cancellationToken),
                    _ => throw new InvalidZipExtensionException(StringLocalizer.GetString(nameof(AppStrings.InvalidZipExtensionException), zipFileExtension)),
                };
            }

            switch (zipFileExtension)
            {
                case ".zip":
                    {
                        foreach (var fsArtifact in fsArtifacts)
                        {
                            duplicateCount += await ExtractZipAsync(zipFullPath, newPath, fsArtifact.FullPath, password, overwrite, onProgress, cancellationToken);
                        }

                        break;
                    }

                case ".rar":
                    {
                        foreach (var fsArtifact in fsArtifacts)
                        {
                            duplicateCount += await ExtractRarAsync(zipFullPath, newPath, fsArtifact.FullPath, password, overwrite, onProgress, cancellationToken);
                        }
                        break;
                    }

                default:
                    throw new InvalidZipExtensionException(StringLocalizer.GetString(nameof(AppStrings.InvalidZipExtensionException), zipFileExtension));
            }



            return duplicateCount;

        }
        catch (IOException ex) when (ex.Message.EndsWith("because a file or directory with the same name already exists."))
        {
            if (Directory.Exists(newPath))
            {
                Directory.Delete(newPath, true);
            }
            var lowerCaseArtifact = StringLocalizer[nameof(AppStrings.Artifact)].Value.ToLowerFirstChar();
            throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, lowerCaseArtifact));
        }
        catch (CryptographicException ex) when (ex.Message == "Encrypted Rar archive has no password specified.")
        {
            throw new InvalidPasswordException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch (CryptographicException ex) when (ex.Message == "The password did not match.")
        {
            if (Directory.Exists(newPath))
            {
                Directory.Delete(newPath, true);
            }
            throw new PasswordDidNotMatchedException(StringLocalizer.GetString(AppStrings.PasswordDidNotMatchedException));
        }
        catch (InvalidFormatException ex) when (ex.Message.StartsWith("Unknown Rar Header:"))
        {
            throw new PasswordDidNotMatchedException(StringLocalizer.GetString(AppStrings.PasswordDidNotMatchedException));
        }
        catch (CryptographicException ex) when (ex.Message == "No password supplied for encrypted zip.")
        {
            if (Directory.Exists(newPath))
            {
                Directory.Delete(newPath, true);
            }
            throw new InvalidPasswordException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch (Exception ex) when (ex.Message == "bad password")
        {
            if (Directory.Exists(newPath))
            {
                Directory.Delete(newPath, true);
            }
            throw new InvalidPasswordException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
    }

    private async Task<List<FsArtifact>> GetRarArtifactsAsync(string zipFilePath, string? password = null)
    {
        var artifact = await LocalDeviceFileService.GetArtifactAsync(zipFilePath);
        var providerType = artifact.ProviderType;

        var artifacts = new List<FsArtifact>();
        using var archive = RarArchive.Open(zipFilePath, new ReaderOptions() { Password = password });

        var entries = archive.Entries.ToList();
        foreach (var entry in entries)
        {
            var newPath = entry.Key;
            var parentPath = Path.GetDirectoryName(newPath);
            var artifactType = entry.IsDirectory ? FsArtifactType.Folder : FsArtifactType.File;

            var entryFileName = Path.GetFileName(newPath);
            var newArtifact = new FsArtifact(newPath, entryFileName, artifactType, providerType)
            {
                FileExtension = !entry.IsDirectory ? Path.GetExtension(newPath) : null,
                LastModifiedDateTime = entry.LastModifiedTime ?? DateTimeOffset.Now,
                ParentFullPath = parentPath
            };

            artifacts.Add(newArtifact);
        }

        FillRemainedArtifacts(providerType, artifacts);

        return artifacts;
    }

    private async Task<List<FsArtifact>> GetZipArtifactsAsync(string zipFilePath, string? password = null)
    {
        var artifact = await LocalDeviceFileService.GetArtifactAsync(zipFilePath);
        var providerType = artifact.ProviderType;

        var artifacts = new List<FsArtifact>();
        using var archive = ZipArchive.Open(zipFilePath, new ReaderOptions() { Password = password });

        var entries = archive.Entries.ToList();
        foreach (var entry in entries)
        {
            var artifactType = entry.IsDirectory ? FsArtifactType.Folder : FsArtifactType.File;
            var path = entry.Key.TrimEnd('/');

            var parentPath = Path.GetDirectoryName(path);

            var entryFileName = Path.GetFileName(path);
            var newFsArtifact = new FsArtifact(path, entryFileName, artifactType, providerType)
            {
                FileExtension = !entry.IsDirectory ? Path.GetExtension(path) : null,
                LastModifiedDateTime = entry.LastModifiedTime ?? DateTimeOffset.Now,
                ParentFullPath = parentPath
            };

            artifacts.Add(newFsArtifact);
        }

        FillRemainedArtifacts(providerType, artifacts);

        return artifacts;
    }

    private static async Task<int> ExtractRarAsync(
        string fullPath,
        string destinationPath,
        string? itemPath,
        string? password,
        bool overwrite,
        Func<ProgressInfo, Task>? onProgress,
        CancellationToken? cancellationToken)
    {
        int? progressCount = null;
        var duplicateCount = 0;
        using var archive = RarArchive.Open(fullPath, new ReaderOptions() { Password = password });
        var entries = archive.Entries;

        if (itemPath is not null)
        {
            entries = entries.Where(c => c.Key.StartsWith(itemPath)).ToList();
            if (!entries.Any())
            {
                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                    return 0;

                var folderName = Path.GetFileNameWithoutExtension(itemPath);
                var finalPath = Path.Combine(destinationPath, folderName);
                if (!Directory.Exists(finalPath))
                {
                    Directory.CreateDirectory(finalPath);
                }

                return 0;
            }
        }

        foreach (var entry in entries)
        {
            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
            {
                return 0;
            }
            if (progressCount is null && onProgress is not null)
            {
                progressCount = await FsArtifactUtils.HandleProgressBarAsync(entry.Key, entries.Count, progressCount, onProgress);
            }
            try
            {
                entry.WriteToDirectory(destinationPath, new ExtractionOptions()
                {
                    ExtractFullPath = true,
                    Overwrite = overwrite
                });

                if (itemPath is not null)
                {
                    var entryFullPath = GetRarFullPath(destinationPath, entry.Key.TrimEnd('\\'));
                    MoveExtractedFileToFinalDestination(destinationPath, entryFullPath);
                }
            }
            catch (IOException ex) when (ex.Message.StartsWith("The file") && ex.Message.EndsWith("already exists."))
            {
                duplicateCount++;
                continue;
            }

            if (onProgress is not null)
            {
                progressCount = await FsArtifactUtils.HandleProgressBarAsync(entry.Key, entries.Count, progressCount, onProgress);
            }
        }

        return duplicateCount;
    }

    private static async Task<int> ExtractZipAsync(
        string fullPath,
        string destinationPath,
        string? itemPath = null,
        string? password = null,
        bool overwrite = false,
        Func<ProgressInfo, Task>? onProgress = null,
        CancellationToken? cancellationToken = null)
    {
        int? progressCount = null;
        var duplicateCount = 0;
        using var archive = ZipArchive.Open(fullPath, new ReaderOptions() { Password = password });

        var entries = archive.Entries;

        if (itemPath is not null)
        {
            entries = entries.Where(c => c.Key.StartsWith(itemPath)).ToList();
            if (!entries.Any())
            {
                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                    return 0;

                var folderName = Path.GetFileNameWithoutExtension(itemPath);
                var finalPath = Path.Combine(destinationPath, folderName);
                if (!Directory.Exists(finalPath))
                {
                    Directory.CreateDirectory(finalPath);
                }

                return 0;
            }
        }

        foreach (var entry in entries)
        {
            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested) return 0;

            if (progressCount is null && onProgress is not null)
            {
                progressCount = await FsArtifactUtils.HandleProgressBarAsync(entry.Key, entries.Count, progressCount, onProgress);
            }

            try
            {
                entry.WriteToDirectory(destinationPath, new ExtractionOptions()
                {
                    ExtractFullPath = true,
                    Overwrite = overwrite
                });

                if (itemPath is not null)
                {
                    var entryFullPath = GetZipFullPath(destinationPath, entry.Key.TrimEnd('/'));
                    MoveExtractedFileToFinalDestination(destinationPath, entryFullPath);
                }
            }
            catch (IOException ex) when (ex.Message.StartsWith("The file") && ex.Message.EndsWith("already exists."))
            {
                duplicateCount++;
                continue;
            }

            if (onProgress is not null)
            {
                progressCount = await FsArtifactUtils.HandleProgressBarAsync(entry.Key, entries.Count, progressCount, onProgress);
            }
        }

        return duplicateCount;
    }

    private static void MoveExtractedFileToFinalDestination(string destinationPath, string extractedItemPath)
    {
        var fileAttribute = File.GetAttributes(extractedItemPath);

        if (fileAttribute.HasFlag(FileAttributes.Directory))
        {
            Directory.Move(extractedItemPath, destinationPath);
        }
        else
        {
            File.Move(extractedItemPath, destinationPath);
        }

        var remainedPath = extractedItemPath;
        while (true)
        {
            var parentPath = Path.GetDirectoryName(remainedPath);
            if (parentPath is null || parentPath.Equals(destinationPath))
                break;

            remainedPath = parentPath;
            Directory.Delete(parentPath, true);
        }
    }
    private static string GetZipFullPath(string destinationPath, string itemPath)
    {

#if WINDOWS
        itemPath = itemPath.Replace("/", "\\");
#endif

        var filePath = Path.Combine(destinationPath, itemPath);
        return filePath;
    }

    private static string GetRarFullPath(string destinationPath, string itemPath)
    {

#if ANDROID || IOS || Mac
        itemPath = itemPath.Replace("\", "/");
#endif

        var filePath = Path.Combine(destinationPath, itemPath);
        return filePath;
    }


    private static List<string> GetRemainedEntries(IEnumerable<string> filesPath)
    {
        var result = new List<string>();

        foreach (var filePath in filesPath)
        {
            var path = filePath;

            while (true)
            {
                var parentFilePath = Path.GetDirectoryName(path);

                if (string.IsNullOrWhiteSpace(parentFilePath)) break;

                path = parentFilePath;

                if (filesPath.Contains(parentFilePath) || result.Contains(parentFilePath)) continue;

                result.Add(parentFilePath);
            }
        }

        return result;
    }

    private static void FillRemainedArtifacts(FsFileProviderType providerType, List<FsArtifact> artifacts)
    {
        var remainedEntries = GetRemainedEntries(artifacts.Select(c => c.FullPath));

        foreach (var remainedEntry in remainedEntries)
        {
            var entryFileName = Path.GetFileName(remainedEntry);
            var newArtifact = new FsArtifact(remainedEntry, entryFileName, FsArtifactType.Folder, providerType)
            {
                FileExtension = null,
                ParentFullPath = Path.GetDirectoryName(remainedEntry)
            };

            artifacts.Add(newArtifact);
        }
    }

}