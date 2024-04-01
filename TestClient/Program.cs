using EventSocket;

const string hostname = "127.0.0.1";
const int port = 8080;

Socket socket = new Socket(SocketType.Client, hostname, port);

socket.On("MessageToClient", (message) => Console.WriteLine($"From Server: {message};"));

while (true)
{
    Console.ReadLine();
    socket.Emit("MessageToServer", "Hello");
}