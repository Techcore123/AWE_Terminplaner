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

namespace Terminplaner {
  /// <summary>
  /// Interaktionslogik für EditTerminWindow.xaml
  /// </summary>
  public partial class EditTerminWindow : Window {
    private Terminansicht.Termin TempTermin;
    public EditTerminWindow() {
      InitializeComponent();
    }
    public EditTerminWindow(Terminansicht.Termin termin) {
      InitializeComponent();
      this.TempTermin          = termin;
      this.TBName.Text         = termin.Terminname;
      this.DPDate.SelectedDate = termin.Date;
      this.TBTime.Text         = termin.Time.ToString("HH.mm");
      this.TBKtkt.Text         = termin.KontaktID.ToString();
      this.TBInfo.Text         = termin.Info;
    }

    private void BtnOK_Click(object sender, RoutedEventArgs e) {
      this.TempTermin.Terminname = this.TBName.Text;
      this.TempTermin.Date       = this.DPDate.SelectedDate.Value;
      //this.TempTermin.Time       = DateTime.Parse(this.TBTime.Text); // FF xxx Does not work yet.
      this.TempTermin.KontaktID  = Convert.ToInt32(this.TBKtkt.Text);
      this.TempTermin.Info       = this.TBInfo.Text;
      this.DialogResult = true;
      this.Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e) {
      this.DialogResult = false;
      this.Close();
    }
  }
}
