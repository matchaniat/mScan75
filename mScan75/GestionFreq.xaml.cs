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
using System.Diagnostics;

namespace mScan75
{
    /// <summary>
    /// Logique d'interaction pour GestionFreq.xaml
    /// </summary>
    public partial class GestionFreq : Window
    {
        private SerialPort port;
        public ObservableCollection<Memoire> test { get; set; }
        public GestionFreq(SerialPort portv)
        {
            InitializeComponent();
            this.port = portv;
            this.DataContext = this;
            test = new ObservableCollection<Memoire>();
            lireFrequences();
        }
        public void lireFrequences(){
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            for (int i=1;i<=30;i++)
            {
                port.WriteLine("CIN," + i.ToString());
                String[] donn = port.ReadLine().Split(',');
                test.Add(new Memoire(donn[1], ((Double.Parse(donn[3])/10000.0).ToString()), donn[4],true, false, false));
            }
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }   
    }
    public class Memoire
    {
        private string _numero;
        private string _freq;
        private string _modu;
        private bool _delai;
        private bool _exclu;
        private bool _pri;

        public Memoire(string n,string f,string m,bool d,bool e,bool p)
        {
            _numero = n;
            _freq = f;
            _modu = m;
            _delai = d;
            _exclu = e;
            _pri = p;
        }
        public string Num
        {
            get { return _numero; }
            set { _numero = value; }
        }
        public string Freq
        {
            get { return _freq; }
            set { _freq = value; }
        }
        public string Modul
        {
            get { return _modu; }
            set { _modu = value; }
        }
        public bool Delai
        {
            get { return _delai; }
            set { _delai = value; }
        }
        public bool Exclu
        {
            get { return _exclu; }
            set { _exclu = value; }
        }
        public bool Pri
        {
            get { return _pri; }
            set { _pri = value; }
        }
    }
}
