using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketEventLibrary.SocketEventMessageCore
{
    public abstract class SocketEventMessage
    {
        private readonly string messageType;


        //Main Data of Message
        public object Key { get; set; }
        public object Payload { get; set; }
        

        //Key and Payload should be initialized
        public SocketEventMessage(object key, object payload)
        {
            Key = key;
            Payload = payload;
            messageType = GetType().Name;
        }


        //Every SocketEventMessage must have his Stream-implementation
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


        //Writing MessageType with StreamWriter
        private void WriteMessageType(MemoryStream memoryStream)
        {
            using StreamWriter streamWriter = new StreamWriter(memoryStream, leaveOpen: true);
            streamWriter.WriteLine(messageType);
            streamWriter.Flush();
        }


        //MemoryStream contains key and payload; Position should be equal zero
        public abstract MemoryStream GetDataStream();


        //Changing state of first 4 bytes
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
