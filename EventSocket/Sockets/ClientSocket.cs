using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventSocket.Sockets
{
    public class ClientSocket
    {
        public IPEndPoint EndPoint { get; private set; }

        public ClientSocket(string hostname, int port)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(hostname), port);
        }

        public async Task<Socket> GetSocket() 
        {
            TcpClient client = new TcpClient();

            await client.ConnectAsync(EndPoint);

            //Socket that is based on Stream To Server
            return new Socket(client.GetStream());
        }
    }
}
