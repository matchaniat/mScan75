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
using System.Threading;

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
            if (ouvrirPort())
            {
                lireVolume();
                lireSquelch();
                lirePriorite();
                controles.IsEnabled = true;
                gpriorite.IsEnabled = true;
            } else
            {
                controles.IsEnabled = false;
                gpriorite.IsEnabled = false;
            }
        }
        private Boolean ouvrirPort()
        {
            Boolean ok = false;
            try
            {
                this.port.Open();
                this.port.WriteLine("MDL");
                Thread.Sleep(100);
                if (port.BytesToRead != 0)
                {
                    if (this.port.ReadLine().Equals("MDL,UBC75XLT"))
                    {
                        etat.Content = "OK";
                        ok = true;
                    }
                } else
                {
                    etat.Content = "X";
                }
            } catch (Exception e)
            {
                etat.Content = "X";
            }
            finally
            {
                port.Close();
            }
            return ok;
        }
        private void lireVolume()
        {
            this.port.Open();
            this.port.WriteLine("VOL");
            String[] vol = this.port.ReadLine().Split(',');
            this.port.Close();
            slidervol.Value = Double.Parse(vol[1]);
            labelvol.Content = vol[1];
        }
        private void changerVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            String vol = slidervol.Value.ToString();
            port.Open();
            port.WriteLine("VOL," + vol);
            labelvol.Content = vol;
            port.ReadLine();
            port.Close();
        }
        private void lireSquelch()
        {
            this.port.Open();
            this.port.WriteLine("SQL");
            String[] sql = this.port.ReadLine().Split(',');
            this.port.Close();
            slidersqu.Value = Double.Parse(sql[1]);
            labelsqu.Content = sql[1];
        }
        private void changerSquelch(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            String sql = slidersqu.Value.ToString();
            port.Open();
            port.WriteLine("SQL," + sql);
            labelsqu.Content = sql;
            port.ReadLine();
            port.Close();
        }
        private void lirePriorite()
        {
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("PRI");
            String[] pri = port.ReadLine().Split(',');
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
            if (pri[1].Equals("0"))
                prioff.IsChecked = true;
            else if (pri[1].Equals("1"))
                prion.IsChecked = true;
            else if (pri[1].Equals("2"))
                prionplus.IsChecked = true;
            else if (pri[1].Equals("3"))
                pridnd.IsChecked = true;
        }
    }
}
