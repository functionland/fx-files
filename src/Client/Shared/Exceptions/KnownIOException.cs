using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Client.Shared.Exceptions
{
    public class KnownIOException : KnownException
    {
        public KnownIOException(string message)
            : base(message)
        {
        }

        public KnownIOException(string message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
