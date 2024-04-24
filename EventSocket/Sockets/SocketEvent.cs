using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SocketEventLibrary.Exceptions;
using SocketEventLibrary.SocketEventMessageCore;

namespace SocketEventLibrary.Sockets
{
    /// <summary>
    /// Class <c>SocketEvent</c> provides methods and events 
    /// to communicate with the other side.
    /// </summary>
    public class SocketEvent
    {
        //
        // ========== public properties: ==========
        //

        /// <value>
        /// Stream for sending and getting Messages.
        /// </value>
        public NetworkStream NetworkStream { get; set; }


        //
        // ========== events: ==========
        //

        /// <summary>
        /// Event invokes when we catch the exception 
        /// that NetworkStream of <c>other side</c> is closed.
        /// </summary>
        public event Action<SocketEvent>? OnOtherSideIsDisconnected;

        /// <summary>
        /// Event invokes when we want to disconnect.
        /// </summary>
        public event Action<SocketEvent>? OnDisconnecting;

        /// <summary>
        /// Event invokes by catching Exception from SocketEventMessageBuilder.
        /// </summary>
        public event Action<SocketEventMessageBuilderException>? OnThrowedException;


        //
        // ========== private fields: ==========
        //

        //Dictionary of Events
        private Dictionary<object, Action<object>> actions = [];

        //Collection of supported SocketEventMessages's types
        private readonly List<Type> supportedMessagesTypes = [];

        //Task for getting incoming Messages
        private Task gettingRequests;


        //
        // ========== constructors: ==========
        //

        /// <summary>
        /// Initializes SocketEvent and starts to accept Messages
        /// </summary>
        /// <param name="networkStream">The NetworkStream of other side.</param>
        public SocketEvent(NetworkStream networkStream)
        {
            NetworkStream = networkStream;

            gettingRequests = Task.Run(HandleRequests);
        }


        //
        // ========== public methods: ==========
        //

        /// <summary>
        /// It will be possible to get Messages of types "T".
        /// </summary>
        /// <typeparam name="T">Type "T" is a type of Message that we can get from other side. 
        /// It should be a SocketEventMessage and realize interface IRecoverable</typeparam>
        public void AddSupportedMessageType<T>()
            where T : SocketEventMessage, IRecoverable
        {
            Type type = typeof(T);

            if (!supportedMessagesTypes.Contains(type))
                supportedMessagesTypes.Add(type);
        }


        /// <summary>
        /// Creates new pair key-callback
        /// </summary>
        /// <param name="key">Key of Action.</param>
        /// <param name="action">The callback, that will be called due to the Message's type(Key).</param>
        public void On(object key, Action<object> action)
        {
            actions[key] = action;
        }

        /// <summary>
        /// Sends Message to the NetStream of the other side.
        /// If NetStream is closed, handles the logic of <c>Disconnection</c>
        /// and closes NetStream.
        /// </summary>
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

        /// <summary>
        /// Calls the Event 'OnDisconnecting', if it's not null.
        /// </summary>
        public void Disconnect()
        {
            OnDisconnecting?.Invoke(this);
        }


        //
        // ========== private methods: ==========
        //

        //Stream gets incoming messages, interprets them and executes suitable callback
        private void HandleRequests()
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
        private void HandleDisconnection()
        {
            //Client's code should handle disconnection
            OnOtherSideIsDisconnected?.Invoke(this);
            NetworkStream.Close();

            gettingRequests.Dispose();
        }


        //
        // ========== destructor: ==========
        //

        //Closing Stream in case if he wasn't closed before
        ~SocketEvent()
        {
            NetworkStream.Close();
        }
    }
}
