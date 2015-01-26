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

namespace Serwer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Label Label;
        public static TextBox TxtBox;

        Server server;

        public MainWindow()
        {
            InitializeComponent();
            MainWindow.Label = mainLabel;
            MainWindow.TxtBox = textBox;
            Canvas.SetZIndex(textBox, 10);

            server = new Server();
        }

        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                server.Send();
            }
        }

        private void Grid_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            MainWindow.TxtBox.Focus();
        }
    }
}
