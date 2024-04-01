using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventSocket.Sockets
{
    public class ClientSocket : Socket
    {
        public TcpClient Client { get; set; }
        public NetworkStream ServerStream { get; set; }

        public ClientSocket(string hostname, int port)
            : base(hostname, port)
        { }

        public override void Init(string hostname, int port)
        {
            Client = new TcpClient();

            Client.Connect(hostname, port);

            ServerStream = Client.GetStream();

            //Client is waiting for incoming messages
            _ = Task.Run(() => HandleRequests(ServerStream));
        }

        //Sending Message to Server
        protected override void SendMessage(SocketMessageText socketMessage)
        {
            socketMessage.GetStream().CopyTo(ServerStream);
        }

        ~ClientSocket()
        {
            ServerStream?.Close();
            Client?.Close();
        }
    }
}
