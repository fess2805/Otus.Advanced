using Practice_1;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Practice_2
{
    /*
     * Пункт 1. Создание класса TCP-сервера
Создайте класс TcpServer. В нем реализуйте метод StartAsync, который будет инициализировать Socket, 
    связывать его с локальным IP-адресом и портом (например, 127.0.0.1:8080) и переводить в режим прослушивания (Listen).
Пункт 2. Реализация цикла приема подключений
В методе StartAsync после вызова Listen организуйте бесконечный асинхронный цикл (while(true)), 
    который ожидает новые подключения с помощью await serverSocket.AcceptAsync(). 
    Для каждого принятого клиентского сокета запускайте отдельную задачу для его обработки 
    (например, _ = ProcessClientAsync(clientSocket)).
Пункт 3. Чтение данных от клиента и парсинг
Реализуйте приватный асинхронный метод ProcessClientAsync(Socket clientSocket). Внутри него организуйте цикл 
    для чтения данных. При чтении (await clientSocket.ReceiveAsync(...)) используйте буфер, 
    арендованный из ArrayPool.Shared. Полученные данные (в виде ReadOnlyMemory) передавайте 
    в статический метод CommandParser.Parse из ДЗ №1. Результат парсинга (команду, ключ, значение) выводите в консоль. 
    Не забудьте возвращать буфер в пул после использования.
Пункт 4. Обработка отключения клиента
Модифицируйте цикл чтения данных в ProcessClientAsync. 
    Если вызов ReceiveAsync возвращает 0, это означает, что клиент закрыл соединение. 
    В этом случае необходимо прервать цикл, корректно закрыть сокет клиента (Shutdown, Close, Dispose) и завершить задачу обработки.
Пункт 5. Запуск сервера
В Program.cs создайте экземпляр вашего TcpServer, вызовите его метод StartAsync и обеспечьте работу 
    приложения в фоновом режиме, чтобы оно не завершилось сразу после запуска (например, с помощью Console.ReadLine()).
     */


    public class MyTcpServer
    {
        private Socket _serverSocket;
        private bool _isRunning = false;
        public async Task StartAsync(string ipAddress = "127.0.0.1", int port = 8080)
        {
            _isRunning = true;

            // Инициализация сокета
            _serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            // Привязка к локальному адресу и порту
            var localEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            _serverSocket.Bind(localEndPoint);

            // Режим прослушивания (максимум 100 подключений в очереди)
            _serverSocket.Listen(100);
            Console.WriteLine($"Сервер запущен на {ipAddress}:{port}");

            await AcceptConnectionsAsync();
        }

        private async Task AcceptConnectionsAsync()
        {
            while (_isRunning)
            {
                try
                {                    
                    var clientSocket = await _serverSocket!.AcceptAsync();
                    Console.WriteLine("Новое подключение получено");
                   
                    _ = ProcessClientAsync(clientSocket);
                }
                catch (SocketException se) when (!_isRunning)
                {
                    Console.WriteLine("Ошибка сокета: ", se.Message);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при приёме подключения: {ex.Message}");
                }
            }
        }

        private async Task ProcessClientAsync(Socket clientSocket)
        {
            const int BufferSize = 1024;

            try
            {
                while (true)
                {                    
                    var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);

                    try
                    {                        
                        var received = await clientSocket.ReceiveAsync(
                            new ArraySegment<byte>(buffer),
                            SocketFlags.None
                        );

                        
                        if (received == 0)
                            break;

                        
                        var data = new ReadOnlyMemory<byte>(buffer, 0, received);
                        
                        var result = CommandParser.Parse(data.Span);

                        Console.WriteLine($"Получено: Команда={result.Command}, Ключ={result.Key}, Значение={result.Value}");
                    }
                    finally
                    {
                        // Обязательно возвращаем буфер в пул
                        ArrayPool<byte>.Shared.Return(buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке клиента: {ex.Message}");
            }
            finally
            {
                // Корректное закрытие сокета клиента
                ShutdownAndCloseSocket(clientSocket);
            }
        }
        
        private void ShutdownAndCloseSocket(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException se)
            {
                Console.WriteLine("Ошибка закрытия сокета: ", se.Message);
            }

            socket.Close();
            socket.Dispose();
        }
    }
}
