using Microsoft.Extensions.FileProviders;

using SharpCompress.Common;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.FxFileInfo;

public class PreviewFileInfo<TFileService> : IFileInfo
    where TFileService : IFileService
{
    private readonly string _path;
    private readonly TFileService _fileService;

    public PreviewFileInfo(string path, TFileService fileService)
    {
        _path = path;
        _fileService = fileService;
    }

    private bool IsMetaDataLoaded = false;
    private object loadMetaDataLock = new object();
    private void EnsureLoadMetadata()
    {
        if (!IsMetaDataLoaded)
        {
            lock (loadMetaDataLock)
            {
                if (!IsMetaDataLoaded)
                {
                    //ToDo: handle exceptions
                    var artifact = _fileService.GetArtifactAsync(_path)?.GetAwaiter().GetResult();
                    _exists = artifact is not null;
                    _length = artifact?.Size ?? 0;
                    _physicalPath = _fileService is IFulaFileService
                        ? artifact?.LocalFullPath ?? _path
                        : artifact?.FullPath ?? _path;
                    _name = Path.GetFileName(_physicalPath);
                    _lastModified = artifact?.LastModifiedDateTime ?? DateTimeOffset.FromUnixTimeSeconds(0);
                    _isDirectory = (artifact?.ArtifactType == FsArtifactType.Folder);

                    IsMetaDataLoaded = true;
                }
            }
        }
    }

    public bool _exists;
    public bool Exists => true;

    public long _length;
    public long Length
    {
        get
        {
            EnsureLoadMetadata();
            return _length;
        }
    }

    public string _physicalPath;
    public string PhysicalPath => _path;

    public string _name;
    public string Name => Path.GetFileName(_path);

    public DateTimeOffset _lastModified;
    public DateTimeOffset LastModified
    {
        get
        {
            EnsureLoadMetadata();
            return _lastModified;
        }
    }

    private bool _isDirectory;
    public bool IsDirectory
    {
        get
        {
            EnsureLoadMetadata();
            return _isDirectory;
        }
    }

    public Stream CreateReadStream()
    {
        try
        {
            return _fileService.GetFileContentAsync(_path).GetAwaiter().GetResult();
        }
        catch
        {
            return new MemoryStream();
        }
    }
}
