using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using EventSocket.SocketEventMessageCore;

namespace EventSocket.Sockets
{
    public class SocketEvent
    {
        //Stream for sending and getting Messages
        public NetworkStream NetworkStream { get; set; }


        //Dictionary of Events
        public Dictionary<object, Action<object>> Actions { get; set; } = [];                               //TOTHINK: do we only work with Actions??


        //Collection of supported SocketEventMessages's types
        private List<Type> supportedMessagesTypes = new List<Type>();


        //Event invokes when we catch exception that NetworkStream is closed
        public event Action<SocketEvent> OnOtherSideIsDisconnected;

        public SocketEvent(NetworkStream networkStream)
        {
            NetworkStream = networkStream;

            _ = Task.Run(HandleRequests);
        }


        //This method belongs to SocketEvent's setup
        public void AddSupportedMessageType<T>() where T : SocketEventMessage
        {
            Type type = typeof(T);

            if (!supportedMessagesTypes.Contains(type))
                supportedMessagesTypes.Add(type);
        }


        //Creating new pair key-callback
        public void On(object key, Action<object> action)
        {
            Actions[key] = action;
        }


        //Sending Message to Stream of other size. In case, when Stream is closed, throwing exception and closing our own Stream
        public void Emit(SocketEventMessage message)
        {
            try
            {
                message.GetStream().CopyTo(NetworkStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");

                //Client's code should handle disconnection
                OnOtherSideIsDisconnected?.Invoke(this);
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
                    MemoryStream memoryStream = ReceiveMemoryStreamOfMessage();                                   //Possible BLOCKING

                    //Building concrete SocketEventMessage basing on received Stream
                    SocketEventMessage message = SocketEventMessageBuilder.GetSocketEventMessage(memoryStream, supportedMessagesTypes);

                    //Executing callback in case of containing received key
                    if (Actions.ContainsKey(message.Key))
                    {
                        Actions[message.Key].Invoke(message.Argument);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");

                    //Client's code should handle disconnection
                    OnOtherSideIsDisconnected.Invoke(this);
                    NetworkStream.Close();

                    break;                                                                              //TODO: how to close connection correctly
                }
            }
        }


        //Waits for incoming message, reads first 4 bytes to get size of Message, gets full Message and returns it
        private MemoryStream ReceiveMemoryStreamOfMessage()
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
        ~SocketEvent()
        {
            NetworkStream.Close();
        }
    }
}
