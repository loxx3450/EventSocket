using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventSocket.Sockets
{
    public class ServerSocket
    {
        public IPEndPoint EndPoint { get; private set; }
        public TcpListener Listener { get; set; }

        public ServerSocket(string hostname, int port)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(hostname), port);

            Listener = new TcpListener(EndPoint);
            Listener.Start();
        }

        public async Task<Socket> GetSocket()
        {
            TcpClient client = await Listener.AcceptTcpClientAsync();

            //Socket that is based on Stream To Client
            return new Socket(client.GetStream());
        }

        ~ServerSocket()
        {
            Listener?.Stop();
        }
    }
}
