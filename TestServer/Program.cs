using EventSocket;
using EventSocket.Sockets;

const string hostname = "127.0.0.1";
const int port = 8080;

Socket<string, string> socket = new ServerSocket<string, string>(hostname, port);

socket.On("MessageToServer", (message) => Console.WriteLine($"From Client: {message};"));

SocketMessageText message = new SocketMessageText("MessageToClient", "Hello");

while (true)
{
    Console.ReadLine();
    socket.Emit(message);
}