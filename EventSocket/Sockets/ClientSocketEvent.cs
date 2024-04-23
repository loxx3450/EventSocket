using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketEventLibrary.Sockets
{
    public class ClientSocketEvent
    {
        //
        // ========== public properties: ==========
        //

        /// <value>
        /// Represents the endpoint of Server.
        /// </value>
        public IPEndPoint EndPoint { get; private set; }


        //
        // ========== constructors: ==========
        //

        public ClientSocketEvent(string hostname, int port)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(hostname), port);
        }


        //
        // ========== public methods: ==========
        //

        /// <summary>
        /// Gets async Socket, that represents Server-side.
        /// </summary>
        /// <returns>The object of SocketEvent, that is based on Stream of Client.</returns>
        public async Task<SocketEvent> GetSocketAsync()
        {
            TcpClient client = new TcpClient();

            await client.ConnectAsync(EndPoint);

            //Socket that is based on Stream To Server
            return new SocketEvent(client.GetStream());
        }
    }
}
