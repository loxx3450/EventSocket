using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using EventSocket.SocketMessageCore;

namespace EventSocket.Sockets
{
    public class Socket
    {
        //Stream for sending and getting Messages
        public NetworkStream NetworkStream { get; set; }


        //Dictionary of Events
        public Dictionary<object, Action<object>> Actions { get; set; } = [];                               //TOTHINK: do we only work with Actions??


        //Collection of supported SocketMessages
        private List<Type> socketMessagesTypes = new List<Type>();


        public Socket(NetworkStream networkStream)
        {
            NetworkStream = networkStream;

            _ = Task.Run(HandleRequests);
        }


        //This method belongs to Socket's setup
        public void AddSupportedSocketMessageType<T>() where T : SocketMessage
        {
            Type type = typeof(T);

            if (!socketMessagesTypes.Contains(type))
                socketMessagesTypes.Add(type);
        }


        //Creating new pair key-callback
        public void On(object key, Action<object> action)
        {
            Actions[key] = action;
        }


        //Sending Message to Stream of other size. In case, when Stream is closed, throwing exception and closing our own Stream
        public void Emit(SocketMessage socketMessage)
        {
            try
            {
                socketMessage.GetStream().CopyTo(NetworkStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                NetworkStream.Close();
            }
        }


        //Stream gets incoming messages, interprets them and executes suitable callback
        public void HandleRequests()
        {
            while (true)
            {
                try
                {
                    MemoryStream memoryStream = ReceiveMemoryStreamOfSocketMessage();                                   //Possible BLOCKING

                    //Building concrete SocketMessage basing on received Stream
                    SocketMessage message = SocketMessageBuilder.GetSocketMessage(memoryStream, socketMessagesTypes);

                    //Executing callback in case of containing received key
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
        }


        //Waits for incoming message, reads first 4 bytes to get size of Message, gets full Message and returns it
        private MemoryStream ReceiveMemoryStreamOfSocketMessage()
        {
            int messageLength = ConvertToInt(ReadBytes(4));

            //Getting Stream which contains Message
            MemoryStream memoryStream = new MemoryStream(messageLength);
            memoryStream.Write(ReadBytes(messageLength), 0, messageLength);
            memoryStream.Position = 0;

            return memoryStream;


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


        //Closing Stream in case if he wasn't closed before
        ~Socket()
        {
            NetworkStream?.Close();
        }
    }
}
