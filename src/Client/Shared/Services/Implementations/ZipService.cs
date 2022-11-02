using Functionland.FxFiles.Client.Shared.Extensions;
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

    public virtual Task<List<FsArtifact>> ViewZipFileAsync(string zipFilePath, string subDirectoriesPath, string? password = null, CancellationToken? cancellationToken = null)
    {
        var extension = Path.GetExtension(zipFilePath);
        var fsArtifacts = new List<FsArtifact>();

        try
        {
            if (extension == ".zip")
            {
                fsArtifacts = ZipFileViewer(zipFilePath, subDirectoriesPath, password);
            }
            else if (extension == ".rar")
            {
                fsArtifacts = RarFileViewer(zipFilePath, subDirectoriesPath, password);
            }
            else
            {
                var invalidZipFileExtensionExceptionMessage = StringLocalizer.GetString(nameof(AppStrings.InvalidZipExtensionException), extension);
                throw new InvalidZipExtensionException(invalidZipFileExtensionExceptionMessage);
            }
        }
        catch (InvalidFormatException ex) when (ex.Message.StartsWith("Unknown Rar Header:"))
        {
            throw new PasswordDidNotMatchedException(StringLocalizer.GetString(AppStrings.PasswordDidNotMatchedException));
        }
        catch
        {
            throw new DomainLogicException(StringLocalizer.GetString(AppStrings.TheOpreationFailedMessage));
        }

        return Task.FromResult(fsArtifacts);
    }

    public virtual Task ExtractZippedArtifactAsync(string zipFullPath,
                                                   string destinationPath,
                                                   string destinationFolderName,
                                                   string? itemPath = null,
                                                   bool overwrite = false,
                                                   string? password = null,
                                                   CancellationToken? cancellationToken = null)
    {
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
                    ExtractZipArtifact(zipFullPath, newPath, itemPath, overwrite, password);
                }
                else
                {
                    ExtractZip(zipFullPath, newPath, password, overwrite);
                }
            }
            else if (zipFileExtension == ".rar")
            {
                if (!string.IsNullOrWhiteSpace(itemPath))
                {
                    ExtractRarArtifact(zipFullPath, newPath, itemPath, overwrite, password);
                }
                else
                {
                    ExtractRar(zipFullPath, newPath, password, overwrite);
                }
            }
            else
            {
                var invalidZipFileExtensionExceptionMessage = StringLocalizer.GetString(nameof(AppStrings.InvalidZipExtensionException), zipFileExtension);
                throw new InvalidZipExtensionException(invalidZipFileExtensionExceptionMessage);
            }

            if (string.IsNullOrWhiteSpace(itemPath)) return Task.CompletedTask;

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

            return Task.CompletedTask;
        }
        catch (IOException ex) when (ex.Message.EndsWith("because a file or directory with the same name already exists."))
        {
            Directory.Delete(newPath, true);
            var lowerCaseArtifact = StringLocalizer[nameof(AppStrings.Artifact)].Value.ToLowerFirstChar();
            throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, lowerCaseArtifact));
        }
        catch (CryptographicException ex) when (ex.Message == "Encrypted Rar archive has no password specified.")
        {
            throw new InvalidPasswordException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch (CryptographicException ex) when (ex.Message == "The password did not match.")
        {
            Directory.Delete(newPath, true);
            throw new PasswordDidNotMatchedException(StringLocalizer.GetString(AppStrings.PasswordDidNotMatchedException));
        }
        catch (InvalidFormatException ex) when (ex.Message.StartsWith("Unknown Rar Header:"))
        {
            throw new PasswordDidNotMatchedException(StringLocalizer.GetString(AppStrings.PasswordDidNotMatchedException));
        }
        catch (IOException ex) when (ex.Message.StartsWith("The file") && ex.Message.EndsWith("already exists."))
        {
            var lowerCaseArtifact = StringLocalizer[nameof(AppStrings.Artifact)].Value.ToLowerFirstChar();
            throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, lowerCaseArtifact));
        }
        catch (CryptographicException ex) when (ex.Message == "No password supplied for encrypted zip.")
        {
            Directory.Delete(newPath, true);
            throw new InvalidPasswordException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch
        {
            throw new DomainLogicException(StringLocalizer.GetString(AppStrings.TheOpreationFailedMessage));
        }
    }

    private List<FsArtifact> RarFileViewer(string zipFilePath, string subDirectoriesPath, string? password = null)
    {
        var length = SplitPath(subDirectoriesPath);
        var providerType = LocalDeviceFileService.GetArtifactAsync(zipFilePath).Result.ProviderType;
        var fsArtifacts = new List<FsArtifact>();

        using (var archive = RarArchive.Open(zipFilePath, new ReaderOptions() { Password = password }))
        {
            foreach (var entry in archive.Entries.ToList())
            {
                var fsArtifactType = entry.IsDirectory ? FsArtifactType.Folder : FsArtifactType.File;
                var newPath = Path.Combine(zipFilePath, entry.Key);
                var entryFileName = Path.GetFileName(newPath);
                var newFsArtifact = new FsArtifact(newPath, entryFileName, fsArtifactType, providerType)
                {
                    FileExtension = !entry.IsDirectory ? Path.GetExtension(newPath) : null,
                    LastModifiedDateTime = entry.LastModifiedTime ?? DateTimeOffset.Now,
                };

                var keyLength = SplitPath(entry.Key);
                if (keyLength == length && newFsArtifact.FullPath.Contains(subDirectoriesPath))
                {
                    fsArtifacts.Add(newFsArtifact);
                }
            }
        }

        return fsArtifacts;
    }


    private List<FsArtifact> ZipFileViewer(string zipFilePath, string subDirectoriesPath, string? password = null)
    {
        var length = SplitPath(subDirectoriesPath);
        var fsArtifacts = new List<FsArtifact>();
        var providerType = LocalDeviceFileService.GetArtifactAsync(zipFilePath).Result.ProviderType;

        using (var archive = ZipArchive.Open(zipFilePath, new ReaderOptions() { Password = password }))
        {
            foreach (var entry in archive.Entries.ToList())
            {
                var fsArtifactType = entry.IsDirectory ? FsArtifactType.Folder : FsArtifactType.File;
                var key = entry.Key.Trim('/').Replace("/", "\\");
                var newPath = "";

                if (zipFilePath.Contains('\\'))
                {
                    newPath = Path.Combine(zipFilePath, key);
                }
                else if (zipFilePath.Contains('/'))
                {
                    newPath = Path.Combine(zipFilePath, entry.Key.Trim('/'));
                }

                var entryFileName = Path.GetFileName(newPath);
                var newFsArtifact = new FsArtifact(newPath, entryFileName, fsArtifactType, providerType)
                {
                    FileExtension = !entry.IsDirectory ? Path.GetExtension(newPath) : null,
                    LastModifiedDateTime = entry.LastModifiedTime ?? DateTimeOffset.Now,
                };

                var keyLength = SplitPath(key);
                if (keyLength == length && newPath.Contains(subDirectoriesPath))
                {
                    fsArtifacts.Add(newFsArtifact);
                }
            }
        }

        return fsArtifacts;
    }

    private static void ExtractZipArtifact(string zipFullPath, string destinationPath, string itemPath, bool overwrite = false, string? password = null)
    {
        var itemExtension = Path.GetExtension(itemPath);
        var filePath = GetFilePath(zipFullPath, itemPath);

        using var archive = ZipArchive.Open(zipFullPath, new ReaderOptions() { Password = password });
        foreach (var entry in archive.Entries.ToList())
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
    }

    private static void ExtractRarArtifact(string zipFullPath, string destinationPath, string itemPath, bool overwrite = false, string? password = null)
    {
        var itemExtension = Path.GetExtension(itemPath);
        var filePath = GetFilePath(zipFullPath, itemPath);

        using var archive = RarArchive.Open(zipFullPath, new ReaderOptions() { Password = password });
        foreach (var entry in archive.Entries.ToList())
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

    }

    private static void ExtractRar(string fullPath, string destinationPath, string? password = null, bool overwrite = false)
    {
        using var archive = RarArchive.Open(fullPath, new ReaderOptions() { Password = password });
        foreach (var entry in archive.Entries.ToList())
        {
            entry.WriteToDirectory(destinationPath, new ExtractionOptions()
            {
                ExtractFullPath = true,
                Overwrite = overwrite
            });
        }
    }

    private static void ExtractZip(string fullPath, string destinationPath,  string? password = null, bool overwrite = false)
    {
        using var archive = ZipArchive.Open(fullPath, new ReaderOptions() { Password = password });
        foreach (var entry in archive.Entries.ToList())
        {
            entry.WriteToDirectory(destinationPath, new ExtractionOptions()
            {
                ExtractFullPath = true,
                Overwrite = overwrite
            });
        }
    }

    private static int SplitPath(string subDirectoriesPath)
    {
        string[] itemsPath;
        int length = 0;
        if (subDirectoriesPath.Contains('\\'))
        {
            itemsPath = subDirectoriesPath.Split('\\');
            length = itemsPath.Length;
        }
        else if (subDirectoriesPath.Contains('/'))
        {
            itemsPath = subDirectoriesPath.Split('/');
            length = itemsPath.Length;
        }
        return length;
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
}