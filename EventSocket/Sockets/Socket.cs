using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EventSocket.Sockets
{
    public abstract class Socket
    {
        //Dictionary of Events
        public Dictionary<string, Action<string>> Actions { get; set; } = [];

        public Socket(string hostname, int port)
        {
            Init(hostname, port);
        }

        public abstract void Init(string hostname, int port);

        public void On(string key, Action<string> action)
        {
            Actions[key] = action;
        }

        public void Emit(SocketMessageText socketMessage)
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

        protected abstract void SendMessage(SocketMessageText socketMessage);

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

                    #region interpretation in case of genetic message
                    ////Interpretation                                                    //TODO: should be automatic
                    //object key = null!;
                    //object argument = null!;

                    //if (nameof(T) is string && nameof(K) is string)
                    //{
                    //    SocketMessageText message = new SocketMessageText(memoryStream);

                    //    key = message.Key;
                    //    argument = message.Argument;
                    //}
                    //else
                    //{
                    //    continue;
                    //}
                    #endregion

                    SocketMessageText message = new SocketMessageText(memoryStream);

                    //Executing callback
                    if (Actions.ContainsKey(message.Key))
                    {
                        Actions[message.Key].Invoke(message.Argument);
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
