﻿using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Extensions;
using Functionland.FxFiles.Client.Shared.Utils;

using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class ZipService : IZipService
{
    [AutoInject] public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

    [AutoInject] public ILocalDeviceFileService LocalDeviceFileService { get; set; } = default!;

    public virtual async Task<List<FsArtifact>> GetAllArtifactsAsync(
        string zipFilePath,
        string? password = null,
        CancellationToken? cancellationToken = null)
    {
        var extension = Path.GetExtension(zipFilePath).ToLower();
        try
        {
            var artifacts = extension switch
            {
                //".rar" => await GetRarArtifactsAsync(zipFilePath, password),
                ".zip" => await GetZipArtifactsAsync(zipFilePath, password, cancellationToken),
                _ => throw new InvalidZipExtensionException(StringLocalizer.GetString(nameof(AppStrings.InvalidZipExtensionException), extension))
            };

            return artifacts;
        }
        catch (InvalidFormatException ex) when (ex.Message.StartsWith("Unknown Rar Header:"))
        {
            throw new NotSupportedEncryptedFileException(StringLocalizer.GetString(AppStrings.NotSupportedEncryptedFileException));
        }
        catch (CryptographicException ex) when (ex.Message == "No password supplied for encrypted zip.")
        {
            throw new NotSupportedEncryptedFileException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch (Exception ex) when (ex.Message == "bad password")
        {
            throw new NotSupportedEncryptedFileException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch (CryptographicException ex) when (ex.Message == "Encrypted Rar archive has no password specified.")
        {
            throw new NotSupportedEncryptedFileException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch (FormatException ex) when (ex.Message == "malformed vint")
        {
            throw new NotSupportedEncryptedFileException(StringLocalizer.GetString(AppStrings.NotSupportedEncryptedFileException));
        }
        catch (OverflowException ex) when (ex.Message == "Arithmetic operation resulted in an overflow.")
        {
            throw new NotSupportedEncryptedFileException(StringLocalizer.GetString(AppStrings.NotSupportedEncryptedFileException));
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException(StringLocalizer.GetString(AppStrings.FileNotFoundException));
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
        var newPath = Path.Combine(destinationPath, destinationFolderName);
        var zipFileExtension = Path.GetExtension(zipFullPath).ToLower();

        if (!Directory.Exists(newPath))
        {
            Directory.CreateDirectory(newPath);
        }

        try
        {
            return zipFileExtension switch
            {
                ".zip" => await ExtractZipItemsAsync(zipFullPath, newPath, fsArtifacts, password, overwrite, onProgress, cancellationToken),
                //".rar" => await ExtractRarItemsAsync(zipFullPath, newPath, fsArtifacts, password, overwrite, onProgress, cancellationToken),
                _ => throw new InvalidZipExtensionException(StringLocalizer.GetString(nameof(AppStrings.InvalidZipExtensionException), zipFileExtension)),
            };
        }
        catch (IOException ex) when (ex.Message.EndsWith("because a file or directory with the same name already exists."))
        {
            if (Directory.Exists(newPath))
            {
                DirectoryUtils.HardDeleteDirectory(newPath);
            }
            var lowerCaseArtifact = StringLocalizer[nameof(AppStrings.Artifact)].Value.ToLowerFirstChar();
            throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException));
        }
        catch (CryptographicException ex) when (ex.Message == "Encrypted Rar archive has no password specified.")
        {
            await DeleteExtractedZipEmptyFiles(zipFullPath, newPath);
            throw new InvalidPasswordException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch (CryptographicException ex) when (ex.Message == "The password did not match.")
        {
            await DeleteExtractedZipEmptyFiles(zipFullPath, newPath);
            throw new PasswordDidNotMatchedException(StringLocalizer.GetString(AppStrings.PasswordDidNotMatchedException));
        }
        catch (InvalidFormatException ex) when (ex.Message.StartsWith("Unknown Rar Header:"))
        {
            await DeleteExtractedZipEmptyFiles(zipFullPath, newPath);
            throw new NotSupportedEncryptedFileException(StringLocalizer.GetString(AppStrings.NotSupportedEncryptedFileException));
        }
        catch (CryptographicException ex) when (ex.Message == "No password supplied for encrypted zip.")
        {
            await DeleteExtractedZipEmptyFiles(zipFullPath, newPath);
            throw new InvalidPasswordException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch (Exception ex) when (ex.Message == "bad password")
        {
            await DeleteExtractedZipEmptyFiles(zipFullPath, newPath);
            throw new PasswordDidNotMatchedException(StringLocalizer.GetString(AppStrings.PasswordDidNotMatchedException));
        }
        catch (FormatException ex) when (ex.Message == "malformed vint")
        {
            throw new NotSupportedEncryptedFileException(StringLocalizer.GetString(AppStrings.NotSupportedEncryptedFileException));
        }
        catch (OverflowException ex) when (ex.Message == "Arithmetic operation resulted in an overflow.")
        {
            throw new NotSupportedEncryptedFileException(StringLocalizer.GetString(AppStrings.NotSupportedEncryptedFileException));
        }
        catch (NotSupportedException)
        {
            throw new NotSupportedEncryptedFileException(StringLocalizer.GetString(AppStrings.NotSupportedEncryptedFileException));
        }
    }

    private async Task DeleteExtractedZipEmptyFiles(string zipFilePath, string destinationPath)
    {
        var zipFileArtifacts = (await GetAllArtifactsAsync(zipFilePath)).Where(a => a.ArtifactType == FsArtifactType.File);
        var emptyFiles = new List<FsArtifact>();

        foreach (var zipFileArtifact in zipFileArtifacts)
        {
            var artifactPath = Path.Combine(destinationPath, zipFileArtifact.FullPath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(artifactPath))
            {
                var artifact = await LocalDeviceFileService.GetArtifactAsync(artifactPath);

                if (artifact.Size == 0)
                {
                    emptyFiles.Add(artifact);
                }
            }
        }

        if (!emptyFiles.Any()) return;

        foreach (var emptyFile in emptyFiles)
        {
            File.Delete(emptyFile.FullPath);
        }
    }

    private async Task<List<FsArtifact>> GetRarArtifactsAsync(string zipFilePath, string? password = null, CancellationToken? cancellationToken = null)
    {
        var artifact = await LocalDeviceFileService.GetArtifactAsync(zipFilePath);

        if (artifact is null)
        {
            throw new FileNotFoundException(StringLocalizer.GetString(AppStrings.FileNotFoundException));
        }

        var providerType = artifact.ProviderType;

        var artifacts = new List<FsArtifact>();
        using var archive = RarArchive.Open(zipFilePath, new ReaderOptions() { Password = password });

        var entries = archive.Entries.ToList();
        foreach (var entry in entries)
        {
            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                return artifacts;

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

        FillRemainedArtifacts(providerType, artifacts, ArchiveType.Rar, cancellationToken: cancellationToken);

        return artifacts;
    }

    private async Task<List<FsArtifact>> GetZipArtifactsAsync(string zipFilePath, string? password = null, CancellationToken? cancellationToken = null)
    {
        var artifact = await LocalDeviceFileService.GetArtifactAsync(zipFilePath);

        if (artifact is null)
        {
            throw new FileNotFoundException(StringLocalizer.GetString(AppStrings.FileNotFoundException));
        }

        var providerType = artifact.ProviderType;

        var artifacts = new List<FsArtifact>();
        using var archive = ZipArchive.Open(zipFilePath, new ReaderOptions() { Password = password });

        var entries = archive.Entries.ToList();
        var c = 0;
        foreach (var entry in entries)
        {
            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                return artifacts;

            c++;
            var artifactType = entry.IsDirectory ? FsArtifactType.Folder : FsArtifactType.File;
            var path = entry.Key.TrimEnd('/');

            var parentPath = Path.GetDirectoryName(path)?.Replace(Path.DirectorySeparatorChar.ToString(), "/");

            var entryFileName = Path.GetFileName(path);
            var newFsArtifact = new FsArtifact(path, entryFileName, artifactType, providerType)
            {
                FileExtension = !entry.IsDirectory ? Path.GetExtension(path) : null,
                LastModifiedDateTime = entry.LastModifiedTime ?? DateTimeOffset.Now,
                ParentFullPath = parentPath
            };

            artifacts.Add(newFsArtifact);
        }

        FillRemainedArtifacts(providerType, artifacts, ArchiveType.Zip, cancellationToken);

        return artifacts;
    }

    private async Task<int> ExtractRarItemsAsync(
        string fullPath,
        string destinationPath,
        IEnumerable<FsArtifact>? artifacts,
        string? password,
        bool overwrite,
        Func<ProgressInfo, Task>? onProgress,
        CancellationToken? cancellationToken)
    {
        var allEntriesCount = 0;
        var duplicateCount = 0;
        var archiveType = Path.GetExtension(fullPath) == ".zip" ? ArchiveType.Zip : ArchiveType.Rar;
        using var archive = RarArchive.Open(fullPath, new ReaderOptions() { Password = password });

        var fsArtifacts = artifacts as FsArtifact[] ?? artifacts?.ToArray();
        if (artifacts is null)
        {
            var keys = archive.Entries.Select(c => c.Key).ToList();

            var correctPaths = keys.Select(k => k.Replace("/", Path.DirectorySeparatorChar.ToString()));
            var remainedEntries = GetRemainedEntries(correctPaths, archiveType);
            allEntriesCount = archive.Entries.Count + remainedEntries.Count;
        }
        else
        {
            if (fsArtifacts != null)
            {
                foreach (var item in fsArtifacts)
                {
                    if (cancellationToken is { IsCancellationRequested: true })
                        return 0;

                    var keys = archive.Entries.Where(c => c.Key.StartsWith(item.FullPath)).Select(c => c.Key).ToList();

                    var correctPaths = keys.Select(k => k.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    var remainedEntries = GetRemainedEntries(correctPaths, archiveType);
                    allEntriesCount += archive.Entries.Count + remainedEntries.Count;
                }
            }
        }

        var entries = archive.Entries;

        if (artifacts is null)
        {
            return await ExtractRarAsync(entries, allEntriesCount, destinationPath, null, overwrite, onProgress,
                cancellationToken);
        }

        if (fsArtifacts != null)
        {
            foreach (var item in fsArtifacts)
            {
                if (cancellationToken is { IsCancellationRequested: true })
                    return 0;

                duplicateCount += await ExtractRarAsync(entries, allEntriesCount, destinationPath, item.FullPath,
                    overwrite, onProgress, cancellationToken);
            }
        }

        return duplicateCount;
    }

    private async Task<int> ExtractZipItemsAsync(
        string fullPath,
        string destinationPath,
        IEnumerable<FsArtifact>? artifacts,
        string? password = null,
        bool overwrite = false,
        Func<ProgressInfo, Task>? onProgress = null,
        CancellationToken? cancellationToken = null)
    {
        var allEntriesCount = 0;
        var duplicateCount = 0;
        var archiveType = Path.GetExtension(fullPath) == ".zip" ? ArchiveType.Zip : ArchiveType.Rar;

        using var archive = ZipArchive.Open(fullPath, new ReaderOptions() { Password = password });

        var fsArtifacts = artifacts as FsArtifact[] ?? artifacts?.ToArray();
        if (artifacts is null)
        {
            var keys = archive.Entries.Select(c => c.Key).ToList();
            var correctPaths = keys.Select(k => k.Replace("/", Path.AltDirectorySeparatorChar.ToString()));
            var remainedEntries = GetRemainedEntries(correctPaths, archiveType);
            allEntriesCount = archive.Entries.Count + remainedEntries.Count;
        }
        else
        {
            if (fsArtifacts != null)
            {
                foreach (var item in fsArtifacts)
                {
                    if (cancellationToken is { IsCancellationRequested: true })
                        return 0;

                    var keys = archive.Entries.Where(c => c.Key.StartsWith(item.FullPath)).Select(c => c.Key).ToList();
                    var correctPaths = keys.Select(k => k.Replace("/", Path.AltDirectorySeparatorChar.ToString()));
                    var remainedEntries = GetRemainedEntries(correctPaths, archiveType);
                    allEntriesCount += archive.Entries.Count + remainedEntries.Count;
                }
            }
        }

        var entries = archive.Entries;

        if (artifacts is null)
            return await ExtractZipAsync(entries, allEntriesCount, destinationPath, null, overwrite, onProgress, cancellationToken);

        if (fsArtifacts != null)
        {
            foreach (var item in fsArtifacts)
            {
                var itemFullPath = Path.Combine(destinationPath, item.Name);
                if (File.Exists(itemFullPath))
                    duplicateCount++;

                if (cancellationToken is { IsCancellationRequested: true })
                    return 0;

                duplicateCount += await ExtractZipAsync(entries, allEntriesCount, destinationPath, item.FullPath,
                    overwrite, onProgress, cancellationToken);
            }
        }

        return duplicateCount;
    }


    private async Task<int> ExtractZipAsync(
       ICollection<ZipArchiveEntry> entries,
       int allEntriesCount,
       string destinationPath,
       string? itemPath = null,
       bool overwrite = false,
       Func<ProgressInfo, Task>? onProgress = null,
       CancellationToken? cancellationToken = null)
    {
        double progressCount = 0;
        var duplicateCount = 0;

        if (itemPath is not null)
        {
            entries = entries.Where(c => c.Key.StartsWith(itemPath)).ToList();
            if (!entries.Any())
            {
                if (cancellationToken is { IsCancellationRequested: true })
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
            var keyName = Path.GetFileName(entry.Key.TrimEnd('/'));
            if (cancellationToken is { IsCancellationRequested: true })
                return 0;

            if (onProgress is not null)
            {
                progressCount = await FsArtifactUtils.HandleProgressBarAsync(keyName, allEntriesCount, progressCount, onProgress);
            }

            try
            {
                entry.WriteToDirectory(destinationPath, new ExtractionOptions()
                {
                    ExtractFullPath = true,
                    Overwrite = overwrite
                });
            }
            catch (IOException ex) when (ex.Message.StartsWith("The file") && ex.Message.EndsWith("already exists."))
            {
                duplicateCount++;
                continue;
            }

            if (onProgress is not null)
            {
                progressCount = await FsArtifactUtils.HandleProgressBarAsync(keyName, allEntriesCount, progressCount, onProgress);
            }
        }


        if (itemPath is null)
            return duplicateCount;

        var entryFullPath = itemPath.Replace("/", Path.AltDirectorySeparatorChar.ToString());

        var splitEntryFullPath = entryFullPath.Split(Path.AltDirectorySeparatorChar.ToString());

        if (splitEntryFullPath.Length == 1)
            return duplicateCount;

        MoveExtractedFileToFinalDestination(destinationPath, entryFullPath);

        return duplicateCount;
    }



    private async Task<int> ExtractRarAsync(
       ICollection<RarArchiveEntry> entries,
       int allEntriesCount,
       string destinationPath,
       string? itemPath = null,
       bool overwrite = false,
       Func<ProgressInfo, Task>? onProgress = null,
       CancellationToken? cancellationToken = null)
    {
        double progressCount = 0;
        var duplicateCount = 0;

        if (itemPath is not null)
        {
            entries = entries.Where(c => c.Key.StartsWith(itemPath)).ToList();
            if (!entries.Any())
            {
                if (cancellationToken is { IsCancellationRequested: true })
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
            var keyName = Path.GetFileName(entry.Key);
            if (cancellationToken is { IsCancellationRequested: true })
                return 0;

            if (onProgress is not null)
            {
                progressCount = await FsArtifactUtils.HandleProgressBarAsync(keyName, allEntriesCount, progressCount, onProgress);
            }

            try
            {
                entry.WriteToDirectory(destinationPath, new ExtractionOptions()
                {
                    ExtractFullPath = true,
                    Overwrite = overwrite
                });
            }
            catch (IOException ex) when (ex.Message.StartsWith("The file") && ex.Message.EndsWith("already exists."))
            {
                duplicateCount++;
                continue;
            }

            if (onProgress is not null)
            {
                progressCount = await FsArtifactUtils.HandleProgressBarAsync(keyName, allEntriesCount, progressCount, onProgress);
            }
        }

        if (itemPath is not null)
        {
            var entryFullPath = itemPath.Replace("/", Path.DirectorySeparatorChar.ToString());
            MoveExtractedFileToFinalDestination(destinationPath, entryFullPath);
        }

        return duplicateCount;
    }


    private static void MoveExtractedFileToFinalDestination(string destinationPath, string extractedItemPath)
    {
        var extenstion = Path.GetExtension(extractedItemPath);
        var fileName = Path.GetFileName(extractedItemPath);

        if (Path.GetDirectoryName(extractedItemPath) == destinationPath)
            return;

        var extractedFinalPath = Path.Combine(destinationPath, extractedItemPath);
        var destinationFinalPath = Path.Combine(destinationPath, fileName);

        if (string.IsNullOrWhiteSpace(extenstion))
        {
            Directory.Move(extractedFinalPath, destinationFinalPath);
        }
        else
        {
            File.Move(extractedFinalPath, destinationFinalPath, true);
        }

        var parentFinalPath = extractedFinalPath;

        do
        {
            var parentPath = Path.GetDirectoryName(parentFinalPath);
            if (destinationPath == parentPath)
                break;
            parentFinalPath = parentPath;
        } while (true);

        if (parentFinalPath is not null)
        {
            DirectoryUtils.HardDeleteDirectory(parentFinalPath);
        }
    }

    private static List<string> GetRemainedEntries(IEnumerable<string> filesPath, ArchiveType archiveType)
    {
        var result = new List<string>();

        var filePaths = filesPath as string[] ?? filesPath.ToArray();
        foreach (var filePath in filePaths)
        {
            var path = filePath;
            while (true)
            {
                var parentFilePath = archiveType switch
                {
                    ArchiveType.Rar => Path.GetDirectoryName(path)?.Replace("/", Path.DirectorySeparatorChar.ToString()),
                    _ => Path.GetDirectoryName(path)?.Replace("\\", Path.AltDirectorySeparatorChar.ToString())
                };

                if (string.IsNullOrWhiteSpace(parentFilePath))
                    break;

                path = parentFilePath;

                if (filePaths.Contains(parentFilePath) || result.Contains(parentFilePath))
                    continue;

                result.Add(parentFilePath);
            }
        }

        return result;
    }

    private void FillRemainedArtifacts(FsFileProviderType providerType, ICollection<FsArtifact> artifacts, ArchiveType archiveType, CancellationToken? cancellationToken)
    {
        var remainedEntries = GetRemainedEntries(artifacts.Select(c =>
                    archiveType switch
                    {
                        ArchiveType.Rar => c.FullPath.Replace("/", Path.DirectorySeparatorChar.ToString()),
                        _ => c.FullPath.Replace("/", Path.AltDirectorySeparatorChar.ToString())
                    }
                ), archiveType
            );

        foreach (var remainedEntry in remainedEntries)
        {
            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                return;

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

public enum ArchiveType
{
    Zip,
    Rar
}