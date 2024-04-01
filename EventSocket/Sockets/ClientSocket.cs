using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventSocket.Sockets
{
    public class ClientSocket<T, K> : Socket<T, K>
    {
        public TcpClient Client { get; set; }
        public NetworkStream Stream { get; set; }

        public ClientSocket(string hostname, int port)
            : base(hostname, port)
        { }

        public override void Init(string hostname, int port)
        {
            Client = new TcpClient();

            Client.Connect(hostname, port);

            Stream = Client.GetStream();

            //Client is waiting for incoming messages
            _ = Task.Run(() => HandleRequests(Stream));
        }

        //Sending Message to Server
        protected override void SendMessage(SocketMessage<T, K> socketMessage)
        {
            socketMessage.GetStream().CopyTo(Stream);
        }

        ~ClientSocket()
        {
            Stream?.Close();
            Client?.Close();
        }
    }
}
