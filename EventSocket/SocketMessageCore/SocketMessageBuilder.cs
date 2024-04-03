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
        //On this phase MemoryStream contains socketMessageType as a string and payload
        public static SocketMessage GetSocketMessage(MemoryStream stream, List<Type> supportedTypes)
        {
            //Read SocketMessageType from stream
            string socketMessageType = ReadSocketMessageType(stream);

            stream.Position = socketMessageType.Length + 2;                                                         //TODO:TEMP

            //Getting Type of received SocketMessage
            Type type = GetTypeOfReceivedMessage(supportedTypes, socketMessageType);

            //We call constructor for concrete SocketMessage which will be build based on MemoryStream of Payload
            return Activator.CreateInstance(type, stream) as SocketMessage ?? throw new Exception();
        }


        //Getting string implementation of sent SocketMessage's Type
        private static string ReadSocketMessageType(MemoryStream stream) 
        {
            using StreamReader reader = new StreamReader(stream, leaveOpen: true);

            return reader.ReadLine() ?? throw new Exception();
        }


        //Trying to find the type of received Message in the collection of Supported Types by Socket
        private static Type GetTypeOfReceivedMessage(List<Type> supportedTypes, string socketMessageType)
        {
            foreach (var t in supportedTypes)
            {
                if (t.Name == socketMessageType)
                {
                    return t;
                }
            }

            throw new Exception();
        }
    }
}
