using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
public class ArtifactUnauthorizedAccessException : DomainLogicException
{
    public ArtifactUnauthorizedAccessException(string message)
        : base(message)
    {
    }

}
