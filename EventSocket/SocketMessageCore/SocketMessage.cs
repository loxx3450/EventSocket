using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSocket.SocketMessageCore
{
    public abstract class SocketMessage
    {
        private string SocketMessageType;
        public object Key { get; set; }
        public object Argument { get; set; }
        
        public SocketMessage(MemoryStream stream)
        {
            SocketMessage socketMessage = ExtractSocketMessage(stream);

            Key = socketMessage.Key;
            Argument = socketMessage.Argument;
            SocketMessageType = socketMessage.SocketMessageType;
        }

        public SocketMessage(object key, object argument)
        {
            Key = key;
            Argument = argument;
            SocketMessageType = GetType().Name;
        }

        public MemoryStream GetStream()
        {
            MemoryStream memoryStream = new MemoryStream();

            //Empty first 4 bytes for describing messageLength
            memoryStream.Write(new byte[4], 0, 4);

            //Writing SocketMessageType
            using StreamWriter streamWriter = new StreamWriter(memoryStream, leaveOpen: true);
            streamWriter.WriteLine(SocketMessageType);
            streamWriter.Flush();

            //Writing PayloadStream
            GetPayloadStream().CopyTo(memoryStream);

            //Changing state of first 4 bytes
            memoryStream.Position = 0;

            int messageLength = (int)memoryStream.Length - 4;

            memoryStream.Write(ConvertIntToBytes(messageLength), 0, 4);
            memoryStream.Position = 0;

            return memoryStream;
        }

        //MemoryStream contains key and argument; Position should be equal 1
        public abstract MemoryStream GetPayloadStream();

        private byte[] ConvertIntToBytes(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        //SocketMessage should be built based on suitable Stream
        protected abstract SocketMessage ExtractSocketMessage(MemoryStream memoryStream);
    }
}
