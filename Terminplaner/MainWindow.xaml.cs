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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void b_contact_Click(object sender, RoutedEventArgs e)
        {
            ContactWindow c_window = new ContactWindow();
            c_window.Show();
            c_window.UpdateGrid();
            this.Close();
        }

        private void b_calendar_Click(object sender, RoutedEventArgs e)
        {
            Terminansicht Calender = new Terminansicht();
            Calender.Show();
            this.Close();
        }

        private void b_exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
