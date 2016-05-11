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
using System.IO;

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
            lireFrequences(1,30);
        }
        public void lireFrequences(int debut,int fin){
            test.Clear();
            port.Open();
            port.WriteLine("PRG");
            port.ReadLine();
            for (int i=debut;i<=fin;i++)
            {
                port.WriteLine("CIN," + i.ToString());
                String[] donn = port.ReadLine().Split(',');
                bool d,lo,pr;
                if (donn[6].Equals("1"))
                    d = true;
                else
                    d = false;
                if (donn[7].Equals("1"))
                    lo = true;
                else
                    lo = false;
                if (donn[8].Equals("1"))
                    pr = true;
                else
                    pr = false;
                test.Add(new Memoire(donn[1], ((Double.Parse(donn[3])/10000.0).ToString()), donn[4],d, lo, pr));
            }
            port.WriteLine("EPG");
            port.ReadLine();
            port.Close();
        }

        private void changerBanque(object sender, RoutedEventArgs e)
        {
            char t2='1';
            char t= (((Button)sender).Content.ToString())[7];
            try { t2 = (((Button)sender).Content.ToString())[8]; } catch (Exception ex) { };
            if (t.Equals('1') && t2.Equals('1'))
                lireFrequences(1, 30);
            else if (t.Equals('2'))
                lireFrequences(31, 60);
            else if (t.Equals('3'))
                lireFrequences(61, 90);
            else if (t.Equals('4'))
                lireFrequences(91, 120);
            else if (t.Equals('5'))
                lireFrequences(121, 150);
            else if (t.Equals('6'))
                lireFrequences(151, 180);
            else if (t.Equals('7'))
                lireFrequences(181, 210);
            else if (t.Equals('8'))
                lireFrequences(211, 240);
            else if (t.Equals('9'))
                lireFrequences(241, 270);
            else if (t.Equals('1') && t2.Equals('0')) 
                lireFrequences(271, 300);
        }

        private void envoyerFrequences(object sender, RoutedEventArgs e)
        {
            foreach(Memoire ligne in test)
            {
                if(!(ligne.Freq.Equals("0")))
                {
                    string freq = ((Double.Parse(ligne.Freq)) * 10000.0).ToString();
                    char delai, lo, pri;
                    if (ligne.Delai)
                        delai = '1';
                    else
                        delai = '0';
                    if (ligne.Exclu)
                        lo = '1';
                    else
                        lo = '0';
                    if (ligne.Pri)
                        pri = '1';
                    else
                        pri = '0';
                    port.Open();
                    port.WriteLine("PRG");
                    port.ReadLine();
                    port.WriteLine("CIN," + ligne.Num + ",," + freq + "," + ligne.Modul + ",," + delai + "," + lo + "," + pri);
                    if(port.ReadLine().Contains("ERR"))
                    {
                        MessageBox.Show("Erreur","Erreur",MessageBoxButton.OK,MessageBoxImage.Error);
                        port.WriteLine("EPG");
                        port.ReadLine();
                        port.Close();
                        return;
                    }
                    port.WriteLine("EPG");
                    port.ReadLine();
                    port.Close();
                }
            }
            MessageBox.Show("Données envoyées", "Envoyé", MessageBoxButton.OK, MessageBoxImage.Information);
            lireFrequences(Int32.Parse(test.First().Num),Int32.Parse(test.Last().Num));
        }

        private void enregistrerFichier(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog opf = new Microsoft.Win32.SaveFileDialog();
            opf.DefaultExt = "csv";
            opf.Filter = "Fichier CSV (*.csv)|*.csv";
            if (opf.ShowDialog(this)==true)
            {
                /*Gestion des erreurs*/
                try
                {
                    /*Création du stream d'ecriture de ton fichier avec en 1er paramètre le lien de ton fichier(openFileDialog1.FileName retourne le lien du fichier que tu as séléctionné)*/
                    StreamWriter fichier = new StreamWriter(opf.FileName,false,Encoding.UTF8);
                    fichier.WriteLine("Numéro;Fréquence;Commentaire");
                    port.Open();
                    port.WriteLine("PRG");
                    port.ReadLine();

                    
                    for (int i=1;i<=300;i++)
                    {
                        port.WriteLine("CIN," + i.ToString());
                        String[] donn = port.ReadLine().Split(',');
                        if (Int32.Parse(donn[3])!=0)
                            fichier.WriteLine(donn[1] + ";" + ((Double.Parse(donn[3]) / 10000.0).ToString()));
                    }
                    port.WriteLine("EPG");
                    port.ReadLine();
                    port.Close();
                    fichier.Close();
                    MessageBox.Show("Fichier enregistré !");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur: Impossible de d'écrire le fichier.");
                }
            }
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
