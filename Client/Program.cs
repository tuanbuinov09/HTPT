using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static Form1 formMain;
        public static Socket ClientSocket;
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Program.formMain = new Form1();
            Application.Run(formMain);
        }
        /*public static string send1(string msg)
        {
            while (true)
            {
                string msgFromClient = null;
                *//*msgFromClient = msg;*//*
                byte[] data = new byte[1024];
                data = Encoding.ASCII.GetBytes("12312");
                MessageBox.Show(data.ToString());
                int port = 13000;
                string IpAddress = "127.0.0.1";
                Program.ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IpAddress), port);
                Program.ClientSocket.Connect(ep);
                MessageBox.Show("Client is connected.. ");
                *//*Program.ClientSocket.Send(System.Text.Encoding.ASCII.GetBytes(msgFromClient), 0, msgFromClient.Length, SocketFlags.None);*/
        /*Program.ClientSocket.Send(data);
                byte[] msgFromServer = new byte[1024];
        int size = Program.ClientSocket.Receive(msgFromServer);
        MessageBox.Show("Server: " + System.Text.Encoding.ASCII.GetString(msgFromServer, 0, size));
                return System.Text.Encoding.ASCII.GetString(msgFromServer, 0, size);
            }
            return "";
        }*/
    }
}
