using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Net;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(8888);

            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            /*IPHostEntry iphostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = iphostInfo.AddressList[0];
            IPEndPoint localEndpoint = new IPEndPoint(ipAddress, 8888);
            Console.WriteLine(localEndpoint);*/

            serverSocket.Start();
            Console.WriteLine(" >> " + "Server Started + " + serverSocket.LocalEndpoint);

            counter = 0;
            while (true)
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine(" >> " + "Client No:" + Convert.ToString(counter) + " started!");
                handleClient client = new handleClient();
                client.startClient(clientSocket, Convert.ToString(counter));
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine(" >> " + "exit");
            Console.ReadLine();
        }
    }

    //Class to handle each client request separatly
    public class handleClient
    {
        TcpClient clientSocket;
        string clNo;
        public void startClient(TcpClient inClientSocket, string clineNo)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }
        private void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    /*networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);*/
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    dataFromClient = System.Text.Encoding.UTF8.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine(" >> " + "From client - " + clNo +": "+ dataFromClient);
                    rCount = Convert.ToString(requestCount);

                    dataFromClient = dataFromClient.Replace(@"\",@"\\");
                    serverResponse = "Server to client (" + clNo + "); Message no." + rCount;
                    
                    var files = System.IO.Directory.GetFiles(dataFromClient, "*.*", System.IO.SearchOption.AllDirectories);
                    String s = serverResponse + "\n";
                    for(int i=0; i < files.Length; i++)
                    {
                        s = s + files[i] +"\n";
                    }
                    sendBytes = Encoding.UTF8.GetBytes(s);
                    networkStream.Flush();
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    Console.WriteLine(" >> " + serverResponse);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> " + ex.ToString());
                    sendBytes = Encoding.UTF8.GetBytes("Lỗi thực hiện yêu cầu, lí do:\n1. Đường dẫn không tồn tại.\n2. Không thể tỉm ổ đĩa (vd: F:\\).\n3. Không đặt dấu \\ ở cuối đường dẫn.\n-----------------------------------------------------");
                    try
                    {
                        NetworkStream networkStream = clientSocket.GetStream();
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                    }catch(Exception ex2)
                    {
                        Console.WriteLine(">> Client socket closed");
                        return;
                    }
                }
            }
        }
    }
}