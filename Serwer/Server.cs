﻿using System;
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

namespace Serwer
{
    class Server
    {
        private Socket socket;
        private Socket clientSocket = null;
        private byte[] buffer;

        private static object mainLabelLock = new Object();
        private static object textBoxLock = new Object();

        private Thread acceptingThread;
        private List<Thread> receivingThreads;

        private List<Socket> clientSockets;

        public Server()
        {
            /*Server.mainLabel = mainLabel;
            Server.textBox = textBox;*/
            //Dispatcher.Invoke

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000));
            socket.Listen(5);

            buffer = new byte[1024];
            ClearBuffer();
            
            acceptingThread = new Thread(Accept);
            //Thread receivingThread = new Thread(Receive);
            receivingThreads = new List<Thread>();

            clientSockets = new List<Socket>();

            acceptingThread.Start();
            //receivingThread.Start();
        }

        ~Server()
        {
            acceptingThread.Abort();
            foreach (Thread t in receivingThreads)            
                t.Abort();            
            foreach (Socket s in clientSockets)            
                s.Close();
            socket.Close();
        }

        public void Accept()
        {
            //clientSocket = socket.Accept();
            while (true)
            {
                Socket newClient = socket.Accept();
                clientSockets.Add(newClient);
                receivingThreads.Add(new Thread(Receive));
                receivingThreads[receivingThreads.Count - 1].Start(newClient);
            }
        }

        public void Receive(object o)
        {        
            Socket clientSocket = (Socket)o;
            //string msg = "", msg2 = "";
            while (true)
            {
                string msg = "", msg2 = "";
                //msg = msg2 = "";
                clientSocket.Receive(buffer);
                //msg = ASCIIEncoding.ASCII.GetString(buffer);
                msg = UTF8Encoding.UTF8.GetString(buffer);
                for (int i = 0; i < msg.Length; i++)
                {
                    if (msg[i] != 0)
                    {
                        // (mainLabelLock)
                        //{
                        //MainWindow.Label.Content += msg[i].ToString();
                        //}
                        msg2 += msg[i];
                    }
                    else break;
                }

                Application.Current.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(() => MainWindow.Label.Content += msg2)
                    );

                foreach (Socket s in clientSockets)
                    if (s != clientSocket)
                    {
                        //s.Send(ASCIIEncoding.ASCII.GetBytes(msg2));
                        s.Send(UTF8Encoding.UTF8.GetBytes(msg2));
                    }

                //msg = ""; msg2 = "";
                ClearBuffer();
            }
        }

        /*public void Receive()
        {
            string msg;
            string msg2 = "";
            while (true)
            {
                if (clientSocket == null)
                    continue;
                clientSocket.Receive(buffer);
                msg = ASCIIEncoding.ASCII.GetString(buffer);
                /*lock (mainLabelLock)
                {
                    MainWindow.Label.Content += "\n";
                }*/
                
           /*     for (int i = 0; i < msg.Length; i++)
                {
                    if (msg[i] != 0)
                    {
                        // (mainLabelLock)
                        //{
                            //MainWindow.Label.Content += msg[i].ToString();
                        //}
                        msg2 += msg[i];
                    }
                    else break;
                }

                Application.Current.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(() => MainWindow.Label.Content += msg2)
                );

                ClearBuffer();
            }
        }*/

        public void Send()
        {
            /*string msg;
            lock (textBoxLock)
            {
                msg = "\nS: " + MainWindow.TxtBox.Text;
                MainWindow.TxtBox.Text = "";
            }
            lock (mainLabelLock)
            {
                MainWindow.Label.Content += msg;
            }
            clientSocket.Send(ASCIIEncoding.ASCII.GetBytes(msg));*/
            string msg = "\nS: " + MainWindow.TxtBox.Text;
            MainWindow.TxtBox.Text = "";
            MainWindow.Label.Content += msg;
            foreach (Socket s in clientSockets)
                //s.Send(ASCIIEncoding.ASCII.GetBytes(msg));
                s.Send(UTF8Encoding.UTF8.GetBytes(msg));
        }

        private void ClearBuffer()
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = 0;
        }
    }
}
