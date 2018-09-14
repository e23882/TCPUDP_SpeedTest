using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace UDPSoundTest
{
    class UDPClient
    {
        static string ip = "127.0.0.1";
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), 5556);
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        byte[] data = new byte[4096]; //存放接收的資料
        public UDPClient()
        {
            ip = "127.0.0.1";
            ipep = new IPEndPoint(IPAddress.Parse(ip), 5556);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //Thread thSendPing = new Thread(sendPing);
            //thSendPing.Start();
            //Thread thReceived = new Thread(received);
            //thReceived.Start();
            //test
            Thread thSendPing = new Thread(SendPacket);
            thSendPing.Start();
            Thread thReceived = new Thread(TestReceived);
            thReceived.Start();

            while (true)
            {
                Console.WriteLine("input message : ");
                string input = Console.ReadLine();
                client.SendTo(Encoding.UTF8.GetBytes(input), ipep);
            }
        }
        public void sendMessage(string message)
        {
            client.SendTo(Encoding.UTF8.GetBytes(message), ipep);
        }
        /// <summary>
        /// 測試UDP Send packet
        /// </summary>
        int sendCounter = 0;
        public void SendPacket()
        {
            
            byte[] data = new byte[1024];
            
            data = Encoding.UTF8.GetBytes(Stopwatch.GetTimestamp().ToString());
            while (true)
            {
                client.SendTo(data, ipep);
                sendCounter++;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void sendPing()
        {

            byte[] data = new byte[1024];
            
            data = Encoding.UTF8.GetBytes(Stopwatch.GetTimestamp().ToString());
            while (true)
            {
                client.SendTo(Encoding.UTF8.GetBytes("ping"), ipep);
                Thread.Sleep(5000);
            }
        }
        /// <summary>
        /// 測試UDP received packet
        /// </summary>
        int receiveCounter = 0;
        double time = 0;
        int testCounter = 0;
        public void TestReceived()
        {
            while (true)
            {
                byte[] data = new byte[1024];                
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint Remote = (EndPoint)sender;
                int recv = client.ReceiveFrom(data, ref Remote);
                long currTS = Stopwatch.GetTimestamp();
                string receiveData = Encoding.UTF8.GetString(data, 0, recv);
                long sendTS = long.Parse(receiveData);
                time += (double)(currTS - sendTS) / (double)Stopwatch.Frequency;
                receiveCounter++;
                if (sendCounter >=1024)
                {
                    Console.WriteLine("loss : " + receiveCounter/sendCounter);
                    Console.WriteLine("speed : " + sendCounter /time);

                    time = 0;
                    sendCounter = 0;
                    receiveCounter = 0;
                    testCounter++;
                }
                if (testCounter == 1000)
                    Console.ReadLine();
            }
        }
        public void received()
        {
            while (true)
            {
                byte[] data = new byte[4096];
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint Remote = (EndPoint)sender;
                int recv = client.ReceiveFrom(data, ref Remote);
                Console.WriteLine("server " + Remote.ToString() + " : " + Encoding.UTF8.GetString(data, 0, recv));
            }

        }
    }
}
