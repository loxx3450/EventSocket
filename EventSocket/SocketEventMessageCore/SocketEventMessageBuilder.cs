using SocketEventLibrary.Exceptions;
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
        //
        // ========== private fields: ==========
        //

        private const string METHOD_NAME = "RecoverSocketEventMessage";


        //
        // ========== public methods: ==========
        //

        /// <summary>
        /// Finds a suitable type of SocketEventMessage by reading Stream 
        /// and builds the inheritance of SocketEventMessage.
        /// Otherwise, throws exception.
        /// </summary>
        /// <returns>The inheritance of SocketEventMessage, based on incoming MemoryStream.</returns>
        /// <exception cref="SocketEventMessageBuilderException">
        /// Thrown when the Builder couldn't find the suitable type.
        /// </exception>
        //On this phase MemoryStream contains messageType as a string and payload
        public static SocketEventMessage GetSocketEventMessage(MemoryStream stream, List<Type> supportedTypes)
        {
            //Read SocketEventMessage's MessageType from stream
            string messageType = ReadMessageType(stream);

            stream.Position = messageType.Length + 2;                                                         //TODO: use something normal

            //Getting Type of received SocketMessage
            Type type = GetTypeOfReceivedMessage(supportedTypes, messageType);

            //Preparations for calling Method
            MethodInfo? method = type.GetMethod(METHOD_NAME);
            object[] args = new object[1] { stream };

            //We call static method to get concrete SocketEventMessage from MemoryStream
            return method?.Invoke(null, args) as SocketEventMessage ??
                throw new SocketEventMessageBuilderException
                    (SocketEventMessageBuilderException.BUILDER_METHOD_RETURNED_NULL);
        }


        //
        // ========== private methods: ==========
        //

        //Getting string implementation of received SocketEventMessage's Type
        private static string ReadMessageType(MemoryStream stream) 
        {
            using StreamReader reader = new StreamReader(stream, leaveOpen: true);

            return reader.ReadLine() ??
                throw new SocketEventMessageBuilderException
                    (SocketEventMessageBuilderException.BUILDER_MESSAGE_TYPE_NOT_FOUND);
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

            throw new SocketEventMessageBuilderException
                (SocketEventMessageBuilderException.BUILDER_NOT_SUPPORTED_TYPE);
        }
    }
}
