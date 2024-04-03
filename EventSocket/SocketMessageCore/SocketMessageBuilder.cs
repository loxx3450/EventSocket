using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventSocket.SocketMessageCore
{
    internal static class SocketMessageBuilder
    {
        public static SocketMessage GetSocketMessage(MemoryStream stream, List<Type> supportedTypes)
        {
            using StreamReader reader = new StreamReader(stream, leaveOpen: true);
            string? socketMessageType = reader.ReadLine() ?? throw new Exception();                                   //TODO:TEMP

            stream.Position = socketMessageType.Length + 2;

            Type? type = null;

            foreach (var t in supportedTypes)
            {
                if (t.Name == socketMessageType)
                {
                    type = t;
                    break;
                }
            }

            if (type is null)
                throw new Exception();

            return Activator.CreateInstance(type, stream) as SocketMessage ?? throw new Exception();
        }
    }
}
