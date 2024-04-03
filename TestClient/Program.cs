using EventSocket.SocketMessageCore;
using EventSocket.Sockets;
using TestSocketMessages;

const string hostname = "127.0.0.1";
const int port = 8080;

ClientSocket socket = new ClientSocket(hostname, port);

//Waiting for Socket from other side
Socket client = await socket.GetSocket();                                                           //Possible BLOCKING

//Socket's setup
SetupSocket(client);

//Messages to send(Important: other side should support this types fo SocketMessage's)
SocketMessageText messageText = new SocketMessageText("MessageToServer", "Hello");
SocketMessageInteger messageInteger = new SocketMessageInteger("IntegerToServer", 1234567890);

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



void SetupSocket(Socket socket)
{
    //1. Setting supported SocketMessage's Types for income
    socket.AddSupportedSocketMessageType<SocketMessageText>();

    //2. Setting callbacks
    socket.On("MessageToClient", (message) => Console.WriteLine($"From Server: {message};"));

    //3. Setting callbacks to events
    socket.OnOtherSideIsDisconnected += (socket) =>
    {
        Environment.Exit(0);
    };
}