using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketEventLibrary.Exceptions
{
    public class SocketEventMessageBuilderException : SocketEventException
    {
        public const string BUILDER_MESSAGE_TYPE_NOT_FOUND = "Some problems with Stream are occured: Type of the Message is not found";
        public const string BUILDER_NOT_SUPPORTED_TYPE = "Type of received Message is not supported";
        public const string BUILDER_METHOD_RETURNED_NULL = "Method to get SocketEventMessage returned null";

        public SocketEventMessageBuilderException()
            : base()
        { }

        public SocketEventMessageBuilderException(string message)
            : base(message)
        { }

        public SocketEventMessageBuilderException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
