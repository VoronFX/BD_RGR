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
			//Db.Realize.ItemsSource = Database.GetRealizes(DateFrom.SelectedDate.Value, DateTo.SelectedDate.Value);
		}

		public Db Database
		{
			get { return (Db)GetValue(DatabaseProperty); }
			set { SetValue(DatabaseProperty, value); }
		}

		public static readonly DependencyProperty DatabaseProperty =
			DependencyProperty<Budnic>.Register(x => x.Database, defaultValue: null);

		private void DateFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
		//	if (Database != null)
		//		if (DateFrom != null)
		//			if (DateTo != null)
		//				Db.Realize.ItemsSource = Database.GetRealizes(DateFrom.SelectedDate.Value, DateTo.SelectedDate.Value);
		}

		private void Realize_LoadingRow(object sender, DataGridRowEventArgs e)
		{
		//	// Adding 1 to make the row count start at 1 instead of 0
		//	// as pointed out by daub815
		//	e.Row.Header = e.Row.IsNewItem ? "new" : (e.Row.GetIndex() + 1).ToString();
		}

		private void Realize_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{

		}

		private void Realize_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			//e.EditAction == 
		}
	}
}
