using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketEventLibrary.SocketEventMessageCore
{
    public interface IRecoverable
    {
        //SocketEventMessage should be built based on suitable Stream
        static abstract SocketEventMessage RecoverSocketEventMessage(MemoryStream memoryStream);
    }
}
