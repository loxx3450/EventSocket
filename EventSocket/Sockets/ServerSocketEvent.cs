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

        //Thread that gets new Connections
        private Thread connectionThread;


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
            connectionThread = new Thread(HandleConnections);
            connectionThread.Start();
        }


        //ServerSocket waits for the last client and then closes Thread for new Connections
        public void StopAcceptingClients()
        {
            Listener.Stop();

            connectionThread.Join();
        }


        private void HandleConnections()
        {
            try
            {
                while (true)
                {
                    TcpClient client = Listener.AcceptTcpClient();

                    SocketEvent socketEvent = new SocketEvent(client.GetStream());

                    OnClientIsConnected?.Invoke(socketEvent);
                }
            }
            catch (SocketException)
            { }
        }


        ~ServerSocketEvent()
        {
            Listener?.Stop();
        }
    }
}
