using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EventSocket.SocketMessages;

namespace EventSocket
{
    internal class SocketMessageBuilder
    {
        public SocketMessage GetSocketMessage(MemoryStream stream)
        {
            using StreamReader reader = new StreamReader(stream, leaveOpen: true);
            string? socketMessageType = reader.ReadLine();
            stream.Position = socketMessageType.Length + 2;                     //TODO:TEMP - should be fixed:stackOverflow

            if (socketMessageType is null)
                throw new ArgumentException();

            Assembly assembly = typeof(SocketMessage).Assembly;
            Type[] types = assembly.GetTypes();
            Type? type = types.First(t => t.Name == socketMessageType);                     //TODO: should not depend on assembly
            //Type? type = assembly.GetType(socketMessageType);

            if (type is null)
                throw new ArgumentException();

            return Activator.CreateInstance(type, stream) as SocketMessage;
        }
    }
}
