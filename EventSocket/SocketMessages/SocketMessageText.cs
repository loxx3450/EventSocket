using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSocket.SocketMessages
{
    public class SocketMessageText : SocketMessage
    {
        public SocketMessageText(string key, string argument)
            : base(key, argument, "SocketMessageText")
        { }

        public SocketMessageText(MemoryStream stream)
            : base(stream)
        { }

        public override MemoryStream GetPayloadStream()
        {
            MemoryStream memoryStream = new MemoryStream();

            //Writing key and argument
            using StreamWriter streamWriter = new StreamWriter(memoryStream, leaveOpen: true);
            streamWriter.WriteLine(Convert.ToString(Key) + '|' + Convert.ToString(Argument));
            streamWriter.Flush();

            memoryStream.Position = 0;

            return memoryStream;
        }

        protected override SocketMessageText ExtractSocketMessage(MemoryStream memoryStream)
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
