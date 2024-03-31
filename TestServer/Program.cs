using EventSocket;

const string hostname = "127.0.0.1";
const int port = 8080;

Socket socket = new Socket(SocketType.Server, hostname, port);

socket.On("MessageToServer", (message) => Console.WriteLine($"From Client: {message};"));

socket.Emit("MessageToClient", "Hello");
Thread.Sleep(1000);
socket.Emit("MessageToClient", "My name is Yehor");
Thread.Sleep(1000);
socket.Emit("MessageToClient", "Good Luck!");
Thread.Sleep(1000);