using System;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Сервер
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Устанавливаем для сокета локальную конечную точку
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

            // Создаем сокет Tcp/Ip
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                // Начинаем слушать соединения
                while (true)
                {
                    Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint);

                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    string data = null;

                    // Мы дождались клиента, пытающегося с нами соединиться

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    // Показываем данные на консоли
                    Console.Write("Полученный текст: " + data + "\n\n");
                    string reply = "";
                    int number = int.Parse(data.ToString());
                        
                        for (int i = 0; i < number; i++)
                        {
                            System.Threading.Thread.Sleep(50);
                        // Отправляем ответ клиенту\
                        var generate = RandomizerFactory.GetRandomizer(new FieldOptionsTextWords() { Min = number, Max = number });
                        string word = generate.Generate();
                        reply = "Спасибо, Вы ввели " + data.ToString()
                                  + "  " + word;
                            byte[] msg = Encoding.UTF8.GetBytes(reply);
                        handler.Send(msg);
                        }
                
                    if (data.IndexOf("<TheEnd>") > -1)
                    {
                        Console.WriteLine("Сервер завершил соединение с клиентом.");
                        break;
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
    
}
