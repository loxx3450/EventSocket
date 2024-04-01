using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSocket
{
    internal class SocketMessage
    {
        public string Key { get; set; }
        public string Argument { get; set; }        //temp
        public MemoryStream MemoryStream { get; set; }

        public SocketMessage(string key, string argument)
        {
            Key = key;
            Argument = argument;

            BuildStream();
        }

        public SocketMessage(MemoryStream stream)
        {
            SocketMessage message = BuildSocketMessage(stream);

            Key = message.Key;
            Argument = message.Argument;
            MemoryStream = message.MemoryStream;
        }

        private void BuildStream()
        {
            MemoryStream = new MemoryStream();

            MemoryStream.Write(new byte[4], 0, 4);

            using StreamWriter streamWriter = new(MemoryStream, leaveOpen: true);
            streamWriter.WriteLine(Key + '|' + Argument);
            streamWriter.Flush();

            MemoryStream.Position = 0;
            int messageLength = (int)MemoryStream.Length - 4;

            MemoryStream.Write(ConvertIntToBytes(messageLength), 0, 4);
            MemoryStream.Position = 0;
        }

        private SocketMessage BuildSocketMessage(MemoryStream stream)
        {
            using StreamReader reader = new StreamReader(stream, leaveOpen: true);
            string? message = reader.ReadLine();

            if (message == null)
                throw new ArgumentException();
            
            string[] strings = message.Split('|');

            return new SocketMessage(strings[0], strings[1]);
        }

        private byte[] ConvertIntToBytes(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }
    }
}
