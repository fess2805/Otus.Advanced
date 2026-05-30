using Practice_2;

Console.WriteLine("Hello, World!");


var tcpServer = new MyTcpServer();
await tcpServer.StartAsync();
