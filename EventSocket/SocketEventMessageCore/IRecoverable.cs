using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketEventLibrary.SocketEventMessageCore
{
    /// <summary>
    /// Provides the absract method 'RecoverSocketEventMessage',
    /// that should be realized to build Message basing on MemoryStream, 
    /// that SocketEvent gets from other side of Connection.
    /// </summary>
    public interface IRecoverable
    {
        /// <summary>
        /// Builds SocketEventMessage basing on Stream.
        /// </summary>
        /// <param name="memoryStream">Stream of SocketEventMessage.</param>
        /// <returns>The instance of SocketEventMessage, 
        /// that is representing the incoming Stream</returns>
        static abstract SocketEventMessage RecoverSocketEventMessage(MemoryStream memoryStream);
    }
}
