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

    public virtual async Task<List<FsArtifact>> ZipFileViewerAsync(string zipFilePath, string subDirectoriesPath, string? password = null, CancellationToken? cancellationToken = null)
    {
        var extension = Path.GetExtension(zipFilePath);
        var fsArtifacts = new List<FsArtifact>();

        try
        {
            if (extension == ".zip")
            {
                fsArtifacts = ZipFileViewer(zipFilePath, subDirectoriesPath, cancellationToken);
            }
            else if (extension == ".rar")
            {
                fsArtifacts = RarFileViewer(zipFilePath, subDirectoriesPath, password, cancellationToken);
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

        return fsArtifacts;
    }

    public virtual async Task ExtractZippedArtifactAsync(string zipFullPath, string destinationPath, string itemPath, bool overwrite = false, string? password = null, CancellationToken? cancellationToken = null)
    {
        var lowerCaseFile = AppStrings.File.ToLowerFirstChar();
        var zipFileName = Path.GetFileName(zipFullPath);
        var zipFileExtension = Path.GetExtension(zipFileName);
        var ZipFileNameWithoutExtension = zipFileName.Replace(zipFileExtension, "");
        var deletedPath = Path.Combine(destinationPath, ZipFileNameWithoutExtension);
        var artifactPath = "";

        try
        {
                var itemExtension = Path.GetExtension(itemPath);
                if (zipFileExtension == ".zip")
                {
                    using (var archive = ZipArchive.Open(zipFullPath, new ReaderOptions() { Password = password }))
                    {
                        foreach (var entry in archive.Entries.ToList())
                        {
                            var filePath = itemPath.Replace(zipFileName + "\\", "");
                            artifactPath = destinationPath + "\\" + filePath;
                            var key = "";
                            if (string.IsNullOrWhiteSpace(itemExtension))
                            {
                                key = entry.Key.Replace("/", "\\").Substring(0, entry.Key.Length - 1);

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
                                key = entry.Key.Replace("/", "\\");

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
            }
                else if (zipFileExtension == ".rar")
                {
                    using (var archive = RarArchive.Open(zipFullPath, new ReaderOptions() { Password = password }))
                    {
                        foreach (var entry in archive.Entries.ToList())
                        {
                            var filePath = itemPath.Replace(zipFileName + "\\", "");
                            artifactPath = destinationPath + "\\" + filePath;

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
            }

                var getFileName = Path.GetFileName(artifactPath);
                if (string.IsNullOrWhiteSpace(itemExtension))
                {
                    Directory.Move(artifactPath, Path.Combine(destinationPath, getFileName));
                }
                else
                {
                    File.Move(artifactPath, Path.Combine(destinationPath, getFileName));
                }
                Directory.Delete(deletedPath, true);
            
            }
        catch (IOException ex) when (ex.Message.EndsWith("because a file or directory with the same name already exists."))
        {
            Directory.Delete(deletedPath, true);
            throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, lowerCaseFile));
        }
        catch (CryptographicException ex) when (ex.Message == "Encrypted Rar archive has no password specified.")
        {
            throw new InvalidPasswordException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch (CryptographicException ex) when (ex.Message == "The password did not match.")
        {
            Directory.Delete(deletedPath, true);
            throw new PasswordDidNotMatchedException(StringLocalizer.GetString(AppStrings.PasswordDidNotMatchedException));
        }
        catch (InvalidFormatException ex) when (ex.Message.StartsWith("Unknown Rar Header:"))
        {
            throw new PasswordDidNotMatchedException(StringLocalizer.GetString(AppStrings.PasswordDidNotMatchedException));
        }
        catch
        {
            throw new DomainLogicException(StringLocalizer.GetString(AppStrings.TheOpreationFailedMessage));
        }

    }

    public virtual async Task ExtractZipAsync(string fullPath, string destinationPath, string? password = null, bool overwrite = false, CancellationToken? cancellationToken = null)
    {
        var extension = Path.GetExtension(fullPath);
        var lowerCaseFile = AppStrings.File.ToLowerFirstChar();
        var zipFileName = Path.GetFileName(fullPath);
        var ZipFileNameWithoutExtension = zipFileName.Replace(extension, "");
        var deletedPath = Path.Combine(destinationPath, ZipFileNameWithoutExtension);

        try
        {
            if (extension == ".zip")
            {
                using (var archive = ZipArchive.Open(fullPath, new ReaderOptions() { Password = password }))
                {
                    foreach (var entry in archive.Entries.ToList())
                    {
                        entry.WriteToDirectory(destinationPath, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = overwrite
                        });
                    }
                }
            }
            else if (extension == ".rar")
            {

                using (var archive = RarArchive.Open(fullPath, new ReaderOptions() { Password = password }))
                {
                    foreach (var entry in archive.Entries.ToList())
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

        catch (CryptographicException ex) when (ex.Message == "The password did not match.")
        {
            Directory.Delete(deletedPath, true);
            throw new PasswordDidNotMatchedException(StringLocalizer.GetString(AppStrings.PasswordDidNotMatchedException));
        }
        catch (InvalidFormatException ex) when (ex.Message.StartsWith("Unknown Rar Header:"))
        {
            throw new PasswordDidNotMatchedException(StringLocalizer.GetString(AppStrings.PasswordDidNotMatchedException));
        }
        catch (IOException ex) when (ex.Message.StartsWith("The file") && ex.Message.EndsWith("already exists."))
        {
            throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, lowerCaseFile));
        }
        catch (CryptographicException ex) when (ex.Message == "Encrypted Rar archive has no password specified.")
        {
            throw new InvalidPasswordException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch (CryptographicException ex) when (ex.Message == "No password supplied for encrypted zip.")
        {
            Directory.Delete(deletedPath, true);
            throw new InvalidPasswordException(StringLocalizer.GetString(AppStrings.InvalidPasswordException));
        }
        catch
        {
            throw new DomainLogicException(StringLocalizer.GetString(AppStrings.TheOpreationFailedMessage));
        }
    }

    private List<FsArtifact> RarFileViewer(string zipFilePath, string subDirectoriesPath, string? password = null, CancellationToken? cancellationToken = null)
    {
        var itemsPath = subDirectoriesPath.Split('\\');
        var fsArtifacts = new List<FsArtifact>();

        using (var archive = RarArchive.Open(zipFilePath, new ReaderOptions() { Password = password }))
        {
            foreach (var entry in archive.Entries.ToList())
            {
                var fsArtifactType = entry.IsDirectory ? FsArtifactType.Folder : FsArtifactType.File;
                var newPath = Path.Combine(zipFilePath, entry.Key);
                var entryFileName = Path.GetFileName(newPath);

                var newFsArtifact = new FsArtifact(newPath, entryFileName, fsArtifactType, FsFileProviderType.InternalMemory)
                {
                    FileExtension = !entry.IsDirectory ? Path.GetExtension(newPath) : null,
                    LastModifiedDateTime = (DateTimeOffset)entry.LastModifiedTime
                };

                var result = entry.Key.Split('\\');
                if (result.Length == itemsPath.Length && newFsArtifact.FullPath.Contains(subDirectoriesPath))
                {
                    fsArtifacts.Add(newFsArtifact);
                }
            }
        }

        return fsArtifacts;

    }
    private List<FsArtifact> ZipFileViewer(string zipFilePath, string subDirectoriesPath, CancellationToken? cancellationToken = null)
    {
        var itemsPath = subDirectoriesPath.Split('\\');
        var fsArtifacts = new List<FsArtifact>();

        using (var archive = ZipArchive.Open(zipFilePath))
        {
            foreach (var entry in archive.Entries.ToList())
            {
                var fsArtifactType = entry.IsDirectory ? FsArtifactType.Folder : FsArtifactType.File;
                var key = entry.Key.Trim('/').Replace("/", "\\");
                var newPath = Path.Combine(zipFilePath, key);
                var entryFileName = Path.GetFileName(newPath);

                var newFsArtifact = new FsArtifact(newPath, entryFileName, fsArtifactType, FsFileProviderType.InternalMemory)
                {
                    FileExtension = !entry.IsDirectory ? Path.GetExtension(newPath) : null,
                    LastModifiedDateTime = (DateTimeOffset)entry.LastModifiedTime
                };

                var result = key.Split('\\');
                if (result.Length == itemsPath.Length && newPath.Contains(subDirectoriesPath))
                {
                    fsArtifacts.Add(newFsArtifact);
                }
            }
        }

        return fsArtifacts;
    }

}