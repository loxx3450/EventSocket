using EventSocket.SocketMessages;
using EventSocket.Sockets;

const string hostname = "127.0.0.1";
const int port = 8080;

ServerSocket socket = new ServerSocket(hostname, port);

List<Socket> sockets = new List<Socket>();

_ = Task.Run(() => CatchingConnections(socket, sockets));

SocketMessageText message = new SocketMessageText("MessageToClient", "Hello");

while (true)
{
    Console.ReadLine();

    foreach (var s in sockets)
    {
        s.Emit(message);
    }
}

async Task CatchingConnections(ServerSocket server, List<Socket> sockets)
{
    while(true)
    {
        Socket socket = await server.GetSocket();
        Console.WriteLine("Connected");

        socket.On("MessageToServer", (message) => Console.WriteLine($"From Client: {message};"));                           //TODO: setting dictionary

        sockets.Add(socket);
    }
}