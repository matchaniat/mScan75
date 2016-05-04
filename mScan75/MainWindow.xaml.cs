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
using System.IO.Ports;

namespace mScan75
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort port;
        public MainWindow()
        {
            InitializeComponent();
            this.port = new SerialPort("com0", 57600, (Parity)Enum.Parse(typeof(Parity), "0", true), 8, (StopBits)Enum.Parse(typeof(StopBits), "1", true));
            port.NewLine = "\r";
            List<string> ports = new List<string>();
            foreach (string s in SerialPort.GetPortNames())
            {
                ports.Add(s);
            }
            portselec.ItemsSource =ports;
        }

        private void changerPort(object sender, SelectionChangedEventArgs e)
        {
            this.port.PortName = ((String)portselec.SelectedValue).ToLower();
        }
    }
}
