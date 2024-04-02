using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using EventSocket.SocketMessages;

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
        public Dictionary<object, Action<object>> Actions { get; set; } = [];                               //TOTHINK: do we only work with Actions??

        public Socket(NetworkStream networkStream)
        {
            NetworkStream = networkStream;

            _ = Task.Run(HandleRequests);
        }

        public void On(object key, Action<object> action)
        {
            Actions[key] = action;
        }

        public void Emit(SocketMessage socketMessage)
        {
            try
            {
                socketMessage.GetStream().CopyTo(NetworkStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");                                                              //TODO:close connection
            }
        }

        //Stream gets incoming messages, interprets them and executes suitable callback
        public void HandleRequests()
        {
            SocketMessageBuilder builder = new SocketMessageBuilder();

            while (true)
            {
                try
                {
                    int messageLength = ConvertToInt(ReadBytes(4));

                    //Getting Stream which contains Message
                    using MemoryStream memoryStream = new MemoryStream(messageLength);
                    memoryStream.Write(ReadBytes(messageLength), 0, messageLength);
                    memoryStream.Position = 0;

                    SocketMessage message = builder.GetSocketMessage(memoryStream);                                         //TODO:static??

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
