using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Exceptions
{
    [Serializable]
    public class SameDestinationFolderException : DomainLogicException
    {
        public SameDestinationFolderException(string message) : base(message)
        {
        }

        public SameDestinationFolderException(LocalizedString message) : base(message)
        {
        }

        public SameDestinationFolderException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public SameDestinationFolderException(LocalizedString message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SameDestinationFolderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
