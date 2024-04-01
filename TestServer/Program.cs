using EventSocket;

const string hostname = "127.0.0.1";
const int port = 8080;

Socket<string, string> socket = new Socket<string, string>(SocketType.Server, hostname, port);

socket.On("MessageToServer", (message) => Console.WriteLine($"From Client: {message};"));

SocketMessage message = new SocketMessage("MessageToClient", "Hello");

while (true)
{
    Console.ReadLine();
    socket.Emit(message);
}