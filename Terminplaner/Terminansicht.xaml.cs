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
    private List<Termin> Termine;
    public Terminansicht() {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      //
      // Init DatePicker
      //   Always select the first day of the current week as the default value.
      //
      DateTime StartOfWeek = DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek - DayOfWeek.Monday));
      this.DTPicker.SelectedDate = StartOfWeek; // Set DTP to the beginning of current week
      //
      // Create DataTable for Termine
      //
      DataTable TableTermine;
      GetTermineInfo(StartOfWeek);

              // this.DataGridTermine.DataContext = TableTermine.DefaultView;
    }
    private void GetTermineInfo(DateTime FirstDateOfWeek) {
      DataTable       TableTermine;
      OleDbConnection ConTermine;
      OleDbCommand    Command;
      
      TableTermine         = new DataTable();
      List<Termin> termine = new List<Termin>();
      //
      // Prepare 
      //
      
      
      
      
      
      
      //
      // Get data from Database
      //
      using (ConTermine = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Terminplaner.mdb")) {
        ConTermine.Open();
        Command = ConTermine.CreateCommand();
        Command.Connection = ConTermine;
        //Command.CommandText = "SELECT Termin.Datum, Termin.Uhrzeit, Termin.Info, " +
        //                      "Kontakt.Vorname + ' ' + Kontakt.Name as KontaktName" +
        //                      "FROM Termin INNER JOIN Kontakt" +
        //                      "ON   Termin.Kontakt=Kontakt.ID;";




        Command.CommandText = "SELECT Datum, Uhrzeit, Info, Kontakt " +
                              "FROM Termin " +
                              "Where Datum Between '" + FirstDateOfWeek + "' and '" +
                              FirstDateOfWeek.AddDays(7) + "' " +
                              "Order by Datum, Uhrzeit" +
                              ";";

//        Command.CommandText = "SELECT Datum " +
//                              "FROM Termin " +
//                              "Where Datum Between '" + FirstDateOfWeek.ToString() + "' and '" +
//                              FirstDateOfWeek.AddDays(7).ToString() + "'" +
//                              ";";

        OleDbDataReader ReaderTermine = Command.ExecuteReader();

        
        




        while (ReaderTermine.Read()) {
          termine.Add(new Termin() {
          Date = ReaderTermine.GetDateTime(0).ToShortDateString(),
          Day  = ReaderTermine.GetDateTime(0).DayOfWeek.ToString(),
          Time = ReaderTermine.GetDateTime(1).ToString("HH"),
          Info = ReaderTermine.GetString(2),
          KontaktID = ReaderTermine.GetString(3).Trim()
          });
        };
        foreach (Termin t in termine) {
          ReaderTermine.Close();
          Command.CommandText = "SELECT Vorname + ' ' + Nachname " +
                                "FROM Kontakt "   +
                                "WHERE ID="+ "2" + ";";
          ReaderTermine = Command.ExecuteReader();
          ReaderTermine.Read();
          t.Name = ReaderTermine.GetString(0);
        }
        //string KontaktID = TableTermine.Rows[0]["Kontakt"].ToString();
        //Command.CommandText = "SELECT Vorname + ' ' + Name " +
        //                      "FROM Termin"   +
        //                      "WHERE ID="+ KontaktID + ";";
        ConTermine.Close();
        for (int i = 0; i < 24 - 6; i++) {
          if (termine[i].Time != (i + 6).ToString()) { 
          }
        }

        
        this.Termine = termine;

      }

    }





    public class Termin {
      //
      // Required for Table
      //
      public string Day { get; set; }
      public string Time { get; set; }
      public string Info { get; set; }
      public string Name { get; set; }
      //
      // Useful additional info:
      //
      public string TerminID { get; set; }
      public string KontaktID { get; set; }
      public string Date { get; set; }
    }
  }
}
