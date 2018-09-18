using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;

namespace UDPSoundTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1.UDP Server\n2.UDP Client\n3.TCP Server\n4.TCP Client");
            string choose = Console.ReadLine();

            if (choose.Equals("1"))
                UDPServer();
            else if (choose.Equals("2"))
            {
                UDPClient udpClient = new UDPClient();
            }
            else if (choose.Equals("3"))
                TCPServer();
            else if (choose.Equals("4"))
            {
                TCPClient tcpClient = new TCPClient();
            }
            else
                Console.WriteLine("???????????????\n???????????????");
            Console.ReadLine();
        }
        public static void UDPServer()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 5556);
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            newsock.Bind(ipep);
            Console.WriteLine("localhost:5556    Waiting for a client...");
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);
            while (true)
            {
                try
                {
                    byte[] data = new byte[10025];
                    int recv = newsock.ReceiveFrom(data, ref Remote);
                    string st = Encoding.UTF8.GetString(data, 0, recv);
                    //Console.WriteLine(Remote + " : " + st);
                    newsock.SendTo(data, recv, SocketFlags.None, Remote); //將原資料送回去
                }
                catch (Exception ie) { }
                
            }
        }

        public static void TCPServer()
        {
            TcpListener serverSocket = new TcpListener(5555);
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine("localhost:5555    Waiting for a client...");
            clientSocket = serverSocket.AcceptTcpClient();
            Console.WriteLine("Accept connection from client");
            bool isConnect = true;

            while ((true))
            {
                if (isConnect)
                {
                    try
                    {
                        NetworkStream networkStream = clientSocket.GetStream();
                        byte[] bytesFrom = new byte[10024];
                        networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);

                        //var dt = System.Text.Encoding.UTF8.GetString(bytesFrom);
                        //Console.WriteLine(dt.Substring(0, dt.IndexOf("$")));
                        networkStream.Write(bytesFrom, 0, bytesFrom.Length);
                        networkStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        isConnect = false;
                        serverSocket.Stop();
                        clientSocket.Close();
                    }
                }
                else
                {
                    serverSocket = new TcpListener(5555);
                    clientSocket = default(TcpClient);
                    serverSocket.Start();
                    clientSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Accept connection from client");
                    isConnect = true;
                }
            }
        }
    }
}
