using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Data.OleDb;

namespace Terminplaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ContactWindow : Window
    {
        private OleDbConnection Databank;
        private bool recentlyCleared  = false;
        private string DefaultPicture = "pack://application:,,,/Pictures/TestBild.png";
        public ContactWindow()
        {
            InitializeComponent();
            Databank = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Terminplaner.mdb");
            UpdateGrid();
        }

        public List<Contact> ReadDatabank()
        {
            List<Contact> contacts = new List<Contact>();
            Databank.Open();
            OleDbCommand Command = Databank.CreateCommand();
            Command.Connection = Databank;
            Command.CommandText = "SELECT * " +
                                  "From Kontakt";
            OleDbDataReader Reader = Command.ExecuteReader();
            while (Reader.Read())
            {
                string Bild = DefaultPicture;
                if (!Reader.IsDBNull(6))
                {
                    Bild = Reader.GetString(6);    // There may be no picture linked
                }

                contacts.Add(new Contact(Reader.GetInt32(0), Reader.GetString(1), Reader.GetString(2), Reader.GetString(3), Reader.GetString(4), Reader.GetString(5), Bild));
            }
            Databank.Close();
            return contacts;
        }

        public void AddToDatabank(Contact contact)
        {
            Databank.Open();
            OleDbCommand Command = Databank.CreateCommand();
            Command.Connection   = Databank;
            Command.CommandText  = "INSERT INTO Kontakt(Nachname, Vorname, Adresse, Telefon, EMail, Bild) " +
                                   "VALUES ('" + contact.Name    + "', '" +
                                                 contact.Vorname + "', '" +
                                                 contact.Adresse + "', '" +
                                                 contact.Telefon + "', '" +
                                                 contact.Email   + "', '" +
                                                 contact.Bild    + "');";
            Command.ExecuteNonQuery();
            Databank.Close();
            return;
        }

        public void EditDatabank(Contact contact)
        {
            if (recentlyCleared)
            {
                AddToDatabank(contact);
            }
            else
            {
                Databank.Open();
                OleDbCommand Command = Databank.CreateCommand();
                Command.Connection   = Databank;
                Contact selected     = (Contact)DataGrid.SelectedItem;
                Command.CommandText  = "UPDATE Kontakt " +
                                       "SET Nachname='" + contact.Name    + "', " +
                                            "Vorname='" + contact.Vorname + "', " +
                                            "Adresse='" + contact.Adresse + "', " +
                                            "Telefon='" + contact.Telefon + "', " +
                                            "EMail='"    + contact.Email   + "', " +
                                            "Bild='" + contact.Bild    + "' " +
                                            "WHERE ID=" + contact.id      + ";";
                Command.ExecuteNonQuery();
                Databank.Close();
            }
            return;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recentlyCleared = false;
            Contact selected = (Contact)DataGrid.SelectedItem;
            tb_name.Text     = selected.Name;
            tb_vorname.Text  = selected.Vorname;
            tb_adresse.Text  = selected.Adresse;
            tb_telefon.Text  = selected.Telefon;
            tb_email.Text    = selected.Email;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(selected.Bild);
            image.EndInit();
            p_bild.Source = image;
        }

        private void b_save_Click(object sender, RoutedEventArgs e)
        {
            Contact selected = (Contact)DataGrid.SelectedItem;
            Contact contact = new Contact(selected.id, tb_name.Text, tb_vorname.Text, tb_adresse.Text, tb_telefon.Text, tb_email.Text, p_bild.Source.ToString());
            EditDatabank(contact);
            UpdateGrid();
        }

        private void b_add_Click(object sender, RoutedEventArgs e)
        {
            Contact contact = new Contact(0, tb_name.Text, tb_vorname.Text, tb_adresse.Text, tb_telefon.Text, tb_email.Text, p_bild.Source.ToString())
            {
                Name    = tb_name.Text,
                Vorname = tb_vorname.Text,
                Adresse = tb_adresse.Text,
                Telefon = tb_telefon.Text,
                Email   = tb_email.Text,
                Bild    = p_bild.Source.ToString()
            };
            AddToDatabank(contact);
            UpdateGrid();
        }

        public  void UpdateGrid()
        {
            recentlyCleared = false;
            DataGrid.SelectedIndex = 0;
            DataGrid.ItemsSource = ReadDatabank();
            DataGrid.Items.Refresh();
            if (DataGrid.Columns.Count > 5)
            {
                DataGrid.Columns[0].Visibility = Visibility.Hidden;  // Hide id
                DataGrid.Columns[6].Visibility = Visibility.Hidden;  // Hide image path
            }
        }

        private void b_browse_Click(object sender, RoutedEventArgs e)
        {
            // Open browse dialog for choosing a picture
            Microsoft.Win32.OpenFileDialog browse = new Microsoft.Win32.OpenFileDialog();
            browse.Filter           = "All (*.*)|*.*|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg ";
            //browse.DefaultExt = ".png";
            browse.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            Nullable<bool> result   = browse.ShowDialog();
            if (!(result.HasValue && result.Value))
            {
                return;  // Cancel when input is incorrect
            }
            //string filename = System.IO.Path.GetFileName(browse.FileName);
            string filepath = browse.FileName;
            if (!File.Exists(filepath))
            {
                return;  // Cancel when path is not valid
            }
            // Read picture and refresh currently shown bitmap
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(filepath);
            image.EndInit();
            p_bild.Source = image;
        }

        private void b_clear_Click(object sender, RoutedEventArgs e)
        {
            recentlyCleared = true;
            tb_name.Clear();
            tb_vorname.Clear();
            tb_adresse.Clear();
            tb_telefon.Clear();
            tb_email.Clear();
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(DefaultPicture);
            image.EndInit();
            p_bild.Source = image;
        }
    }
    public class Contact
    {
        public int    id      { get; set; }
        public string Name    { get; set; }
        public string Vorname { get; set; }
        public string Adresse { get; set; }
        public string Telefon { get; set; }
        public string Email   { get; set; }
        public string Bild    { get; set; }
        public Contact(int id, string name, string vorname, string adresse, string telefon, string email, string bild)
        {
            this.id      = id;
            this.Name    = name;
            this.Vorname = vorname;
            this.Adresse = adresse;
            this.Telefon = telefon;
            this.Email   = email;
            this.Bild    = bild;
        }
    }
}
