using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace shitproject
{
	public class CustomGridView : UserControl
	{
		public virtual void UpdateTable()
		{
		}

		public bool AllowEditing
		{
			get { return (bool)GetValue(AllowEditingProperty); }
			set { SetValue(AllowEditingProperty, value); }
		}

		public static readonly DependencyProperty AllowEditingProperty =
			DependencyProperty<CustomGridView>.Register(x => x.AllowEditing, false);

		public Db Database
		{
			get { return (Db)GetValue(DatabaseProperty); }
			set { SetValue(DatabaseProperty, value); }
		}

		public static readonly DependencyProperty DatabaseProperty =
			DependencyProperty<CustomGridView>.Register(x => x.Database, (d, e) => d.UpdateTable());

		public void Preparat_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			if (e.EditAction == DataGridEditAction.Commit)
				((Db.ISubmitRemove)e.Row.Item).Submit(Database);
		}

		public void Preparat_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Delete) return;

			var grid = (DataGrid)sender;

			if (grid.SelectedItems.Count <= 0 ||
				MessageBox.Show("Are you sure you want to delete?",
					"Delete", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
				return;

			foreach (var row in grid.SelectedItems)
			{
				((Db.ISubmitRemove)row).Remove(Database);
			}

		}
	}
}