using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSocket
{
    public class SocketMessageText : SocketMessage<string, string>
    {
        public SocketMessageText(string key, string argument) : base(key, argument)
        { }

        public SocketMessageText(MemoryStream stream) : base(stream) 
        { }

        public override MemoryStream GetStream()
        {
            MemoryStream memoryStream = new MemoryStream();

            //Empty first 4 bytes for describing messageLength
            memoryStream.Write(new byte[4], 0, 4);

            //Writing key and argument
            using StreamWriter streamWriter = new(memoryStream, leaveOpen: true);
            streamWriter.WriteLine(Key + '|' + Argument);
            streamWriter.Flush();

            //Changing state of first 4 bytes
            memoryStream.Position = 0;

            int messageLength = (int)memoryStream.Length - 4;

            memoryStream.Write(ConvertIntToBytes(messageLength), 0, 4);
            memoryStream.Position = 0;

            return memoryStream;


            byte[] ConvertIntToBytes(int value)
            {
                byte[] bytes = BitConverter.GetBytes(value);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(bytes);

                return bytes;
            }
        }

        protected override SocketMessage<string, string> ExtractSocketMessage(MemoryStream memoryStream)
        {
            //Depends on types
            using StreamReader reader = new StreamReader(memoryStream, leaveOpen: true);
            string? message = reader.ReadLine();

            if (message == null)
                throw new ArgumentException();

            string[] strings = message.Split('|');

            return new SocketMessageText(strings[0], strings[1]);
        }
    }
}
