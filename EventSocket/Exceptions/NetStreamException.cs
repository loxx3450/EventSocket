using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketEventLibrary.Exceptions
{
    public class NetStreamException : SocketEventException
    {
        public NetStreamException()
            : base()
        { }

        public NetStreamException(string message)
            : base(message)
        { }

        public NetStreamException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
