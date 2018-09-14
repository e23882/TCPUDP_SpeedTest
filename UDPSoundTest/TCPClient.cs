using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UDPSoundTest
{
    class TCPClient
    {
        string ip = string.Empty;
        TcpClient client ;
        IPEndPoint ipendpoint;
        NetworkStream stream;
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        public TCPClient()
        {
            client = new TcpClient(ip, 5555);
            ipendpoint = client.Client.RemoteEndPoint as IPEndPoint;
            stream = client.GetStream();
            Thread thSendPing = new Thread(sendTest);
            thSendPing.Start();
            Thread thReceived = new Thread(receivedTest);
            thReceived.Start();
            try
            {
                while (true)
                {
                    //byte[] messages = Encoding.Default.GetBytes("hello");
                    //stream.Write(messages, 0, messages.Length);
                }
            }
            catch (Exception ie)
            {
                stream.Close();
                client.Close();
            }
        }

        public void sendTest()
        {
            while (true)
            {
                byte[] data = new byte[1024];
                data = Encoding.UTF8.GetBytes("|"+Stopwatch.GetTimestamp().ToString() + "$");
                stream.Write(data, 0, data.Length);
            }
        }
        double time=0;
        int receiveCounter = 0;
        int sendCounter = 0;
        int testCounter = 0;
        public void receivedTest()
        {
            while (true)
            {
                long currTS = Stopwatch.GetTimestamp();
                byte[] bytes = new Byte[1024];
                string data = string.Empty;
                int length = stream.Read(bytes, 0, bytes.Length);
                data = Encoding.Default.GetString(bytes, 0, length);
                if (data.IndexOf("$") != -1 && data.IndexOf("|")!=-1)
                {
                    long sendTS = long.Parse(data.Substring(data.IndexOf("|")+1, data.IndexOf("$")- data.IndexOf("|")-1));
                    time += (double)(currTS - sendTS) / (double)Stopwatch.Frequency;
                    receiveCounter++;
                    if (sendCounter >= 1024)
                    {
                        Console.WriteLine("loss : " + receiveCounter / sendCounter);
                        Console.WriteLine("speed : " + sendCounter / time);

                        time = 0;
                        sendCounter = 0;
                        receiveCounter = 0;
                        testCounter++;
                    }
                    if (testCounter == 1000)
                        Console.ReadLine();
                }
            }
        }
    }
}
