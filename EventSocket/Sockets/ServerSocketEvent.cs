
using SocketEventLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketEventLibrary.Sockets
{
    public class ServerSocketEvent
    {
        //TcpListener
        public IPEndPoint EndPoint { get; private set; }
        private readonly TcpListener listener;


        //Thread that gets new Connections
        private Thread connectionThread;


        //Invokes when ServerSocket receives new Connection
        public event Action<SocketEvent>? OnClientIsConnected;


        public ServerSocketEvent(string hostname, int port)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(hostname), port);

            listener = new TcpListener(EndPoint);
            listener.Start();

            connectionThread = new Thread(HandleConnections);
        }


        //Getting first Socket, which will try to connect
        public async Task<SocketEvent> GetSocketAsync()
        {
            try
            {
                TcpClient client = await listener.AcceptTcpClientAsync();

                //Socket that is based on Stream To Client
                return new SocketEvent(client.GetStream());
            }
            catch (SocketException ex) 
            {
                throw new ServerSocketEventException(ServerSocketEventException.SERVER_SOCKET_CLOSED_LISTENER);
            }
        }


        //User should be subscribed on the event OnClientIsConnected
        public void StartAcceptingClients()
        {
            connectionThread.Start();
        }
        

        //Closing Thread, that accepts new Clients, by closing Listener
        public void StopAcceptingClients()
        {
            listener.Stop();

            connectionThread.Join();
        }


        //Waiting for new Client, creating new SocketEvent and invoking Event, that new Client has connected
        private void HandleConnections()
        {
            try
            {
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();

                    SocketEvent socketEvent = new SocketEvent(client.GetStream());

                    OnClientIsConnected?.Invoke(socketEvent);
                }
            }
            catch (SocketException)
            { }
        }


        ~ServerSocketEvent()
        {
            listener.Stop();

            if (connectionThread.ThreadState == ThreadState.Running) 
                connectionThread.Join();
        }
    }
}
