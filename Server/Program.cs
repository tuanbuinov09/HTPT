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
            /*
             * Tạo server socket, cổng 8888
             */
            TcpListener serverSocket = new TcpListener(8888);
            /*
             * Tương đương với clientSocket = null;
             */
            TcpClient clientSocket = default(TcpClient);
            /*
             * Tạo biến đếm đánh số client
             * Khởi động server và in thông báo
             */
            int counter = 0;
            serverSocket.Start();
            Console.WriteLine(" >> " + "Server Started + " + serverSocket.LocalEndpoint);

            counter = 0;

            while (true)
            {
            /*
             * Tăng counter lên 1
             * Nếu có client kết nối đến, server chấp nhân kết nối và in thông báo
             * Tạo đối tượng client kiểu HandleClient được định nghĩa bên dưới
             * Khởi động client truyển vào clientSocket và thứ tự của client đó
             */
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine(" >> " + "Client No:" + Convert.ToString(counter) + " started!");
                HandleClient client = new HandleClient();
                client.startClient(clientSocket, Convert.ToString(counter));
            }
            /*
             * nếu vòng lặp kết thúc, đóng client, server và thông báo
             */
            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine(" >> " + "exit");
            Console.ReadLine();
        }
    }

    //Lớp này để chia luồng xử lý các client
    public class HandleClient
    {
        TcpClient clientSocket;
        string clNo;
        /*
         * Hàm khởi tạo client, gán client socket trong đối tượng này bằng client socket
         * được server accept và gửi từ main.
         * Tạo một thread thực hiện doChat (hàm nhận yêu cầu từ client, xử lý và trả kết quả cho client.
         */
        public void startClient(TcpClient inClientSocket, string clineNo)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            Thread clientThread = new Thread(doChat);
            clientThread.Start();
        }
        private void doChat()
        {
            /*
             * biến requestCount để đánh số yêu cầu của client
             * tạo mảng byte byteFrom, sendBytes để nhận, gửi dữ liệu từ client,server , kích thước 10025
             * tạo các chuỗi để lấy dữ liệu từ mảng byteFrom
             */
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
                    /*
                     * Tạo network stream để nhận dữ liệu từ client
                     * Dùng phương thức read truyền vào mảng nhận dữ liệu, vị trí bắt đầu nhận và kích thước cần nhận
                     * Chuyển dữ liệu nhận được thành kiểu String và lấy yêu cầu (đường dẫn gửi từ client)
                     * In dòng thông báo dữ liệu nhận từ client
                    */
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    dataFromClient = System.Text.Encoding.UTF8.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine(" >> " + "From client - " + clNo +": "+ dataFromClient);
                    rCount = Convert.ToString(requestCount);

                    /*
                     * Thay thế \ = \\ từ đường dẫn
                     */
                    dataFromClient = dataFromClient.Replace(@"\",@"\\");
                    serverResponse = "Server to client (" + clNo + "); Message no." + rCount;
                    /*
                     * Dùng phương thức System.IO.Directory.GetFiles để lấy thư mục và tệp con của đường dẫn
                     * dùng vòng lặp để nối các thư mục, tệp thành 1 
                     * Encode dữ liệu thành mảng byte
                     */
                    var files = System.IO.Directory.GetFiles(dataFromClient, "*.*", System.IO.SearchOption.AllDirectories);
                    String s = serverResponse + "\n";
                    for(int i=0; i < files.Length; i++)
                    {
                        s = s + files[i] +"\n";
                    }
                    sendBytes = Encoding.UTF8.GetBytes(s);
                    /*
                     * clear dữ liệu trên stream rồi ghi sendBytes lên network stream
                     */
                    networkStream.Flush();
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    Console.WriteLine(" >> " + serverResponse);

                }
                catch (Exception ex)
                {
                    /*Bắt lỗi thực hiện yêu cầu*/
                    Console.WriteLine(" >> " + ex.ToString());
                    sendBytes = Encoding.UTF8.GetBytes("Lỗi thực hiện yêu cầu, lí do:\n1. Đường dẫn không tồn tại.\n2. Không thể tỉm ổ đĩa (vd: F:\\).\n3. Không đặt dấu \\ ở cuối đường dẫn.\n-----------------------------------------------------");
                    try
                    {
                        /*
                         * Bắt lỗi client ngắt kết nối
                         */
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