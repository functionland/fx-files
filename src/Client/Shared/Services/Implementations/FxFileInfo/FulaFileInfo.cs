using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.FxFileInfo;

public class FulaFileInfo : IFileInfo
{
    private readonly string _path;

    public FulaFileInfo(string path)
    {
        _path = path;
    }

    public bool Exists => true;

    public long Length => 0;

    public string PhysicalPath => null;

    public string Name => null;

    public DateTimeOffset LastModified => DateTimeOffset.FromUnixTimeSeconds(0);

    public bool IsDirectory => false;

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
