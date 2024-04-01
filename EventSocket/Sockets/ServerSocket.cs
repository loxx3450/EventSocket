using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventSocket.Sockets
{
    public class ServerSocket<T, K> : Socket<T, K>
    {
        public TcpListener Listener { get; set; }
        public List<TcpClient> Clients { get; set; } = new List<TcpClient>();

        public ServerSocket(string hostname, int port)
            : base(hostname, port)
        { }

        public override void Init(string hostname, int port)
        {
            Listener = new TcpListener(IPAddress.Parse(hostname), port);
            Listener.Start();

            _ = Task.Run(HandleConnections);
        }

        //Server gets new Clients and submits them to processing
        private void HandleConnections()
        {
            while (true)
            {
                TcpClient tcpClient = Listener.AcceptTcpClient();

                Console.WriteLine($"Client {tcpClient.Client.RemoteEndPoint} is connected");

                Clients.Add(tcpClient);                                                             //lock?

                _ = Task.Run(() => HandleRequests(tcpClient.GetStream()));
            }
        }

        //Sending Message to everybody who is connected to Server
        protected override void SendMessage(SocketMessage<T, K> socketMessage)
        {
            foreach (var client in Clients)
            {
                socketMessage.GetStream().CopyTo(client.GetStream());
            }
        }

        ~ServerSocket()
        {
            Listener?.Stop();

            foreach (var client in Clients)                 //??
            {
                client.Close();
            }
        }
    }
}
