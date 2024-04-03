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
        //Waiting for Socket from other side
        Socket socket = await server.GetSocket();                                           //Possible BLOCKING
        Console.WriteLine("Connected");

        //Socket's setup
        SetupSocket(socket);

        //Adding Socket to the colelction of Sockets(Network Streams) that are representing server side
        sockets.Add(socket);
    }
}


void SetupSocket(Socket socket)
{
    //1. Setting supported SocketMessage's Types for income
    socket.AddSupportedSocketMessageType<SocketMessageInteger>();
    socket.AddSupportedSocketMessageType<SocketMessageText>();

    //2. Setting callbacks
    socket.On("MessageToServer", (message) => Console.WriteLine($"From Client: {message};"));
    socket.On("IntegerToServer", (integer) => Console.WriteLine($"From Client: {integer};"));

    //3. Setting callbacks to events
    socket.OnOtherSideIsDisconnected += (socket) =>
    {
        sockets.Remove(socket);
    };
}