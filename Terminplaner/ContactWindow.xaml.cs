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

namespace Terminplaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ContactWindow : Window
    {
        private List<Contact> MockDatabase;
        public ContactWindow()
        {
            InitializeComponent();
            FillMockDatabase();
            DataGrid.ItemsSource = FillMockDatabase();
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
                Email   = "Hansen@Hansen.de"
            });
            contacts.Add(new Contact()
            {
                Name    = "Knudsen",
                Vorname = "Karl",
                Adresse = "Uferweg 12, 40724 Hilden",
                Telefon = "02103-383838",
                Email   = "Karl@Knudsen.de"
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void b_save_Click(object sender, RoutedEventArgs e)
        {
            Contact selected = (Contact)DataGrid.SelectedItem;
            selected.Name = tb_name.Text;
            selected.Vorname = tb_vorname.Text;
            selected.Adresse = tb_adresse.Text;
            selected.Telefon = tb_telefon.Text;
            selected.Email = tb_email.Text;
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            DataGrid.Items.Refresh();
        }
    }
    public class Contact
    {
        public string Name { get; set; }
        public string Vorname { get; set; }
        public string Adresse { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
    }
}
