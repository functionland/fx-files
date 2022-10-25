using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class ZipService : IZipService
{
    public virtual Task<List<FsArtifact>> GetZippedFsArtifactsAsync(string zipFilePath, string subDirectoriesPath, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task ExtractZippedArtifactAsync(string zipFullPath, string destinationPath, List<string> fileNames, bool overwrite = false, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public virtual async Task ExtractZipAsync(string fullPath, string destinationPath, string? password = null, bool overwrite = false, CancellationToken? cancellationToken = null)
    {
        var extention = Path.GetExtension(fullPath);

        if(extention == ".zip")
        {
            try
            {
                using (var archive = ZipArchive.Open(fullPath, new ReaderOptions() { Password = password }))
                {
                    foreach (var entry in archive.Entries)
                    {
                        entry.WriteToDirectory(destinationPath, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = overwrite
                        });
                    }
                }
            }
            catch (CryptographicException)
            {
                throw new Exception("The password did not match.");
            }
            catch (IOException) //TODO: This should be more specific
            {
                throw new Exception("The artifact already exists.");
            }

        }
        else if(extention == ".rar")
        {
            try
            {
                using (var archive = RarArchive.Open(fullPath, new ReaderOptions() { Password = password }))
                {
                    foreach (var entry in archive.Entries)
                    {
                        entry.WriteToDirectory(destinationPath, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = overwrite
                        });
                    }
                }
            }
            catch (CryptographicException)
            {
                throw new Exception("The password did not match.");
            }
            catch (IOException) //TODO: This should be more specific
            {
                throw new Exception("The artifact already exists.");
            }

        }
        else
        {
            throw new Exception(""); //TODO
        } 
    }
}
