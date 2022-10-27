using Functionland.FxFiles.Client.Shared.Extensions;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class ZipService : IZipService
{
    [AutoInject]public IStringLocalizer<AppStrings> StringLocalizer { get; set; } = default!;

    public virtual Task<List<FsArtifact>> GetZippedArtifactsAsync(string zipFilePath, string subDirectoriesPath, string? password = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }
    public virtual Task ExtractZippedArtifactAsync(string zipFullPath, string destinationPath, List<string> fileNames, bool overwrite = false, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public virtual async Task ExtractZipAsync(string fullPath, string destinationPath, string? password = null, bool overwrite = false, CancellationToken? cancellationToken = null)
    {
        var extension = Path.GetExtension(fullPath);
        var lowerCaseFile = AppStrings.File.ToLowerFirstChar();

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
            throw new PasswordDidNotMatchedException(StringLocalizer.GetString(AppStrings.PasswordDidNotMatchedException));
        }
        catch (IOException ex) when (ex.Message.StartsWith("The file") && ex.Message.EndsWith("already exists."))
        {
            throw new ArtifactAlreadyExistsException(StringLocalizer.GetString(AppStrings.ArtifactAlreadyExistsException, lowerCaseFile));
        }
        catch
        {
            throw new DomainLogicException(StringLocalizer.GetString(AppStrings.TheOpreationFailedMessage));
        }
    }

}