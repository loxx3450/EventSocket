using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketEventLibrary.SocketEventMessageCore
{
    /// <summary>
    /// Class <c>SocketEventMessage</c> models a pair of <c>Key</c> and <c>Payload</c>
    /// </summary>
    public abstract class SocketEventMessage
    {
        //
        // ========== public properties: ==========
        //

        /// <value>
        /// Property <c>Key</c> represents the identifier of SocketEventMessage and could be represented as any type.
        /// </value>
        public object Key { get; set; }

        /// <value>
        /// Property <c>Payload</c> represents the payload(argument) of SocketEventMessage and could be represented as any type.
        /// </value>
        public object Payload { get; set; }


        //
        // ========== private fields: ==========
        //

        //Should be written at Stream
        private readonly string messageType;


        //
        // ========== constructors: ==========
        //

        /// <summary>
        /// Initializes <c>Key</c> and <c>Payload</c>.
        /// </summary>
        /// <param name="key">Identifier of Message.</param>
        /// <param name="payload">Payload(argument) of Message</param>
        public SocketEventMessage(object key, object payload)
        {
            Key = key;
            Payload = payload;
            messageType = GetType().Name;
        }


        //
        // ========== abstract methods: ==========
        //

        /// <summary>
        /// Builds MemoryStream that is based on 
        /// SocketEventMessage's <c>Key</c> and <c>Payload</c>. 
        /// MemoryStream should contain key and payload
        /// and the position of Stream should be equal zero.
        /// </summary>
        /// <returns>The instance of MemoryStream.</returns>
        protected abstract MemoryStream GetDataStream();


        //
        // ========== public methods: ==========
        //

        /// <summary>
        /// Builds MemoryStream that is based on the whole
        /// SocketEventMessage. Executes abstract method 'GetDataStream'.
        /// </summary>
        /// <returns>The instance of MemoryStream 
        /// with the whole info about Message.</returns>
        public MemoryStream GetStream()
        {
            MemoryStream memoryStream = new MemoryStream();

            //Empty first 4 bytes for describing messageLength
            memoryStream.Write(new byte[4], 0, 4);

            //Writing MessageType
            WriteMessageType(memoryStream);

            //Writing DataStream
            GetDataStream().CopyTo(memoryStream);

            //Changing state of MessageLength (is null at the moment)
            ChangeMessageLengthState(memoryStream);

            memoryStream.Position = 0;

            return memoryStream;
        }


        //
        // ========== private methods: ==========
        //

        //Writes MessageType with StreamWriter
        private void WriteMessageType(MemoryStream memoryStream)
        {
            using StreamWriter streamWriter = new StreamWriter(memoryStream, leaveOpen: true);
            streamWriter.WriteLine(messageType);
            streamWriter.Flush();
        }

        //Changes state of first 4 bytes
        private void ChangeMessageLengthState(MemoryStream memoryStream)
        {
            memoryStream.Position = 0;

            int messageLength = (int)memoryStream.Length - 4;

            memoryStream.Write(ConvertIntToBytes(messageLength), 0, 4);


            byte[] ConvertIntToBytes(int value)
            {
                byte[] bytes = BitConverter.GetBytes(value);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(bytes);

                return bytes;
            }
        }
    }
}
