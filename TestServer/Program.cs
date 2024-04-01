using EventSocket;

const string hostname = "127.0.0.1";
const int port = 8080;

Socket socket = new Socket(SocketType.Server, hostname, port);

socket.On("MessageToServer", (message) => Console.WriteLine($"From Client: {message};"));

while (true)
{
    Console.ReadLine();
    socket.Emit("MessageToClient", "Hello");
}