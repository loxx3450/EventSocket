using EventSocket.SocketMessageCore;
using EventSocket.Sockets;
using TestSocketMessages;

const string hostname = "127.0.0.1";
const int port = 8080;

ServerSocket socket = new ServerSocket(hostname, port);

List<Socket> sockets = new List<Socket>();

_ = Task.Run(() => CatchingConnections(socket, sockets));

//Messages to send(Important: other side should support these types fo SocketMessage's)
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

        //Socket's setup
        socket.AddSupportedSocketMessageType<SocketMessageInteger>();
        socket.AddSupportedSocketMessageType<SocketMessageText>();

        socket.On("MessageToServer", (message) => Console.WriteLine($"From Client: {message};"));
        socket.On("IntegerToServer", (integer) => Console.WriteLine($"From Client: {integer};"));

        //Adding Socket to the colelction of Sockets(Network Streams) that are representing server side
        sockets.Add(socket);
    }
}