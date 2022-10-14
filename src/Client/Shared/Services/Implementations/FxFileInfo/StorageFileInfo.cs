using Microsoft.Extensions.FileProviders;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.FxFileInfo;

public class StorageFileInfo : IFileInfo
{
    private readonly string _path;
    private readonly FileInfo _fileInfo;

    public StorageFileInfo(string path)
    {
        _path = path;
        _fileInfo = new FileInfo(path);
    }

    public bool Exists => _fileInfo.Exists;

    public long Length => _fileInfo.Length;

    public string PhysicalPath => _path;

    public string Name => _fileInfo.Name;

    public DateTimeOffset LastModified => _fileInfo.LastWriteTime;

    public bool IsDirectory => Directory.Exists(_path);

    public Stream CreateReadStream()
    {
        try
        {
            var streamReader = new StreamReader(_path);
            return streamReader.BaseStream;
        }
        catch
        {
            return new MemoryStream();
        }
    }
}
