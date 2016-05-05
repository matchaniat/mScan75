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
                lireCloseCall();
                controles.IsEnabled = true;
                gpriorite.IsEnabled = true;
                gclosecall.IsEnabled = true;
            } else
            {
                controles.IsEnabled = false;
                gpriorite.IsEnabled = false;
                gclosecall.IsEnabled = false;
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
            else if (pri[1].Equals("3"))
                pridnd.IsChecked = true;
        }

        private void changerPrioriteOff(object sender, RoutedEventArgs e)
        {
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("PRI,0");
            String[] pri = port.ReadLine().Split(',');
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void changerPrioriteOn(object sender, RoutedEventArgs e)
        {
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("PRI,1");
            String[] pri = port.ReadLine().Split(',');
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void changerPrioriteDND(object sender, RoutedEventArgs e)
        {
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("PRI,3");
            String[] pri = port.ReadLine().Split(',');
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void lireCloseCall()
        {
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("CLC");
            String[] clc = port.ReadLine().Split(',');
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
            if (clc[1].Equals("0"))
                ccoff.IsChecked = true;
            else if (clc[1].Equals("1"))
                ccon.IsChecked = true;
            else if (clc[1].Equals("2"))
                ccdnd.IsChecked = true;
            if (clc[2].Equals("0"))
                checkBoxalbip.IsChecked = false;
            else
                checkBoxalbip.IsChecked = true;
            if (clc[3].Equals("0"))
                checkBoxallum.IsChecked = false;
            else
                checkBoxallum.IsChecked = true;
            char[] bandes = clc[4].ToCharArray();
            if (bandes[0].Equals('0'))
                ccvhfb.IsChecked = false;
            else
                ccvhfb.IsChecked = true;
            if (bandes[1].Equals('0'))
                ccair.IsChecked = false;
            else
                ccair.IsChecked = true;
            if (bandes[2].Equals('0'))
                ccvhfh.IsChecked = false;
            else
                ccvhfh.IsChecked = true;
            if (bandes[4].Equals('0'))
                ccuhf.IsChecked = false;
            else
                ccuhf.IsChecked = true;
        }

        private void ChangerCloseCallOff(object sender, RoutedEventArgs e)
        {
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("CLC");
            String[] clc = port.ReadLine().Split(',');
            port.WriteLine("CLC,0," + clc[2] + "," + clc[3] + "," + clc[4] + ",");
            port.ReadLine();
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void ChangerCloseCallOn(object sender, RoutedEventArgs e)
        {
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("CLC");
            String[] clc = port.ReadLine().Split(',');
            port.WriteLine("CLC,1," + clc[2] + "," + clc[3] + "," + clc[4] + ",");
            port.ReadLine();
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void ChangerCloseCallDND(object sender, RoutedEventArgs e)
        {
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("CLC");
            String[] clc = port.ReadLine().Split(',');
            port.WriteLine("CLC,2," + clc[2] + "," + clc[3] + "," + clc[4] + ",");
            port.ReadLine();
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void changerCloseCallBipOn(object sender, RoutedEventArgs e)
        {
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("CLC");
            String[] clc = port.ReadLine().Split(',');
            port.WriteLine("CLC,"+clc[1]+",1," + clc[3] + "," + clc[4] + ",");
            port.ReadLine();
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void changerCloseCallBipOff(object sender, RoutedEventArgs e)
        {
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("CLC");
            String[] clc = port.ReadLine().Split(',');
            port.WriteLine("CLC," + clc[1] + ",0," + clc[3] + "," + clc[4] + ",");
            port.ReadLine();
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void changerCloseCallLumOn(object sender, RoutedEventArgs e)
        {
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("CLC");
            String[] clc = port.ReadLine().Split(',');
            port.WriteLine("CLC," + clc[1] + ","+clc[2]+",1," + clc[4] + ",");
            port.ReadLine();
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void changerCloseCallLumOff(object sender, RoutedEventArgs e)
        {
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("CLC");
            String[] clc = port.ReadLine().Split(',');
            port.WriteLine("CLC," + clc[1] + "," + clc[2] + ",0," + clc[4] + ",");
            port.ReadLine();
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void changerCloseCallVHFB(object sender, RoutedEventArgs e)
        {
            char et;
            if (ccvhfb.IsChecked == true)
                et = '1';
            else
                et = '0';
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("CLC");
            String[] clc = port.ReadLine().Split(',');
            char[] bandes = clc[4].ToCharArray();
            port.WriteLine("CLC," + clc[1] + "," + clc[2] + "," + clc[3] + "," + et + bandes[1]+bandes[2]+bandes[3]+bandes[4]+",");
            port.ReadLine();
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void changerCloseCallAIR(object sender, RoutedEventArgs e)
        {
            char et;
            if (ccair.IsChecked == true)
                et = '1';
            else
                et = '0';
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("CLC");
            String[] clc = port.ReadLine().Split(',');
            char[] bandes = clc[4].ToCharArray();
            port.WriteLine("CLC," + clc[1] + "," + clc[2] + "," + clc[3] + "," + bandes[0] + et + bandes[2] + bandes[3] + bandes[4] + ",");
            port.ReadLine();
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void changerCloseCallVHFH(object sender, RoutedEventArgs e)
        {
            char et;
            if (ccvhfh.IsChecked == true)
                et = '1';
            else
                et = '0';
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("CLC");
            String[] clc = port.ReadLine().Split(',');
            char[] bandes = clc[4].ToCharArray();
            port.WriteLine("CLC," + clc[1] + "," + clc[2] + "," + clc[3] + "," + bandes[0] + bandes[1] + et + bandes[3] + bandes[4] + ",");
            port.ReadLine();
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void changerCloseCallUHF(object sender, RoutedEventArgs e)
        {
            char et;
            if (ccuhf.IsChecked == true)
                et = '1';
            else
                et = '0';
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            port.WriteLine("CLC");
            String[] clc = port.ReadLine().Split(',');
            char[] bandes = clc[4].ToCharArray();
            port.WriteLine("CLC," + clc[1] + "," + clc[2] + "," + clc[3] + "," + bandes[0] + bandes[1] + bandes[2] + bandes[3] + et + ",");
            port.ReadLine();
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }
    }
}
