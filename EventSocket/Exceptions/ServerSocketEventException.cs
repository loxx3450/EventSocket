using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketEventLibrary.Exceptions
{
    public class ServerSocketEventException : SocketEventException
    {
        public const string SERVER_SOCKET_CLOSED_LISTENER = "TcpListener is closed";

        public ServerSocketEventException()
            : base()
        { }

        public ServerSocketEventException(string message)
            : base(message)
        { }

        public ServerSocketEventException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
