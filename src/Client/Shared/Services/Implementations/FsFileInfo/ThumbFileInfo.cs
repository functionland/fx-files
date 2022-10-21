using Functionland.FxFiles.Client.Shared.Services.Contracts;

using Microsoft.Extensions.FileProviders;

using System.Threading;
using System.Xml.Linq;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.FsFileInfo;

public class ThumbFileInfo<TFileService> : IFileInfo
    where TFileService : IFileService
{
    private IArtifactThumbnailService<TFileService> _artifactThumbnailService;
    private TFileService _fileService;
    private FileInfo _fileInfo;
    private readonly string _path;

    public ThumbFileInfo(string path, IArtifactThumbnailService<TFileService> artifactThumbnailService, TFileService fileService)
    {
        _path = path;
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
                        var thumbnailAddress = _artifactThumbnailService.GetOrCreateThumbnailAsync(artifact, ThumbnailScale.Medium).GetAwaiter().GetResult();
                        if(thumbnailAddress != null)
                        {
                            _physicalPath = thumbnailAddress;
                            _fileInfo = new FileInfo(thumbnailAddress);
                        }
                    }
                }
            }
        }
    }

    public bool Exists
    {
        get
        {
            EnsureLoadThumbnail();
            return _fileInfo.Exists;
        }
    }

    public long Length
    {
        get
        {
            EnsureLoadThumbnail();
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
            var streamReader = new StreamReader(PhysicalPath);
            return streamReader.BaseStream;
        }
        catch
        {
            return new MemoryStream();
        }
    }
}
