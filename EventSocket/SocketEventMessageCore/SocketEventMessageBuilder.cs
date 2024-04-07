using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SocketEventLibrary.SocketEventMessageCore
{
    internal static class SocketEventMessageBuilder
    {
        //On this phase MemoryStream contains messageType as a string and payload
        public static SocketEventMessage GetSocketEventMessage(MemoryStream stream, List<Type> supportedTypes)
        {
            //Read SocketEventMessage's MessageType from stream
            string messageType = ReadMessageType(stream);

            stream.Position = messageType.Length + 2;                                                         //TODO:TEMP

            //Getting Type of received SocketMessage
            Type type = GetTypeOfReceivedMessage(supportedTypes, messageType);

            MethodInfo method = type.GetMethod("RecoverSocketEventMessage");

            object[] args = new object[1] { stream };

            //We call constructor for concrete SocketEventMessage which will be build based on MemoryStream of Payload
            return method.Invoke(null, args) as SocketEventMessage ?? throw new Exception();
        }


        //Getting string implementation of sent SocketEventMessage's Type
        private static string ReadMessageType(MemoryStream stream) 
        {
            using StreamReader reader = new StreamReader(stream, leaveOpen: true);

            return reader.ReadLine() ?? throw new Exception();
        }


        //Trying to find the type of received Message in the collection of Supported Types by Socket
        private static Type GetTypeOfReceivedMessage(List<Type> supportedTypes, string receivedMessageType)
        {
            foreach (var t in supportedTypes)
            {
                if (t.Name == receivedMessageType)
                {
                    return t;
                }
            }

            throw new InvalidCastException();
        }
    }
}
