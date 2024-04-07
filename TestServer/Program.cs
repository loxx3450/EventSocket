using SocketEventLibrary.SocketEventMessageCore;
using SocketEventLibrary.Sockets;
using TestSocketEventMessages;

const string hostname = "127.0.0.1";
const int port = 8080;

ServerSocketEvent socket = new ServerSocketEvent(hostname, port);

List<SocketEvent> sockets = new List<SocketEvent>();

socket.OnClientIsConnected += SetupSocket;


//Clients will be connecting with us in other Thread and make logic of event OnClientIsConnected after success Connection
socket.StartAcceptingClients();


//Messages to send(Important: other side should support these types fo SocketEventMessage's)
SocketEventMessageText messageFromClient = new SocketEventMessageText("MessageToClientFromClient", "Hello");
SocketEventMessageText messageFromServer = new SocketEventMessageText("MessageToClientFromServer", "Hello");

while (true)
{
    switch (Console.ReadLine())
    {
        case "end":
            socket.StopAcceptingClients();
            break;
        default:
            foreach (var s in sockets)
            {
                s.Emit(messageFromServer);
            }
            break;
    }
}


void SetupSocket(SocketEvent socket)
{
    //1. Setting supported SocketMessage's Types for income
    //socket.AddSupportedMessageType<SocketEventMessageInteger>();
    socket.AddSupportedMessageType<SocketEventMessageText>();

    //2. Setting callbacks
    socket.On("MessageToServer", (message) => Console.WriteLine($"From Client: {message};"));
    socket.On("IntegerToServer", (integer) => Console.WriteLine($"From Client: {integer};"));

    socket.On("MessageToOtherClient", (message) =>
    {
        foreach (var s in sockets)
        {
            //finding exact one(client, room...)

            SocketEventMessageText messageFromClient = new SocketEventMessageText("MessageToClientFromClient", Convert.ToString(message));

            s.Emit(messageFromClient);
        }
    });

    //3. Setting callbacks to events
    socket.OnOtherSideIsDisconnected += (socket) =>
    {
        sockets.Remove(socket);
    };

    //Adding SocketEvent to the colelction of Sockets(Network Streams) that are representing server side
    sockets.Add(socket);
}