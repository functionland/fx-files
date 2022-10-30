namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IZipService
{
    Task ExtractZipAsync(string fullPath, string destinationPath, string? password = null, bool overwrite = false, CancellationToken? cancellationToken = null);
    Task<List<FsArtifact>> ZipFileViewerAsync(string zipFilePath, string subDirectoriesPath, string? password = null, CancellationToken? cancellationToken = null);
    Task ExtractZippedArtifactAsync(string zipFullPath, string destinationPath, string itemPath, bool overwrite = false, string? password = null, CancellationToken? cancellationToken = null);
}
