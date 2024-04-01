using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSocket
{
    public abstract class ISocketMessage<T, K>
    {
        public T Key { get; set; }
        public K Argument { get; set; }

        public ISocketMessage(MemoryStream stream)
        {
            ISocketMessage<T, K> socketMessage = ExtractSocketMessage(stream);

            Key = socketMessage.Key;
            Argument = socketMessage.Argument;
        }

        public ISocketMessage(T key, K argument)
        {
            Key = key;
            Argument = argument;
        }

        public abstract MemoryStream GetStream();

        //SocketMessage should be built based on suitable Stream
        protected abstract ISocketMessage<T, K> ExtractSocketMessage(MemoryStream memoryStream);
    }
}
