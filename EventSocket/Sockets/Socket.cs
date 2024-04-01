using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EventSocket.Sockets
{
    public abstract class Socket<T, K>
    {
        //Dictionary of Events
        public Dictionary<T, Action<K>> Events { get; set; } = [];

        public Socket(string hostname, int port)
        {
            Init(hostname, port);
        }

        public abstract void Init(string hostname, int port);

        public void On(T key, Action<K> value)
        {
            Events[key] = value;
        }

        public void Emit(SocketMessage<T, K> socketMessage)
        {
            try
            {
                SendMessage(socketMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }

        protected abstract void SendMessage(SocketMessage<T, K> socketMessage);


        //Stream gets incoming messages, interprets them and executes suitable callback
        public void HandleRequests(NetworkStream stream)
        {
            while (true)
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
                        SocketMessageText message = new SocketMessageText(memoryStream);

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
    }
}
