using EventSocket.SocketMessages;
using EventSocket.Sockets;

const string hostname = "127.0.0.1";
const int port = 8080;

ClientSocket socket = new ClientSocket(hostname, port);

Socket client = await socket.GetSocket();

client.On("MessageToClient", (message) => Console.WriteLine($"From Server: {message};"));

SocketMessageText message = new SocketMessageText("MessageToServer", "Hello");

while (true)
{
    Console.ReadLine();
    client.Emit(message);
}