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
using System.Data;
//using System.Data.SqlClient;
using System.Data.OleDb;

namespace Terminplaner {
  /// <summary>
  /// Interaktionslogik für Terminansicht.xaml
  /// </summary>
  public partial class Terminansicht : Window {
    public Terminansicht() {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      DataTable TableTermine = new DataTable();
      using (OleDbConnection ConTermine = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Terminplaner.mdb")) {
        ConTermine.Open();
        OleDbCommand Command = ConTermine.CreateCommand();
        Command.Connection = ConTermine;
        // Command.CommandText = "INSERT INTO Termin(Datum, Uhrzeit, Kontakt) " +
        //                       "VALUES('25.03.2021', '16:00:00', 2)";
        int i = Command.ExecuteNonQuery();


        // Insert into Teilnehmer(Name, Vorname, Schule, SchuleSeit, Erfahrung, Cobol,
        // Basic, C, CPlus, Delphi, Java, CSharp)
        // Values(’Mustermann’, ’Gabi’, ’BKHilden’, ’02.09.2002’, 3, false, false,
        // true, true, false, false, true);




        // Command.CommandText = "SELECT STR(ID) + ' ' + Vorname + ' ' + Nachname FROM Kontakt WHERE ID=2;";
        Command.CommandText = "Select STR(ID) + ' ' + STR(Datum) + ' ' + STR(Uhrzeit) + ' ' + STR(Kontakt) From Termin;";
        // Command.CommandText = "Select Termin.Kontakt, Termin.Datum, Termin.Uhrzeit, Kontakt.Vorname, Kontakt.Nachname From Termin INNER JOIN Kontakt ON Termin.Kontakt=Kontakt.ID;";

        OleDbDataReader ReaderTermine = Command.ExecuteReader();
        ReaderTermine.Read();
//        ReaderTermine.Read();
        Type test = ReaderTermine.GetFieldType(0);
        string teststring = ReaderTermine.GetString(0);
        TableTermine.Load(ReaderTermine);
        ConTermine.Close();
        //string ID        = TableTermine.Rows[0]["Vorname"].ToString();
        //string Datum     = TableTermine.Rows[0]["Datum"].ToString();
        //string Uhrzeit   = TableTermine.Rows[0]["Uhrzeit"].ToString();
        //string KontaktID = TableTermine.Rows[0]["Kontakt"].ToString();
        

      }
        this.DataGridTermine.DataContext = TableTermine.DefaultView;
    }
  }
}
