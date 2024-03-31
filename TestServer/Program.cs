using EventSocket;

const string hostname = "127.0.0.1";
const int port = 8080;

Socket socket = new Socket(SocketType.Server, hostname, port);

socket.Write("Hello from Server");
Thread.Sleep(1000);
socket.Write("Hello from Server");
Thread.Sleep(1000);
socket.Write("Hello from Server");
Thread.Sleep(1000);