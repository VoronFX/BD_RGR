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
using MySql.Data.MySqlClient;

namespace shitproject
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Database = new Db("da2107", "localhost", "da2107", "hvyM9cl0");

			Filter.Products.ItemsSource = Database.GetProducts();
			Filter.Cuisines.ItemsSource = Database.GetCuisines();
		}


		public Db Database
		{
			get { return (Db)GetValue(DatabaseProperty); }
			set { SetValue(DatabaseProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Database.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DatabaseProperty =
			DependencyProperty.Register("Database", typeof(Db), typeof(MainWindow), new PropertyMetadata(null));


		private void Dishes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Dishes.SelectedItem == null)
			{
				Recipes.ItemsSource = null;
				return;
			}
			Recipes.ItemsSource = Database.GetRecipes(((Db.Dish)Dishes.SelectedItem).idDishes, Filter.Author.Text);
		}


		private void Recipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Recipes.SelectedItem == null)
			{
				Algorithm.Navigate("about:blanc");
				return;
			}
			Algorithm.NavigateToString("<meta content=\"text/html; charset=UTF-8\" http-equiv=\"content-type\">" + ((Db.Recipe)Recipes.SelectedItem).Algorithm);
		}

		private void Filter_OnFilterChanged(object sender, EventArgs e)
		{
			Dishes.ItemsSource = Database.GetDishes(
				name: Filter.Name.Text,
				author: Filter.Author.Text,
				cuisines: (Filter.Cuisines.SelectedItems.Count > 0 ? Filter.Cuisines.SelectedItems : Filter.Cuisines.Items).Cast<string>(),
				products: (Filter.Products.SelectedItems.Count > 0 ? Filter.Products.SelectedItems : Filter.Products.Items).Cast<string>());

		}
	}
}
