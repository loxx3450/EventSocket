using SocketEventLibrary.SocketEventMessageCore;
using SocketEventLibrary.Sockets;
using TestSocketEventMessages;

const string hostname = "127.0.0.1";
const int port = 8080;

ClientSocketEvent socket = new ClientSocketEvent(hostname, port);

//Waiting for SocketEvent from other side
SocketEvent client = await socket.GetSocketAsync();                                                           //Possible BLOCKING

//Socket's setup
SetupSocket(client);

//Messages to send(Important: other side should support these types fo SocketEventMessage's)
SocketEventMessageText messageToServerText = new SocketEventMessageText("MessageToServer", "Hello");
SocketEventMessageText messageToClientText = new SocketEventMessageText("MessageToOtherClient", "Hi, other client");
SocketEventMessageInteger messageInteger = new SocketEventMessageInteger("IntegerToServer", 1234567890);

while (true)
{
    switch (Console.ReadLine())
    {
        case "1":
            client.Emit(messageInteger);
            break;
        case "2":
            client.Emit(messageToClientText);
            break;
        default:
            client.Emit(messageToServerText);
            break;
    }
}



void SetupSocket(SocketEvent socket)
{
    //1. Setting supported SocketEventMessages's Types for income
    socket.AddSupportedMessageType<SocketEventMessageText>();

    //2. Setting callbacks
    socket.On("MessageToClientFromServer", (message) => Console.WriteLine($"From Server: {message};"));
    socket.On("MessageToClientFromClient", (message) => Console.WriteLine($"From Other Client: {message};"));

    //3. Setting callbacks to events
    socket.OnOtherSideIsDisconnected += (socket) =>
    {
        Environment.Exit(0);
    };

    socket.OnThrowedException += (ex) =>
        Console.WriteLine(ex.Message);
}