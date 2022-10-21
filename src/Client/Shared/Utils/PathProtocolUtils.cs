using System.Text.RegularExpressions;

namespace Functionland.FxFiles.Client.Shared.Utils;

public static class PathProtocolUtils
{
    public static Regex PathRegex { get; set; } = new Regex(@"^\*(?<protocol>\w+)\*\/(?<address>.*)");

    public static string GetPathProtocol(PathProtocol protocol)
    {
        return protocol switch
        {
            PathProtocol.Storage => "storage",
            PathProtocol.Fula => "fula",
            PathProtocol.Wwwroot => "wwwroot",
            PathProtocol.ThumbnailStorage => "thumbnailStorage",
            PathProtocol.ThumbnailFula => "thumbnailFula",
            _ => throw new ArgumentOutOfRangeException(nameof(protocol), protocol, null)
        };
    }

    public static PathProtocol GetPathProtocol(string protocol)
    {
        return protocol switch
        {
            "storage" => PathProtocol.Storage,
            "fula" => PathProtocol.Fula,
            "wwwroot" => PathProtocol.Wwwroot,
            "thumbnailStorage" => PathProtocol.ThumbnailStorage,
            "thumbnailFula" => PathProtocol.ThumbnailFula,
            _ => throw new ArgumentOutOfRangeException(nameof(protocol), protocol, null)
        };
    }

    public static string GetPath(string address, PathProtocol protocol)
    {
        var protocolStr = GetPathProtocol(protocol);
        return $"*{protocolStr}*/{address}";
    }

    public static bool IsMatch(string path)
    {
        return PathRegex.IsMatch(path);
    }

    public static (bool, PathProtocol, string) Match(string path)
    {
        var match = PathRegex.Match(path);

        if (match.Success)
        {
            var protocolStr = match.Groups["protocol"].Value;
            var protocol = GetPathProtocol(protocolStr);

            var address = match.Groups["address"].Value;

            return (true, protocol, address);
        }

        return (false, PathProtocol.None, path);
    }
}


