using Microsoft.Extensions.FileProviders;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.FsFileInfo;

public class ThumbFileInfo<TFileService> : IFileInfo
    where TFileService : IFileService
{
    private IArtifactThumbnailService<TFileService> _artifactThumbnailService;
    private TFileService _fileService;
    private FileInfo? _fileInfo;
    private readonly string _path;
    private readonly ThumbnailScale _scale;

    public ThumbFileInfo(string path, ThumbnailScale scale, IArtifactThumbnailService<TFileService> artifactThumbnailService, TFileService fileService)
    {
        _path = path;
        _scale = scale;
        _artifactThumbnailService = artifactThumbnailService;
        _fileService = fileService;
    }

    private bool IsMetaDataLoaded = false;
    private object loadMetaDataLock = new object();
    private void EnsureLoadThumbnail()
    {
        if (!IsMetaDataLoaded)
        {
            lock (loadMetaDataLock)
            {
                if (!IsMetaDataLoaded)
                {
                    //ToDo: handle exceptions
                        var artifact = _fileService.GetArtifactAsync(_path)?.GetAwaiter().GetResult();
                        if (artifact != null)
                        {
                            var thumbnailAddress = _artifactThumbnailService.GetOrCreateThumbnailAsync(artifact, _scale)?.GetAwaiter().GetResult();
                            if (thumbnailAddress != null)
                            {
                                _physicalPath = thumbnailAddress;
                                _fileInfo = new FileInfo(thumbnailAddress);
                            }
                        }
                        IsMetaDataLoaded = true;
                }
            }
        }
    }

    public bool Exists => true;

    public long Length
    {
        get
        {
            EnsureLoadThumbnail();
            if (_fileInfo is null)
                return 0;

            return _fileInfo.Length;
        }
    }

    public string _physicalPath;
    public string PhysicalPath
    {
        get
        {
            EnsureLoadThumbnail();
            return _physicalPath;
        }
    }

    public string Name => Path.GetFileName(_path);

    public DateTimeOffset LastModified
    {
        get
        {
            EnsureLoadThumbnail();
            if (_fileInfo is null)
                return DateTimeOffset.FromUnixTimeSeconds(0);

            return _fileInfo.LastWriteTime;
        }
    }

    public bool IsDirectory
    {
        get
        {
            EnsureLoadThumbnail();
            return Directory.Exists(_physicalPath);
        }
    }

    public Stream CreateReadStream()
    {
        try
        {
            EnsureLoadThumbnail();
            if (_physicalPath == null)
                return new MemoryStream();

            var streamReader = new StreamReader(_physicalPath);
            return streamReader.BaseStream;
        }
        catch
        {
            return new MemoryStream();
        }
    }
}
