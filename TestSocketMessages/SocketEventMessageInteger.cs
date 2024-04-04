using EventSocket.SocketEventMessageCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSocketEventMessages
{
    public class SocketEventMessageInteger : SocketEventMessage
    {
        public SocketEventMessageInteger(string key, int argument)
            : base(key, argument)
        { }

        public SocketEventMessageInteger(MemoryStream stream)
            : base(stream)
        { }

        public override MemoryStream GetPayloadStream()
        {
            MemoryStream memoryStream = new MemoryStream();

            string payload = Convert.ToString(Key) + '|' + Convert.ToString(Convert.ToInt32(Argument));

            using StreamWriter streamWriter = new StreamWriter(memoryStream, leaveOpen: true);
            streamWriter.WriteLine(payload);
            streamWriter.Flush();

            memoryStream.Position = 0;

            return memoryStream;
        }

        protected override SocketEventMessage ExtractSocketEventMessage(MemoryStream memoryStream)
        {
            using StreamReader streamReader = new StreamReader(memoryStream, leaveOpen: true);

            string? payload = streamReader.ReadLine() ?? throw new ArgumentException();

            string[] strings = payload.Split('|');

            return new SocketEventMessageInteger(strings[0], Convert.ToInt32(strings[1]));
        }
    }
}
