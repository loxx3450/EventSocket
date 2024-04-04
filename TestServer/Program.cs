using EventSocket.SocketEventMessageCore;
using EventSocket.Sockets;
using TestSocketEventMessages;

const string hostname = "127.0.0.1";
const int port = 8080;

ServerSocketEvent socket = new ServerSocketEvent(hostname, port);

List<SocketEvent> sockets = new List<SocketEvent>();

_ = Task.Run(() => CatchingConnections(socket, sockets));


//Messages to send(Important: other side should support these types fo SocketEventMessage's)
SocketEventMessageText message = new SocketEventMessageText("MessageToClient", "Hello");

while (true)
{
    Console.ReadLine();

    foreach (var s in sockets)
    {
        s.Emit(message);
    }
}



async Task CatchingConnections(ServerSocketEvent server, List<SocketEvent> sockets)
{
    while(true)
    {
        //Waiting for SocketEvent from other side
        SocketEvent socket = await server.GetSocket();                                           //Possible BLOCKING
        Console.WriteLine("Connected");

        //Socket's setup
        SetupSocket(socket);

        //Adding SocketEvent to the colelction of Sockets(Network Streams) that are representing server side
        sockets.Add(socket);
    }
}


void SetupSocket(SocketEvent socket)
{
    //1. Setting supported SocketMessage's Types for income
    socket.AddSupportedMessageType<SocketEventMessageInteger>();
    socket.AddSupportedMessageType<SocketEventMessageText>();

    //2. Setting callbacks
    socket.On("MessageToServer", (message) => Console.WriteLine($"From Client: {message};"));
    socket.On("IntegerToServer", (integer) => Console.WriteLine($"From Client: {integer};"));

    //3. Setting callbacks to events
    socket.OnOtherSideIsDisconnected += (socket) =>
    {
        sockets.Remove(socket);
    };
}