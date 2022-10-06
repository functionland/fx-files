using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations
{
    public class FxFileProvider : IFileProvider
    {
        private readonly IFileProvider _fileProvider;

        public FxFileProvider(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return _fileProvider.GetDirectoryContents(subpath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (subpath.EndsWith(".HandleByApp=true"))
            {
                subpath = subpath.Replace("%20", " ");
                subpath = subpath.Replace(".HandleByApp=true", string.Empty);
                subpath = subpath.Replace("_content/Functionland.FxFiles.Client.Shared/", string.Empty);

                return new FxFileInfo(subpath);
            }

            return _fileProvider.GetFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return _fileProvider.Watch(filter);
        }
    }
}

public class FxFileInfo : IFileInfo
{
    private readonly string _path;

    public FxFileInfo(string path)
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
