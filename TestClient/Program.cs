using EventSocket;

const string hostname = "127.0.0.1";
const int port = 8080;

Socket socket = new Socket(SocketType.Client, hostname, port);

socket.On("MessageToClient", (message) => Console.WriteLine($"From Server: {message};"));

socket.Emit("MessageToServer", "Hello");
Thread.Sleep(1000);
socket.Emit("MessageToServer", "My name is Yehor");
Thread.Sleep(1000);
socket.Emit("MessageToServer", "Good Luck!");
Thread.Sleep(1000);