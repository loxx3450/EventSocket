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

    public class Socket<T,K>
    {
        public SocketType Type { get; set; }

        //Server Side
        public TcpListener? Listener { get; set; }
        public List<TcpClient>? Clients { get; set; } = new List<TcpClient>();

        //Client Side
        public TcpClient? Client { get; set; }
        public NetworkStream? Stream { get; set; }

        //Dictionary of Events
        public Dictionary<T, Action<K>> Events { get; set; } = [];

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

                //Client is waiting for incoming messages
                _ = Task.Run(() => HandleRequests(Stream));
            }
        }

        //Server gets new Clients and submits them to processing
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

        public void On(T key, Action<K> value)
        {
            Events[key] = value;
        }

        public void Emit(ISocketMessage<T, K> socketMessage)
        {
            try
            {
                if (Type == SocketType.Client)
                {
                    //Sending Message to Server
                    socketMessage.GetStream().CopyTo(Stream);
                }
                else if (Type == SocketType.Server)
                {
                    //Sending Message to everybody who is connected to Server
                    foreach (var client in Clients)
                    {
                        socketMessage.GetStream().CopyTo(client.GetStream());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }

        //Stream gets incoming messages, interprets them and executes suitable callback
        public void HandleRequests(NetworkStream stream)
        {
            while(true)
            {
                try
                {
                    int messageLength = ConvertToInt(ReadBytes(stream, 4));

                    //Getting Stream which contains Message
                    using MemoryStream memoryStream = new MemoryStream(messageLength);
                    memoryStream.Write(ReadBytes(stream, messageLength), 0, messageLength);
                    memoryStream.Position = 0;

                    //Interpretation                                                    //TODO: should be automatic
                    object key = null!;
                    object argument = null!;

                    if (nameof(T) is string && nameof(K) is string)
                    {
                        SocketMessage message = new SocketMessage(memoryStream);

                        key = message.Key;
                        argument = message.Argument;
                    }
                    else
                    {
                        continue;
                    }

                    //Executing callback
                    Action<K> value;

                    if (Events.TryGetValue((T)key, out value))
                    {
                        value.Invoke((K)argument);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                    stream.Close();
                    break;                                                                              //TODO: how to close connection correctly
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
            if (Client is not null)
            {
                Stream?.Close();

                Client?.Close();
            }

            if (Listener is not null)
            {
                Listener?.Stop();

                foreach (var client in Clients)                 //??
                { 
                    client.Close();
                }
            }

            
        }
    }
}
