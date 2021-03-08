using System;
using System.Collections.Generic;
using System.IO;
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
        private List<Contact> MockDatabase;
        private int ID;
        private OleDbConnection Databank;
        public ContactWindow()
        {
            InitializeComponent();
            this.ID = 0;
            Databank = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Terminplaner.mdb");
            //FillMockDatabase();
            //DataGrid.ItemsSource = FillMockDatabase();
            DataGrid.ItemsSource = ReadDatabank();
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
                string Bild = "E:\\Ausbildung\\OnlineSchule\\AWE Project\\AWE Project\\Terminplaner\\Pictures\\TestBild.png";
                if (!Reader.IsDBNull(6))
                {
                    Bild = Reader.GetString(6);    // There may be no picture linked
                }
                
                contacts.Add(new Contact()
                {
                    id      = Reader.GetInt32(0),
                    Name    = Reader.GetString(1),
                    Vorname = Reader.GetString(2),
                    Adresse = Reader.GetString(3),
                    Telefon = Reader.GetString(4),
                    Email   = Reader.GetString(5),
                    Bild    = Bild
                });
            }
            Databank.Close();
            return contacts;
        }

        public void AddToDatabank(Contact contact)
        {
            Databank.Open();
            OleDbCommand Command = Databank.CreateCommand();
            Command.Connection = Databank;
            string CMD = "INSERT INTO Kontakt " +
                         "VALUES (" + contact.Name    + ", " +
                                      contact.Vorname + ", " +
                                      contact.Adresse + ", " +
                                      contact.Telefon + ", " +
                                      contact.Email   + ", " +
                                      contact.Bild    + ");";
            Command.ExecuteNonQuery();
            Databank.Close();
            return;
        }

        public void EditDatabank(Contact contact)
        {
            Databank.Open();
            OleDbCommand Command = Databank.CreateCommand();
            Command.Connection = Databank;
            Contact selected = (Contact)DataGrid.SelectedItem;
            string CMD = "UPDATE Kontakt " +
                         "SET Nachname="   + contact.Name    + ", " +
                              "Vorname="   + contact.Vorname + ", " +
                              "Adresse="   + contact.Adresse + ", " +
                              "Telefon="   + contact.Telefon + ", " +
                              "EMail="     + contact.Email   + ", " +
                              "Bild="      + contact.Bild    + " " + 
                              "WHERE ID="  + contact.id      + ";";
            return;
        }
        public List<Contact> FillMockDatabase() 
        {
            List<Contact> contacts = new List<Contact>();
            contacts.Add(new Contact()
            {
                Name    = "Hansen",
                Vorname = "Frank",
                Adresse = "Baumallee 11, 40724 Hilden",
                Telefon = "02103-1828228",
                Email   = "Hansen@Hansen.de",
                Bild    = "E:\\Ausbildung\\OnlineSchule\\AWE Project\\AWE Project\\Terminplaner\\Pictures\\TestBildRed.png"
            });
            contacts.Add(new Contact()
            {
                Name    = "Knudsen",
                Vorname = "Karl",
                Adresse = "Uferweg 12, 40724 Hilden",
                Telefon = "02103-383838",
                Email   = "Karl@Knudsen.de",
                Bild    = "E:\\Ausbildung\\OnlineSchule\\AWE Project\\AWE Project\\Terminplaner\\Pictures\\TestBildGreen.png"
            });
            return contacts;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
            selected.Name    = tb_name.Text;
            selected.Vorname = tb_vorname.Text;
            selected.Adresse = tb_adresse.Text;
            selected.Telefon = tb_telefon.Text;
            selected.Email   = tb_email.Text;
            selected.Bild    = p_bild.Source.ToString();
            UpdateGrid();
        }

        private void b_add_Click(object sender, RoutedEventArgs e)
        {
            List<Contact> contacts = (List<Contact>)DataGrid.ItemsSource;
            contacts.Add(new Contact()
            {
                Name    = tb_name.Text,
                Vorname = tb_vorname.Text,
                Adresse = tb_adresse.Text,
                Telefon = tb_telefon.Text,
                Email   = tb_email.Text,
                Bild    = p_bild.Source.ToString()
            });
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            DataGrid.Items.Refresh();
        }

        private int getNextID()
        {
            this.ID++;
            return this.ID;
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
    }
}
