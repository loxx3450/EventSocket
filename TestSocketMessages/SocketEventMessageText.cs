using SocketEventLibrary.SocketEventMessageCore;
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

        public override MemoryStream GetDataStream()
        {
            MemoryStream memoryStream = new MemoryStream();

            //Writing key and payload
            using StreamWriter streamWriter = new StreamWriter(memoryStream, leaveOpen: true);
            streamWriter.WriteLine(Convert.ToString(Key) + '|' + Convert.ToString(Payload));
            streamWriter.Flush();

            memoryStream.Position = 0;

            return memoryStream;
        }

        public static SocketEventMessage RecoverSocketEventMessage(MemoryStream memoryStream)
        {
            using StreamReader reader = new StreamReader(memoryStream, leaveOpen: true);

            string? payload = reader.ReadLine() ?? throw new ArgumentException();

            string[] strings = payload.Split('|');

            return new SocketEventMessageText(strings[0], strings[1]);
        }
    }
}
