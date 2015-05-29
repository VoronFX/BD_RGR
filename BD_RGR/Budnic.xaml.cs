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

namespace shitproject
{
	/// <summary>
	/// Interaction logic for Budnic.xaml
	/// </summary>
	public partial class Budnic : Window
	{
		public Budnic()
		{
			InitializeComponent();
			Database = new Db("da2105", "localhost", "da2105", "Stws65ls");
			Preparat.ItemsSource = Database.GetPreparats();
			Realize.ItemsSource = Database.GetRealizes(DateFrom.SelectedDate.Value, DateTo.SelectedDate.Value);
		}

		public Db Database { get; set; }

		private void DateFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Database != null)
			if (DateFrom != null)
				if (DateTo != null)
					Realize.ItemsSource = Database.GetRealizes(DateFrom.SelectedDate.Value, DateTo.SelectedDate.Value);
		}
	}
}
