using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSocket.SocketEventMessageCore
{
    public abstract class SocketEventMessage
    {
        private readonly string MessageType;
        public object Key { get; set; }
        public object Argument { get; set; }
        

        //Both constructors should be realized in the derived classes
        public SocketEventMessage(object key, object argument)
        {
            Key = key;
            Argument = argument;
            MessageType = GetType().Name;
        }


        public MemoryStream GetStream()
        {
            MemoryStream memoryStream = new MemoryStream();

            //Empty first 4 bytes for describing messageLength
            memoryStream.Write(new byte[4], 0, 4);

            //Writing MessageType
            WriteMessageType(memoryStream);

            //Writing PayloadStream
            GetPayloadStream().CopyTo(memoryStream);

            //Changing state of MessageLength (is null at the moment)
            ChangeMessageLengthState(memoryStream);

            memoryStream.Position = 0;

            return memoryStream;
        }


        //Writing MessageType with StreamWriter
        private void WriteMessageType(MemoryStream memoryStream)
        {
            using StreamWriter streamWriter = new StreamWriter(memoryStream, leaveOpen: true);
            streamWriter.WriteLine(MessageType);
            streamWriter.Flush();
        }


        //MemoryStream contains key and argument; Position should be equal 1
        public abstract MemoryStream GetPayloadStream();


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
