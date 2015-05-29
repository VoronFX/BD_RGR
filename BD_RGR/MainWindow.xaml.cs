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
            Database = new Db("da2101", "localhost", "da2101", "iRzQr1Oy");

			Filter.Types.ItemsSource = Database.GetTypes();
			Filter.Entities.ItemsSource = Database.GetEntities();
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
            //if (Dishes.SelectedItem == null)
            //{
            //    Recipes.ItemsSource = null;
            //    return;
            //}
            //Recipes.ItemsSource = Database.GetRecipes(((Db.Resourse)Dishes.SelectedItem).idDishes, Filter.Author.Text);
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
			Dishes.ItemsSource = Database.GetResourses(
				name: Filter.Name.Text,
				author: Filter.Author.Text,
                entities: (Filter.Entities.SelectedItems.Count > 0 ? Filter.Entities.SelectedItems : Filter.Entities.Items).Cast<string>(),
                type: (Filter.Types.SelectedItems.Count > 0 ? Filter.Types.SelectedItems : Filter.Types.Items).Cast<string>(),
                from: Filter.FromDate.SelectedDate.Value,
                to: Filter.ToDate.SelectedDate.Value
                
                );

		}
	}
}
