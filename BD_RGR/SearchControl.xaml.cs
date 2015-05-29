using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	/// Interaction logic for SearchControl.xaml
	/// </summary>
	public partial class SearchControl : UserControl
	{
		public SearchControl()
		{
			InitializeComponent();
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			((ListBox) sender).ItemsSource = ((IEnumerable<string>) ((ListBox) sender).ItemsSource).OrderBy(
				x =>
					((ListBox) sender).SelectedItems.Contains(x) ? 0 : 1);

			var handler = FilterChanged;
			if (handler != null) handler(this, EventArgs.Empty);
		}



		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			var handler = FilterChanged;
			if (handler != null) handler(this, EventArgs.Empty);
		}

		public event EventHandler FilterChanged;

	}
}
