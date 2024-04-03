using EventSocket.SocketMessageCore;
using EventSocket.Sockets;
using TestSocketMessages;

const string hostname = "127.0.0.1";
const int port = 8080;

ClientSocket socket = new ClientSocket(hostname, port);

Socket client = await socket.GetSocket();

//Socket's setup
client.AddSupportedSocketMessageType<SocketMessageText>();
client.On("MessageToClient", (message) => Console.WriteLine($"From Server: {message};"));

//Messages to send(Important: other side should support this types fo SocketMessage's)
SocketMessageText messageText = new SocketMessageText("MessageToServer", "Hello");
SocketMessageInteger messageInteger = new SocketMessageInteger("IntegerToServer", 1234567890);

while (true)
{
    switch (Console.ReadLine())
    {
        case "1":
            client.Emit(messageInteger);
            break;
        default:
            client.Emit(messageText);
            break;
    }
    
}