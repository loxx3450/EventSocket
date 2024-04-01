using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EventSocket
{
    public enum SocketType
    {
        Server,
        Client
    }

    public class Socket
    {
        public SocketType Type { get; set; }

        public TcpListener? Listener { get; set; }
        public List<TcpClient>? Clients { get; set; } = new List<TcpClient>();

        public TcpClient? Client { get; set; }

        public NetworkStream? Stream { get; set; }

        public Dictionary<string, Action<string>> Events { get; set; } = [];

        public Socket(SocketType socketType, string hostname, int port)
        {
            Type = socketType;

            if (Type == SocketType.Server)
            {
                Listener = new TcpListener(IPAddress.Parse(hostname), port);
                Listener.Start();

                _ = Task.Run(HandleConnections);
            }
            else
            {
                Client = new TcpClient();

                Client.Connect(hostname, port);

                Stream = Client.GetStream();

                _ = Task.Run(() => HandleRequests(Stream));
            }
        }

        public void HandleConnections()
        {
            while (true)
            {
                TcpClient tcpClient = Listener.AcceptTcpClient();

                Console.WriteLine($"Client {tcpClient.Client.RemoteEndPoint} is connected");

                Clients.Add(tcpClient);                                                             //lock?

                _ = Task.Run(() => HandleRequests(tcpClient.GetStream()));
            }
        }

        public void On(string key, Action<string> value)
        {
            Events[key] = value;
        }

        public void Emit(string key, string argument)
        {
            try
            {
                SocketMessage socketMessage = new(key, argument);

                if (Type == SocketType.Client)
                {
                    socketMessage.MemoryStream.CopyTo(Stream);
                }
                else if (Type == SocketType.Server)
                {
                    foreach (var client in Clients)
                    {
                        socketMessage.MemoryStream.CopyTo(client.GetStream());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }

        public void HandleRequests(NetworkStream stream)
        {
            while(true)
            {
                try
                {
                    int messageLength = ConvertToInt(ReadBytes(stream, 4));

                    using MemoryStream memoryStream = new MemoryStream(messageLength);
                    memoryStream.Write(ReadBytes(stream, messageLength), 0, messageLength);
                    memoryStream.Position = 0;

                    SocketMessage message = new SocketMessage(memoryStream);

                    if (Events.TryGetValue(message.Key, out Action<string> value))
                    {
                        value.Invoke(message.Argument);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                    stream.Close();
                    break;                                                                              //TODO
                }
            }

            byte[] ReadBytes(NetworkStream stream, int count)
            {

                byte[] bytes = new byte[count];
                stream.ReadExactly(bytes, 0, count);

                return bytes;
            }

            int ConvertToInt(byte[] bytes)
            {
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(bytes);

                return BitConverter.ToInt32(bytes, 0);
            }
        }

        ~Socket()
        {
            Stream?.Close();

            if (Clients is not null)                        //??
            {
                foreach(var client in Clients) 
                { 
                    client.Close();
                }
            }

            Listener?.Stop();
            Client?.Close();
        }
    }
}
