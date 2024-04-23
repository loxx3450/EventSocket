
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
        //
        // ========== public properties: ==========
        //

        /// <value>
        /// Represents the endpoint of Server.
        /// </value>
        public IPEndPoint EndPoint { get; private set; }


        //
        // ========== events: ==========
        //

        /// <summary>
        /// Is invoked, when ServerSocket receives new Connection.
        /// </summary>
        public event Action<SocketEvent>? OnClientIsConnected;


        //
        // ========== private fields: ==========
        //

        //TcpListener
        private readonly TcpListener listener;

        //Thread that gets new Connections
        private Thread connectionThread;


        //
        // ========== constructors: ==========
        //

        /// <summary>
        /// Initialized TcpListener starts to handle new <c>Connections</c>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the problem with 
        /// <paramref name="hostname"/> or <paramref name="port"/> 
        /// is occured
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the problem with 
        /// <paramref name="hostname"/> or <paramref name="port"/> 
        /// is occured
        /// </exception>
        /// <exception cref="SocketException">
        /// Thrown when the problem with 
        /// <paramref name="hostname"/> or <paramref name="port"/> 
        /// is occured
        /// </exception>
        /// <param name="hostname">hostname of Server.</param>
        /// <param name="port">port of Server.</param>
        public ServerSocketEvent(string hostname, int port)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(hostname), port);

            listener = new TcpListener(EndPoint);
            listener.Start();

            connectionThread = new Thread(HandleConnections);
        }


        //
        // ========== public methods: ==========
        //

        /// <summary>
        /// Gets async the first <c>Socket</c>, which will try to connect.
        /// </summary>
        /// <returns>The object of SocketEvent, that is based on Stream of Server.</returns>
        /// <exception cref="ServerSocketEventException">
        /// Occures when the <c>socket of Server</c> is closed 
        /// or doesn't accept new connections
        /// </exception>
        public async Task<SocketEvent> GetSocketAsync()
        {
            try
            {
                TcpClient client = await listener.AcceptTcpClientAsync();

                //Socket that is based on Stream To Client
                return new SocketEvent(client.GetStream());
            }
            catch (SocketException)
            {
                throw new ServerSocketEventException
                    (ServerSocketEventException.SERVER_SOCKET_CLOSED_LISTENER);
            }
        }


        /// <summary>
        /// <c>The Server</c> starts to accept new clients.
        /// The event <c>OnClientIsConnected</c> should be realized
        /// </summary>
        public void StartAcceptingClients()
        {
            connectionThread.Start();
        }

        /// <summary>
        /// Closes Thread, that accepts new Clients, by closing Listener
        /// </summary>
        public void StopAcceptingClients()
        {
            listener.Stop();

            connectionThread.Join();
        }


        //
        // ========== private fields: ==========
        //

        //Waits for new Client, creates new SocketEvent
        //and invokes Event, that new Client has connected
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


        //
        // ========== destructor: ==========
        //

        //Stops TcpListener and closes Thread
        ~ServerSocketEvent()
        {
            listener.Stop();

            if (connectionThread.ThreadState == ThreadState.Running) 
                connectionThread.Join();
        }
    }
}
