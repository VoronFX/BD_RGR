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

namespace shitproject
{
	/// <summary>
	/// Interaction logic for IncomeView.xaml
	/// </summary>
	public partial class IncomeView : CustomGridView
	{
		public IncomeView()
		{
			InitializeComponent();
		}

		public override void UpdateTable()
		{
			if (Database != null)
				if (DateFrom != null)
					if (DateTo != null)
						Income.ItemsSource = Database.GetIncome(
							//Name.Text, PriceMin.Value.Value, PriceMax.Value.Value,
							DateFrom.SelectedDate.Value, DateTo.SelectedDate.Value);
		}


		private void TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateTable();
		}

		private void SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateTable();
		}

		private void EventSetter_OnHandler(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			UpdateTable();
		}
	}
}
