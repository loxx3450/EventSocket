using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EventSocket.Sockets
{
    public enum SocketType
    {
        ServerSocket,
        ClientSocket
    }

    public class Socket
    {
        //Stream for sending and getting Messages
        public NetworkStream NetworkStream { get; set; }

        //Dictionary of Events
        public Dictionary<string, Action<string>> Actions { get; set; } = [];

        public Socket(NetworkStream networkStream)
        {
            NetworkStream = networkStream;

            _ = Task.Run(HandleRequests);
        }

        public void On(string key, Action<string> action)
        {
            Actions[key] = action;
        }

        public void Emit(SocketMessageText socketMessage)
        {
            try
            {
                socketMessage.GetStream().CopyTo(NetworkStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }

        //Stream gets incoming messages, interprets them and executes suitable callback
        public void HandleRequests()
        {
            while (true)
            {
                try
                {
                    int messageLength = ConvertToInt(ReadBytes(4));

                    //Getting Stream which contains Message
                    using MemoryStream memoryStream = new MemoryStream(messageLength);
                    memoryStream.Write(ReadBytes(messageLength), 0, messageLength);
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
                    NetworkStream.Close();
                    break;                                                                              //TODO: how to close connection correctly
                }
            }

            byte[] ReadBytes(int count)
            {

                byte[] bytes = new byte[count];
                NetworkStream.ReadExactly(bytes, 0, count);

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
            NetworkStream?.Close();
        }
    }
}
