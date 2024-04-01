using EventSocket;
using EventSocket.Sockets;

const string hostname = "127.0.0.1";
const int port = 8080;

Socket<string, string> socket = new ClientSocket<string, string>(hostname, port);

socket.On("MessageToClient", (message) => Console.WriteLine($"From Server: {message};"));

SocketMessageText message = new SocketMessageText("MessageToServer", "Hello");

while (true)
{
    Console.ReadLine();
    socket.Emit(message);
}