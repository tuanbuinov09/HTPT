using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        /*
         * Tạo socket, network stream để nhận gửi dữ liệu
         */
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        /*
         * Event khi ấn nút gửi yêu cầu
         * lấy đường dẫn nhập từ textBox
         * tạo mảng byte outstream và chuyển đường dẫn đc nhập vào sang dạng mảng byte
         * ghi outstream ra serverStream rồi clear dữ liệu trên stream đó
         */
            string pathToSend = textBoxPath.Text;
            NetworkStream serverStream = clientSocket.GetStream();
            byte[] outStream = System.Text.Encoding.UTF8.GetBytes(pathToSend + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            /*
             * tạo mảng nhận dữ liệu từ server
             * đọc dữ liệu từ server stream, chuyển sang dạng String và dùng hàm msg để hiển thị lên richtextbox
             */

            byte[] inStream = new byte[10025];
            serverStream.Read(inStream, 0, inStream.Length);
            string returndata = System.Text.Encoding.UTF8.GetString(inStream);
   
            msg("Data from Server : \n" + returndata);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.DetectUrls = false;
            richTextBox1.Text = "Client Started\n-----------------------------------------------------";
            clientSocket.Connect("127.0.0.1", 8888);
            label1.Text = "Client Socket Program - Server Connected ...";
        }
        public void msg(string mesg)
        {
            richTextBox1.Text = "";
            richTextBox1.Text = richTextBox1.Text  + " >> " + mesg;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
