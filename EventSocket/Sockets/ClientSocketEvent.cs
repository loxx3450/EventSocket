using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventSocket.Sockets
{
    public class ClientSocketEvent
    {
        public IPEndPoint EndPoint { get; private set; }

        public ClientSocketEvent(string hostname, int port)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(hostname), port);
        }

        public async Task<SocketEvent> GetSocketAsync()
        {
            TcpClient client = new TcpClient();

            await client.ConnectAsync(EndPoint);

            //Socket that is based on Stream To Server
            return new SocketEvent(client.GetStream());
        }
    }
}
