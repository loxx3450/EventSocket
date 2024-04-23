using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketEventLibrary.Exceptions
{
    /// <summary>
    /// Class <c>SocketEventException</c> is a custom default exception of SocketEventLibrary.
    /// </summary>
    public class SocketEventException : Exception
    {
        public SocketEventException() 
            : base()
        { }

        public SocketEventException(string message)
            : base(message)
        { }

        public SocketEventException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
