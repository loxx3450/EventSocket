using EventSocket;

const string hostname = "127.0.0.1";
const int port = 8080;

Socket<string, string> socket = new Socket<string, string>(SocketType.Client, hostname, port);

socket.On("MessageToClient", (message) => Console.WriteLine($"From Server: {message};"));

SocketMessage message = new SocketMessage("MessageToServer", "Hello");

while (true)
{
    Console.ReadLine();
    socket.Emit(message);
}