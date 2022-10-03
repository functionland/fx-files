using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Exceptions
{
    [Serializable]
    public class SameDestinationFileException : DomainLogicException
    {
        public SameDestinationFileException(string message) : base(message)
        {
        }

        public SameDestinationFileException(LocalizedString message) : base(message)
        {
        }

        public SameDestinationFileException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public SameDestinationFileException(LocalizedString message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SameDestinationFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
