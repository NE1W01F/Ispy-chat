using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ISpy_Server
{
    class Program
    {
        public static TcpListener listener = new TcpListener(IPAddress.Any, 2355);
        public static List<TcpClient> clients = new List<TcpClient>();
        public static List<Thread> threads = new List<Thread>();
        static void Main(string[] args)
        {
            listener.Start();
            Console.WriteLine("Server Running on Port 2355");
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Thread thread = new Thread(() => Client_Thread(client));
                clients.Add(client);
                threads.Add(thread);
                Console.WriteLine("Client Connected");
                thread.Start();
            }
        }

        public static void Client_Thread(TcpClient client)
        {
            string username;
            bool exit = true;
            while (exit)
            {
                try
                {
                    Stream stream = client.GetStream();
                    StreamReader sr = new StreamReader(stream);
                    string messsage = sr.ReadLine();
                    username = messsage.Split(':')[0];
                    if (!String.IsNullOrEmpty(messsage))
                    {
                        Console.WriteLine($"MSG: {messsage}");
                        Brodcast_Message(client, messsage);
                    }
                }
                catch (Exception) { exit = false; }
            }
        }

        public static void Brodcast_Message(TcpClient tcp, string message)
        {
            for(int i = 0; i < clients.Count; i++)
            {
                if(tcp != clients[i])
                {
                    if (clients[i].Connected)
                    {
                        StreamWriter stream = new StreamWriter(clients[i].GetStream());
                        stream.WriteLine(message);
                        stream.Flush();
                    }
                    else
                    {
                        clients.RemoveAt(i);
                        threads[i].Abort();
                        threads.RemoveAt(i);
                        Console.WriteLine("Client Disconnected");
                    }
                }
            }
        }
    }
}
