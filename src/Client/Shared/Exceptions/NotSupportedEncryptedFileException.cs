using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions;

[Serializable]
internal class NotSupportedEncryptedFileException : DomainLogicException
{
    public NotSupportedEncryptedFileException(string message) : base(message)
    {
    }

    public NotSupportedEncryptedFileException(LocalizedString message) : base(message)
    {
    }

    public NotSupportedEncryptedFileException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public NotSupportedEncryptedFileException(LocalizedString message, Exception? innerException) : base(message, innerException)
    {
    }

    protected NotSupportedEncryptedFileException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
