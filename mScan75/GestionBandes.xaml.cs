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
using System.IO.Ports;
using System.Collections.ObjectModel;

namespace mScan75
{
    /// <summary>
    /// Logique d'interaction pour GestionBandes.xaml
    /// </summary>
    public partial class GestionBandes : Window
    {
        private SerialPort port;
        public ObservableCollection<Bande> data { get; set; }
        public GestionBandes(SerialPort portv)
        {
            InitializeComponent();
            this.port = portv;
            this.DataContext = this;
            data = new ObservableCollection<Bande>();
            lireBandes();
        }
        public void lireBandes()
        {
            data.Clear();
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            for (int i=1;i<=10;i++)
            {
                port.WriteLine("CSP," + i.ToString());
                String[] donn = port.ReadLine().Split(',');
                data.Add(new Bande(donn[1], ((Double.Parse(donn[2]) / 10000.0).ToString()), ((Double.Parse(donn[3]) / 10000.0).ToString())));
            }
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void envoyerBandes(object sender, RoutedEventArgs e)
        {
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            foreach (Bande ligne in data)
            {
                string d = ((Double.Parse(ligne.Debut)) * 10000.0).ToString();
                string f = ((Double.Parse(ligne.Fin)) * 10000.0).ToString();
                port.WriteLine("CSP," + ligne.Num + ","+d+","+f);
                if (port.ReadLine().Contains("ERR"))
                {
                    MessageBox.Show("Erreur ligne "+ligne.Num, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
            lireBandes();
            MessageBox.Show("Données envoyées", "Envoyé", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    public class Bande
    {
        private string _numero;
        private string _debut;
        private string _fin;
        public Bande(string n,string d,string f)
        {
            _numero = n;
            _debut = d;
            _fin = f;
        }
        public string Num
        {
            get { return _numero; }
            set { _numero = value; }
        }
        public string Debut
        {
            get { return _debut; }
            set { _debut = value; }
        }
        public string Fin
        {
            get { return _fin; }
            set { _fin = value; }
        }
    }
}
