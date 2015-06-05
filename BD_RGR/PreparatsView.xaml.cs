using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace shitproject
{
	/// <summary>
	/// Interaction logic for PreparatsView.xaml
	/// </summary>
	public partial class PreparatsView : CustomGridView
	{

		public PreparatsView()
		{
			InitializeComponent();
		}

		public override void UpdateTable()
		{
			if (Database != null)
				Preparat.ItemsSource = Database.GetPreparats(
					Name.Text, Proizvoditel.Text,
					Izmerenia.Text, Country.Text,
					CountMin.Value.Value, CountMax.Value.Value);
		}

		private void SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateTable();
		}

		private void TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateTable();
		}

	}
}
