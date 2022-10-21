using Microsoft.Extensions.FileProviders;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.FxFileInfo;

public class PreviewFileInfo : IFileInfo
{
    private readonly string _path;
    private readonly IFileService _fileService;

    public PreviewFileInfo(string path, IFileService fileService)
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
                    _stream = _fileService.GetFileContentAsync(_path).GetAwaiter().GetResult();

                    IsMetaDataLoaded = true;
                }
            }
        }
    }

    public bool _exists;
    public bool Exists
    {
        get
        {
            EnsureLoadMetadata();
            return _exists;
        }
    }

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
    public string PhysicalPath
    {
        get
        {
            EnsureLoadMetadata();
            return _physicalPath;
        }
    }

    public string _name;
    public string Name
    {
        get
        {
            EnsureLoadMetadata();
            return _name;
        }
    }

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

    public Stream _stream;
    public Stream CreateReadStream()
    {
        try
        {
            EnsureLoadMetadata();
            return _stream;
        }
        catch
        {
            return new MemoryStream();
        }
    }
}
