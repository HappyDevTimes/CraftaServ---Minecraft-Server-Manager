using PortforwardWPF.Classes;
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
using System.Windows.Shapes;
using static PortforwardWPF.MainWindow;

namespace PortforwardWPF.Windows
{
    /// <summary>
    /// Interaction logic for TeleportWindow.xaml
    /// </summary>
    public partial class TeleportWindow : Window
    {
        MainWindow mw;
        public TeleportWindow()
        {
            InitializeComponent();
            mw = MainWindow.instance;
        }

        private void btnTeleport_Click(object sender, RoutedEventArgs e)
        {
            Commands cmd = new Commands();
            cmd.TeleportUser(tbUsername.Text);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
