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
using System.Data.OleDb;
using System.Globalization;

namespace Terminplaner {
  /// <summary>
  /// Interaktionslogik für Terminansicht.xaml
  /// </summary>
  public partial class Terminansicht : Window {
    private List<Termin> Termine;
    public Terminansicht() {
      InitializeComponent();
    }

    /***************************************************
    * 
    *  Window_Loaded()
    *
    ****************************************************
    */
    private void Window_Loaded(object sender, RoutedEventArgs e) {
      //
      // Init DatePicker
      //   Always select the first day of the current week as the default value.
      //
      DateTime StartOfWeek = DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek - DayOfWeek.Monday));
      this.DTPicker.SelectedDate = StartOfWeek; // Set DTP to the beginning of current week
      this.GetTermineInfo(StartOfWeek);
      this.TerminToList(StartOfWeek);

    }
    /***************************************************
    * 
    *  GetTermineInfo()
    *
    ****************************************************
    */
    private void GetTermineInfo(DateTime FirstDateOfWeek) {
      DataTable       TableTermine;
      OleDbConnection ConTermine;
      OleDbDataReader ReaderTermine;
      OleDbCommand    Command;
      
      TableTermine         = new DataTable();
      List<Termin> termine = new List<Termin>();
      //
      // Get relevant data from Database
      //
      using (ConTermine = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Terminplaner.mdb")) {
        ConTermine.Open();
        Command = ConTermine.CreateCommand();
        Command.Connection = ConTermine;
        Command.Parameters.AddWithValue("FirstDay", FirstDateOfWeek);
        Command.Parameters.AddWithValue("LastDay", FirstDateOfWeek.AddDays(7));
        Command.CommandText = "SELECT Termin.Datum, Termin.Uhrzeit, Termin.Info, Termin.Terminname, Termin.ID, " +
                              "Kontakt.Vorname + ' ' + Kontakt.Nachname, " + "Kontakt.ID " +
                              "FROM Termin " +
                              "INNER JOIN Kontakt ON Termin.KontaktID=Kontakt.ID " +
                              "Where Datum Between @FirstDay and @LastDay " +
                              "Order by Datum, Uhrzeit" +
                              ";";
        ReaderTermine = Command.ExecuteReader();
        //
        // Fill list with data
        //
        while (ReaderTermine.Read()) {
          termine.Add(new Termin() {
           Date       = ReaderTermine.GetDateTime(0)
          ,Day        = ReaderTermine.GetDateTime(0).DayOfWeek.ToString()
          ,Time       = ReaderTermine.GetDateTime(1)
          ,Info       = ReaderTermine.GetString(2)
          ,Terminname = ReaderTermine.GetString(3)
          ,TerminID   = ReaderTermine.GetInt32(4)
          ,Kontakt    = ReaderTermine.GetString(5)
          ,KontaktID  = ReaderTermine.GetInt32(6)
          });;
        };
        ConTermine.Close();
      }
      this.Termine = termine;
    }

    /**********************************
    *
    *  TerminToList()
    *
    *    Description:
    *      Inserts a contact into the datatable.
    */
    private void TerminToList(DateTime SelWeek) {
      if (this.Termine == null) {
        return;
      }
      DataTable TerminDaten = new DataTable();
      //
      // Init Rows
      //
      TerminDaten.Columns.Add("Uhrzeit");
      TerminDaten.Columns.Add("Monday");
      TerminDaten.Columns.Add("Tuesday");
      TerminDaten.Columns.Add("Wednesday");
      TerminDaten.Columns.Add("Thursday");
      TerminDaten.Columns.Add("Friday");
      TerminDaten.Columns.Add("Saturday");
      TerminDaten.Columns.Add("Sunday");
      //
      // Init colums
      //
      for(int i = 6; i <= 24; i++) {
        DataRow dr = TerminDaten.NewRow();
        dr["Uhrzeit"] = String.Format("{0:D2}:00", i);
        TerminDaten.Rows.Add(dr);
      }
      //
      // Add values to rows
      //
      List<Termin> ArbList = this.Termine.Where(x => GetIso8601WeekOfYear(x.Date) == GetIso8601WeekOfYear(SelWeek)).ToList();
      ArbList.ForEach(x => {
        int Stunde = x.Time.Hour;
        string Day = x.Day;

        DataRow dr = TerminDaten.Rows[Stunde - 6];
        dr[Day] = x.Terminname;
      });
      this.DataGridTermine.DataContext = TerminDaten.DefaultView;
    }

    /**********************************
    *
    *  GetIso8601WeekOfYear()
    *
    *    Description:
    *      Returns Week of year
    */
    public int GetIso8601WeekOfYear(DateTime time) {
      // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
      // be the same week# as whatever Thursday, Friday or Saturday are,
      // and we always get those right
      DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
      if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday) {
        time = time.AddDays(3);
      }
      //
      // Return the week of our adjusted day
      //
      return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }

    /**********************************
    *
    *  DTPicker_SelectedDateChanged()
    *
    *    Description:
    *      DPChanged event
    */
    private void DTPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) {
      if (this.DTPicker.SelectedDate.HasValue) {
        this.TerminToList(this.DTPicker.SelectedDate.Value);
      }
    }

    /**********************************
    *
    *  EventSetter_OnHandler()
    *
    *    Description:
    *      MouseOver event
    */
    private void EventSetter_OnHandler(object sender, MouseEventArgs e) {
      string       Tag;
      int          Stunde;
      DataGridCell dgc = sender as DataGridCell;


      if (!this.DTPicker.SelectedDate.HasValue || dgc == null) {
        return;
      }
      
      switch (dgc.Column.Header) {
        case "Montag":     Tag = "Monday";    break;
        case "Dienstag":   Tag = "Tuesday";   break;
        case "Mittwoch":   Tag = "Wednesday"; break;
        case "Donnerstag": Tag = "Thursday";  break;
        case "Freitag":    Tag = "Friday";    break;
        case "Samstag":    Tag = "Saturday";  break;
        case "Sonntag":    Tag = "Sunday";    break;
        default:           Tag = "";          break;
      }
      Stunde        = Convert.ToInt32(((string)((DataRowView)dgc.DataContext).Row.ItemArray[0]).Substring(0, 2));
      Termin termin = this.Termine.FirstOrDefault(x => GetIso8601WeekOfYear(x.Date) == GetIso8601WeekOfYear(this.DTPicker.SelectedDate.Value) && x.Time.Hour == Stunde && x.Day.Equals(Tag));

      if(termin != null) {
        this.LblDate.Content    = new TextBlock() { Text = termin.Date.ToString("dd.MM.yyyy"), TextWrapping = TextWrapping.Wrap };
        this.LblTime.Content    = new TextBlock() { Text = termin.Time.ToString("HH.mm"), TextWrapping = TextWrapping.Wrap };
        this.LblContact.Content = new TextBlock() { Text = termin.Kontakt, TextWrapping = TextWrapping.Wrap };
        this.LblAddInfo.Content = new TextBlock() { Text = termin.Info, TextWrapping = TextWrapping.Wrap };
      } else {
        this.LblDate.Content    = new TextBlock() { Text = String.Empty, TextWrapping = TextWrapping.Wrap };
        this.LblTime.Content    = new TextBlock() { Text = String.Empty, TextWrapping = TextWrapping.Wrap };
        this.LblContact.Content = new TextBlock() { Text = String.Empty, TextWrapping = TextWrapping.Wrap };
        this.LblAddInfo.Content = new TextBlock() { Text = String.Empty, TextWrapping = TextWrapping.Wrap };
      }
    }

    /**********************************
    *
    *  EventSetter_OnHandler()
    *
    *    Description:
    *      MouseOver event
    */
    private void DataGridTermine_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
      string       Tag;
      int          Stunde;
      DataGridCell dgc = sender as DataGridCell;


      if (!this.DTPicker.SelectedDate.HasValue || dgc == null) {
        return;
      }
      
      switch (dgc.Column.Header) {
        case "Montag":     Tag = "Monday";    break;
        case "Dienstag":   Tag = "Tuesday";   break;
        case "Mittwoch":   Tag = "Wednesday"; break;
        case "Donnerstag": Tag = "Thursday";  break;
        case "Freitag":    Tag = "Friday";    break;
        case "Samstag":    Tag = "Saturday";  break;
        case "Sonntag":    Tag = "Sunday";    break;
        default:           Tag = "";          break;
      }
      Stunde        = Convert.ToInt32(((string)((DataRowView)dgc.DataContext).Row.ItemArray[0]).Substring(0, 2));
      Termin termin = this.Termine.FirstOrDefault(x => GetIso8601WeekOfYear(x.Date) == GetIso8601WeekOfYear(this.DTPicker.SelectedDate.Value) && x.Time.Hour == Stunde && x.Day.Equals(Tag));
      if (termin != null) {
        this.EditTermin(termin);
      } else {
        this.AddTermin(Tag, Stunde);
      }

    }

    private void AddTermin(string Tag, int Stunde) {
      Termin termin    = new Termin();
      termin.Day       = Tag;
      termin.Time     += new TimeSpan(Stunde, 0, 0);
      termin.Date      = new DateTime(this.DTPicker.SelectedDate.Value.Year,
                                      this.DTPicker.SelectedDate.Value.Month,
                                      this.DTPicker.SelectedDate.Value.Day,
                                      0, 0, 0);
      EditTerminWindow Dlg = new EditTerminWindow(termin);
      if (Dlg.ShowDialog().Value) {
        CreateTermin(termin); // FF xxxxxx TBD
      }
    }

    private void EditTermin(Termin termin) {
      EditTerminWindow Dlg = new EditTerminWindow(termin);
      if (Dlg.ShowDialog().Value) {
        UpdateTermin(termin); // FF xxxxxx TBD
      }
    }
    void CreateTermin(Termin termin) { // FF xxxxx TBD
      string sCmd = "INSERT INTO Termin(Datum, Uhrzeit, Info, Terminname, KontaktID) " +
                         "VALUES (" + termin.Date    + ", " +
                                      termin.Time + ", " +
                                      termin.Info + ", " +
                                      termin.Terminname + ", " +
                                      termin.KontaktID   + ";";
      //  SendQuery(sCmd);
    }
    void UpdateTermin(Termin termin) { // FF xxxx TBD
      string sCmd = "";
      //  SendQuery(sCmd);
    }

    /**********************************
    *
    *  Class: Termin()
    *
    *    Description:
    *      Termin class
    */
    public class Termin {
      //
      // Required for Table
      //
      public string Day { get; set; }
      public DateTime Time { get; set; }
      public string Info { get; set; }
      public string Terminname { get; set; }
      public string Kontakt { get; set; }
      //
      // Useful additional info:
      //
      public int TerminID { get; set; }
      public int KontaktID { get; set; }
      public DateTime Date { get; set; }
    }
  }
}
