using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventSocket.Sockets
{
    public class ServerSocketEvent
    {
        public IPEndPoint EndPoint { get; private set; }
        public TcpListener Listener { get; set; }


        //Invokes when ServerSocket receives new Connection
        public event Action<SocketEvent> OnClientIsConnected;


        public ServerSocketEvent(string hostname, int port)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(hostname), port);

            Listener = new TcpListener(EndPoint);
            Listener.Start();
        }


        public async Task<SocketEvent> GetSocketAsync()
        {
            TcpClient client = await Listener.AcceptTcpClientAsync();

            //Socket that is based on Stream To Client
            return new SocketEvent(client.GetStream());
        }


        //User should be subscribed on the event OnClientIsConnected
        public void StartAcceptingClients()
        {
            _ = Task.Run(HandleConnections);
        }


        private void HandleConnections()
        {
            while (true)
            {
                TcpClient client = Listener.AcceptTcpClient();

                //Part of Debug
                Console.WriteLine("Connected");                                         //TEMP

                SocketEvent socketEvent = new SocketEvent(client.GetStream());

                OnClientIsConnected?.Invoke(socketEvent);
            }
        }


        ~ServerSocketEvent()
        {
            Listener?.Stop();
        }
    }
}
