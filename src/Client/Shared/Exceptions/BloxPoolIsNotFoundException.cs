using System.Runtime.Serialization;

namespace Functionland.FxFiles.Client.Shared.Exceptions
{
    [Serializable]
    public class BloxPoolIsNotFoundException : DomainLogicException
    {
        public BloxPoolIsNotFoundException(string message) : base(message)
        {
        }

        public BloxPoolIsNotFoundException(LocalizedString message) : base(message)
        {
        }

        public BloxPoolIsNotFoundException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public BloxPoolIsNotFoundException(LocalizedString message, Exception? innerException) : base(message, innerException)
        {
        }

        protected BloxPoolIsNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
