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
			Database = new Db("da2105", "localhost", "da2107", "hvyM9cl0");
			Preparat.ItemsSource = Database.GetPreparats();
		}

		public Db Database { get; set; }
	}
}
