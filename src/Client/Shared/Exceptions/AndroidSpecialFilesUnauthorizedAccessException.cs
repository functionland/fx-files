using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class AndroidSpecialFilesUnauthorizedAccessException : DomainLogicException
{
    public AndroidSpecialFilesUnauthorizedAccessException(string message)
        : base(message)
    {
    }

}

