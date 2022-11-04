using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Extensions;
using Functionland.FxFiles.Client.Shared.Models;
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

    [AutoInject] public ILocalDeviceFileService LocalDeviceFileService { get; set; }


    public virtual async Task<List<FsArtifact>> GetAllArtifactsAsync(string zipFilePath, string? password = null, CancellationToken? cancellationToken = null)
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

    public virtual async Task<int> ExtractZippedArtifactAsync(string zipFullPath,
                                                   string destinationPath,
                                                   string destinationFolderName,
                                                   string? itemPath = null,
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
            if (zipFileExtension == ".zip")
            {
                if (!string.IsNullOrWhiteSpace(itemPath))
                {
                    duplicateCount = await ExtractZipArtifactAsync(zipFullPath, newPath, itemPath, overwrite, password, onProgress, cancellationToken);
                }
                else
                {
                    duplicateCount = await ExtractZipAsync(zipFullPath, newPath, password, overwrite, onProgress, cancellationToken);
                }
            }
            else if (zipFileExtension == ".rar")
            {
                if (!string.IsNullOrWhiteSpace(itemPath))
                {
                    duplicateCount = await ExtractRarArtifactAsync(zipFullPath, newPath, itemPath, overwrite, password, onProgress, cancellationToken);
                }
                else
                {
                    duplicateCount = await ExtractRarAsync(zipFullPath, newPath, password, overwrite, onProgress, cancellationToken);
                }
            }
            else
            {
                var invalidZipFileExtensionExceptionMessage = StringLocalizer.GetString(nameof(AppStrings.InvalidZipExtensionException), zipFileExtension);
                throw new InvalidZipExtensionException(invalidZipFileExtensionExceptionMessage);
            }

            if (string.IsNullOrWhiteSpace(itemPath)) return duplicateCount;

            string? artifactPath = "";
            var extractedZipFilePath = GetFilePath(zipFullPath, itemPath);

            //Add another path in Windows
            if (destinationPath.Contains('\\'))
            {
                artifactPath = destinationPath + "\\" + extractedZipFilePath;
            }
            //Add another path in Android
            else
            {
                artifactPath = destinationPath + "/" + extractedZipFilePath;
            }

            var fileAttribute = File.GetAttributes(itemPath);
            var getFileName = Path.GetFileName(artifactPath);

            if (fileAttribute.HasFlag(FileAttributes.Directory))
            {
                Directory.Move(artifactPath, Path.Combine(destinationPath, getFileName));
            }
            else
            {
                File.Move(artifactPath, Path.Combine(destinationPath, getFileName));
            }

            Directory.Delete(newPath, true);

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
        var fileName = Path.GetFileNameWithoutExtension(zipFilePath);
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
        var fileName = Path.GetFileNameWithoutExtension(zipFilePath);
        var artifact = await LocalDeviceFileService.GetArtifactAsync(zipFilePath);
        var providerType = artifact.ProviderType;

        var artifacts = new List<FsArtifact>();
        using var archive = ZipArchive.Open(zipFilePath, new ReaderOptions() { Password = password });

        var entries = archive.Entries.ToList();
        var keys = entries.Select(c => c.Key).ToList();
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

   
    private static async Task<int> ExtractZipArtifactAsync(string zipFullPath,
                                                string destinationPath,
                                                string itemPath,
                                                bool overwrite = false,
                                                string? password = null,
                                                Func<ProgressInfo, Task>? onProgress = null,
                                                CancellationToken? cancellationToken = null)
    {
        int? progressCount = null;
        var duplicateCount = 0;
        var itemExtension = Path.GetExtension(itemPath);
        var filePath = GetFilePath(zipFullPath, itemPath);

        using var archive = ZipArchive.Open(zipFullPath, new ReaderOptions() { Password = password });
        var entries = archive.Entries.ToList();
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
                string? key;
                if (string.IsNullOrWhiteSpace(itemExtension))
                {
                    if (filePath.Contains('\\'))
                    {
                        key = entry.Key.Replace("/", "\\")[..(entry.Key.Length - 1)];
                    }
                    else
                    {
                        key = entry.Key[..^1];
                    }

                    if (key.StartsWith(filePath))
                    {
                        entry.WriteToDirectory(destinationPath, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = overwrite
                        });
                    }
                }
                else
                {
                    if (filePath.Contains('\\'))
                    {
                        key = entry.Key.Replace("/", "\\");
                    }
                    else
                    {
                        key = entry.Key;
                    }

                    if (key.Contains(filePath))
                    {
                        entry.WriteToDirectory(destinationPath, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = overwrite
                        });
                    }
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

    private static async Task<int> ExtractRarArtifactAsync(
        string zipFullPath,
        string destinationPath,
        string itemPath,
        bool overwrite = false,
        string? password = null,
        Func<ProgressInfo, Task>? onProgress = null,
        CancellationToken? cancellationToken = null)
    {
        int? progressCount = null;
        var duplicateCount = 0;
        var itemExtension = Path.GetExtension(itemPath);
        var filePath = GetFilePath(zipFullPath, itemPath);

        using var archive = RarArchive.Open(zipFullPath, new ReaderOptions() { Password = password });
        var entries = archive.Entries.ToList();
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
                if (string.IsNullOrWhiteSpace(itemExtension))
                {
                    if (entry.Key.StartsWith(filePath))
                    {
                        entry.WriteToDirectory(destinationPath, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = overwrite
                        });
                    }
                }
                else
                {
                    if (entry.Key.Contains(filePath))
                    {
                        entry.WriteToDirectory(destinationPath, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = overwrite
                        });
                    }
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

    private static async Task<int> ExtractRarAsync(
        string fullPath,
        string destinationPath,
        string? password = null,
        bool overwrite = false,
        Func<ProgressInfo, Task>? onProgress = null,
        CancellationToken? cancellationToken = null)
    {
        int? progressCount = null;
        var duplicateCount = 0;
        using var archive = RarArchive.Open(fullPath, new ReaderOptions() { Password = password });
        var entries = archive.Entries.ToList();
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
        string? password = null,
        bool overwrite = false,
        Func<ProgressInfo, Task>? onProgress = null,
        CancellationToken? cancellationToken = null)
    {
        int? progressCount = null;
        var duplicateCount = 0;
        using var archive = ZipArchive.Open(fullPath, new ReaderOptions() { Password = password });
        var entries = archive.Entries.ToList();
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

    private static string GetFilePath(string zipFullPath, string itemPath)
    {
        var zipFileName = Path.GetFileName(zipFullPath);

        var filePath = "";
        //Add another path in Windows
        if (zipFullPath.Contains('\\'))
        {
            filePath = itemPath.Replace(zipFileName + "\\", "");
        }
        //Add another path in Android
        else if (zipFullPath.Contains('/'))
        {
            filePath = itemPath.Replace(zipFileName + "/", "");
        }
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