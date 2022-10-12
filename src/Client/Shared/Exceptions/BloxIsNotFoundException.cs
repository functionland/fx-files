using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions
{
    [Serializable]
    public class BloxIsNotFoundException : DomainLogicException
    {
        public BloxIsNotFoundException(string message) : base(message)
        {
        }

        public BloxIsNotFoundException(LocalizedString message) : base(message)
        {
        }

        public BloxIsNotFoundException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public BloxIsNotFoundException(LocalizedString message, Exception? innerException) : base(message, innerException)
        {
        }

        protected BloxIsNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
