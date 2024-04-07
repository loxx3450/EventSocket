using EventSocket.SocketEventMessageCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSocketEventMessages
{
    public class SocketEventMessageText : SocketEventMessage, IRecoverable
    {
        public SocketEventMessageText(string key, string argument)
            : base(key, argument)
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

        public static SocketEventMessage RecoverSocketEventMessage(MemoryStream memoryStream)
        {
            using StreamReader reader = new StreamReader(memoryStream, leaveOpen: true);
            string? message = reader.ReadLine();

            if (message == null)
                throw new ArgumentException();

            string[] strings = message.Split('|');

            return new SocketEventMessageText(strings[0], strings[1]);
        }
    }
}
