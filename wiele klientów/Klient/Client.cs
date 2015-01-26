using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Klient
{
    class Client
    {
        private Socket socket;
        private byte[] buffer;

        private static object mainLabelLock = new Object();
        private static object textBoxLock = new Object();

        //public delegate void ClientDelegate();

        public Client()
        {
            /*Client.mainLabel = label;
            Client.textBox = txtBox;*/

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            buffer = new byte[1024];
            ClearBuffer();
            
            socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000));

            Thread receivingThread = new Thread(Receive);
            receivingThread.Start();
        }

        public void Receive()
        {
            string msg;
            string msg2 = "";
            while (true)
            {
                socket.Receive(buffer);
                msg = ASCIIEncoding.ASCII.GetString(buffer);
                for (int i = 0; i < msg.Length; i++)
                {
                    if (msg[i] != 0)
                    {
                        //lock (mainLabelLock)
                        //{
                            msg2 += msg[i].ToString();
                        //}
                    }
                    else break;
                }
                
                Application.Current.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(() => MainWindow.Label.Content += msg2) //bez Action?
                );

                ClearBuffer();
            }
        }

        public void Send()
        {
            string msg;
            lock (textBoxLock)
            {
                msg = "\nK: " + MainWindow.TxtBox.Text;
                MainWindow.TxtBox.Text = "";
            }
            lock (mainLabelLock)
            {
                MainWindow.Label.Content += msg;
            }
            socket.Send(ASCIIEncoding.ASCII.GetBytes(msg));
        }

        private void ClearBuffer()
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = 0;
        }
    }
}
