using EventSocket;

const string hostname = "127.0.0.1";
const int port = 8080;

Socket socket = new Socket(SocketType.Client, hostname, port);

socket.Write("Hello from client");
Thread.Sleep(1000);
socket.Write("Hello from client");
Thread.Sleep(1000);
socket.Write("Hello from client");
Thread.Sleep(1000);