using EventSocket.SocketEventMessageCore;
using EventSocket.Sockets;
using TestSocketEventMessages;

const string hostname = "127.0.0.1";
const int port = 8080;

ClientSocketEvent socket = new ClientSocketEvent(hostname, port);

//Waiting for SocketEvent from other side
SocketEvent client = await socket.GetSocket();                                                           //Possible BLOCKING

//Socket's setup
SetupSocket(client);

//Messages to send(Important: other side should support this types fo SocketEventMessage's)
SocketEventMessageText messageText = new SocketEventMessageText("MessageToServer", "Hello");
SocketEventMessageInteger messageInteger = new SocketEventMessageInteger("IntegerToServer", 1234567890);

while (true)
{
    switch (Console.ReadLine())
    {
        case "1":
            client.Emit(messageInteger);
            break;
        default:
            client.Emit(messageText);
            break;
    }
}



void SetupSocket(SocketEvent socket)
{
    //1. Setting supported SocketEventMessages's Types for income
    socket.AddSupportedMessageType<SocketEventMessageText>();

    //2. Setting callbacks
    socket.On("MessageToClient", (message) => Console.WriteLine($"From Server: {message};"));

    //3. Setting callbacks to events
    socket.OnOtherSideIsDisconnected += (socket) =>
    {
        Environment.Exit(0);
    };
}