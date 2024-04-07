using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SocketEventLibrary.Exceptions;
using SocketEventLibrary.SocketEventMessageCore;

namespace SocketEventLibrary.Sockets
{
    public class SocketEvent
    {
        //Stream for sending and getting Messages
        public NetworkStream NetworkStream { get; set; }


        //Dictionary of Events
        public Dictionary<object, Action<object>> actions = [];


        //Collection of supported SocketEventMessages's types
        private readonly List<Type> supportedMessagesTypes = [];


        //Event invokes when we catch exception that NetworkStream is closed
        public event Action<SocketEvent>? OnOtherSideIsDisconnected;

        //Event invokes when we want to disconnect
        public event Action<SocketEvent>? OnDisconnecting;

        //Event invokes by catching Exception from Builder
        public event Action<SocketEventMessageBuilderException>? OnThrowedException;


        public SocketEvent(NetworkStream networkStream)
        {
            NetworkStream = networkStream;

            _ = Task.Run(HandleRequests);
        }


        //This method belongs to SocketEvent's setup
        public void AddSupportedMessageType<T>()
            where T : SocketEventMessage, IRecoverable
        {
            Type type = typeof(T);

            if (!supportedMessagesTypes.Contains(type))
                supportedMessagesTypes.Add(type);
        }


        //Creating new pair key-callback
        public void On(object key, Action<object> action)
        {
            actions[key] = action;
        }


        //Sending Message to Stream of the other Side. In case, when Stream is closed, throwing exception and closing our own Stream
        public void Emit(SocketEventMessage message)
        {
            try
            {
                message.GetStream().CopyTo(NetworkStream);
            }
            catch (Exception)
            {
                HandleDisconnection();
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
                    if (actions.ContainsKey(message.Key))
                    {
                        actions[message.Key].Invoke(message.Payload);
                    }

                    memoryStream.Close();
                }
                catch (SocketEventMessageBuilderException ex)
                {
                    OnThrowedException?.Invoke(ex);
                }
                catch (Exception)
                {
                    HandleDisconnection();
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


        //Method is called when the other side is not more available
        public void HandleDisconnection()
        {
            //Client's code should handle disconnection
            OnOtherSideIsDisconnected?.Invoke(this);
            NetworkStream.Close();
        }


        //User should describe logic of Disconnection and execute it to close Connection
        public void Disconnect()
        {
            OnDisconnecting?.Invoke(this);
        }


        //Closing Stream in case if he wasn't closed before
        ~SocketEvent()
        {
            NetworkStream.Close();
        }
    }
}
