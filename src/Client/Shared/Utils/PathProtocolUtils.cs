using System.Text.RegularExpressions;

namespace Functionland.FxFiles.Client.Shared.Utils;

public static class PathProtocolUtils
{
    public static Regex PathRegex { get; set; } = new Regex(@"^(./)?\*(?<protocol>\w+)\*(?<address>.*)");

    public static string GetPathProtocol(PathProtocol protocol)
    {
        return protocol switch
        {
            PathProtocol.Storage => "storage",
            PathProtocol.Fula => "fula",
            PathProtocol.Wwwroot => "wwwroot",
            PathProtocol.ThumbnailStorage => "thumbnail-storage",
            PathProtocol.ThumbnailFula => "thumbnail-fula",
            _ => throw new ArgumentOutOfRangeException(nameof(protocol), protocol, null)
        };
    }

    public static string InProtocol(this string address, PathProtocol protocol)
    {
        return GetPath(protocol, address);
    }

    public static PathProtocol GetPathProtocol(string protocol)
    {
        return protocol switch
        {
            "storage" => PathProtocol.Storage,
            "fula" => PathProtocol.Fula,
            "wwwroot" => PathProtocol.Wwwroot,
            "thumbnail-storage" => PathProtocol.ThumbnailStorage,
            "thumbnail-fula" => PathProtocol.ThumbnailFula,
            _ => throw new ArgumentOutOfRangeException(nameof(protocol), protocol, null)
        };
    }

    public static string GetPath(PathProtocol protocol, string address)
    {
        var protocolStr = GetPathProtocol(protocol);
        return $"*{protocolStr}*{address}";
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


