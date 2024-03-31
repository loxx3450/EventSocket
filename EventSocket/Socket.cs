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
        public TcpListener? Listener;
        public TcpClient? Client;

        public NetworkStream Stream;

        public StreamReader Reader;
        public StreamWriter Writer;

        public Dictionary<string, Action<string>> Events = new();

        public Socket(SocketType socketType, string hostname, int port)
        {
            if (socketType == SocketType.Server)
            {
                Listener = new TcpListener(IPAddress.Parse(hostname), port);
                Listener.Start();

                TcpClient tcpClient = Listener.AcceptTcpClient();

                Stream = tcpClient.GetStream();
            }
            else
            {
                Client = new TcpClient();

                Client.Connect(hostname, port);

                Stream = Client.GetStream();
            }

            Reader = new StreamReader(Stream);
            Writer = new StreamWriter(Stream);

            _ = Task.Run(() => HandleRequests());
        }

        public void On(string key, Action<string> value)
        {
            Events[key] = value;
        }

        public void Emit(string key, string argument)
        {
            Writer.WriteLine(key + '|' + argument);
            Writer.Flush();
        }

        public void HandleRequests()
        {
            while(true)
            {
                string? message = Reader.ReadLine();

                if (message is not null)
                {
                    string[] strings = message.Split('|');

                    Events[strings[0]].Invoke(strings[1]);
                }
            }
        }

        ~Socket()
        {
            Writer.Close();
            Reader.Close();
            Stream.Close();

            if (Listener is not null)
                Listener.Stop();
            else if (Client is not null)
                Client.Close();
        }
    }
}
