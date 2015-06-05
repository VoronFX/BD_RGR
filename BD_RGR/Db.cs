using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace shitproject
{
	#region ext
		
	public static class DbExtensionMethods
	{
		public static bool HasColumn(this MySqlDataReader dr, string columnName)
		{
			for (int i = 0; i < dr.FieldCount; i++)
			{
				if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
					return true;
			}
			return false;
		}

		public static Int32 SafeGetInt32(this MySqlDataReader reader, string colName)
		{
			if (!reader.HasColumn(colName)) return 0;
			return reader.SafeGetInt32(reader.GetOrdinal(colName));
		}

		public static Int32 SafeGetInt32(this MySqlDataReader reader, int colIndex)
		{
			return !reader.IsDBNull(colIndex) ? reader.GetInt32(colIndex) : 0;
		}

		public static string SafeGetString(this MySqlDataReader reader, string colName)
		{
			if (!reader.HasColumn(colName)) return null;
			return reader.SafeGetString(reader.GetOrdinal(colName));
		}

		public static string SafeGetString(this MySqlDataReader reader, int colIndex)
		{
			return !reader.IsDBNull(colIndex) ? reader.GetString(colIndex) : string.Empty;
		}


		public static DateTime SafeGetDateTime(this MySqlDataReader reader, string colName)
		{
			if (!reader.HasColumn(colName)) return default(DateTime);
			return reader.SafeGetDateTime(reader.GetOrdinal(colName));
		}

		public static DateTime SafeGetDateTime(this MySqlDataReader reader, int colIndex)
		{
			return !reader.IsDBNull(colIndex) ? reader.GetDateTime(colIndex) : default(DateTime);
		}


		public static Double SafeGetDouble(this MySqlDataReader reader, string colName)
		{
			if (!reader.HasColumn(colName)) return default(Double);
			return reader.SafeGetDouble(reader.GetOrdinal(colName));
		}

		public static Double SafeGetDouble(this MySqlDataReader reader, int colIndex)
		{
			return !reader.IsDBNull(colIndex) ? reader.GetDouble(colIndex) : default(Double);
		}
		public static void Extract(this MySqlDataReader reader, object targetTo)
		{
			foreach (var property in targetTo.GetType().GetProperties())
				reader.ExtractProperty(property, targetTo);
		}

		public static bool ExtractProperty(this MySqlDataReader reader, PropertyInfo propertyInfo, object targetTo)
		{
			if (propertyInfo.PropertyType == typeof(int))
				propertyInfo.SetValue(targetTo, reader.SafeGetInt32(propertyInfo.Name));
			else if (propertyInfo.PropertyType == typeof(string))
				propertyInfo.SetValue(targetTo, reader.SafeGetString(propertyInfo.Name));
			else if (propertyInfo.PropertyType == typeof(DateTime))
				propertyInfo.SetValue(targetTo, reader.SafeGetDateTime(propertyInfo.Name));
			else if (propertyInfo.PropertyType == typeof(Double))
				propertyInfo.SetValue(targetTo, reader.SafeGetDouble(propertyInfo.Name));
			else return false;
			return true;
		}
	}
 
	#endregion
	public class Db : DependencyNotifyPropertyChanged, IDisposable
	{
		#region QueryHelper

		public QueryResult Query(string query)
		{
			return new QueryResult(query, myConnection);
		}

		public struct QueryResult : IEnumerable<MySqlDataReader>, IDisposable
		{
			MySqlCommand myCommand;

			public QueryResult(string query, MySqlConnection connection)
			{
				myCommand = new MySqlCommand(query, connection);
				enumerator = null;
			}
			struct QueryResultEnumerator : IEnumerator<MySqlDataReader>
			{
				private MySqlDataReader mySqlDataReader;

				public QueryResultEnumerator(MySqlCommand command)
				{
					mySqlDataReader = command == null ? null : command.ExecuteReader();
				}
				public MySqlDataReader Current { get { return mySqlDataReader; } }

				public void Dispose()
				{
					if (mySqlDataReader == null) return;
					mySqlDataReader.Dispose();
					mySqlDataReader = null;
				}

				public bool MoveNext()
				{
					if (mySqlDataReader == null) return false;
					return mySqlDataReader.Read();
				}

				public void Reset() { throw new NotImplementedException(); }

				object System.Collections.IEnumerator.Current { get { return Current; } }

			}

			private IEnumerator<MySqlDataReader> enumerator;

			public IEnumerator<MySqlDataReader> GetEnumerator()
			{
				if (enumerator != null)
					enumerator.Dispose();

				enumerator = new QueryResultEnumerator(myCommand);

				return enumerator;
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void Dispose()
			{
				if (myCommand == null) return;
				if (enumerator != null)
					enumerator.Dispose();

				myCommand.Dispose();
				myCommand = null;
			}
		}
		#endregion

		readonly MySqlConnection myConnection;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="database">Имя базы в MySQL</param>
		/// <param name="host">Имя или IP-адрес сервера (если локально то можно и localhost)</param>
		/// <param name="user">Имя пользователя MySQL</param>
		/// <param name="password">пароль пользователя БД MySQL</param>
		public Db(string database, string host, string user, string password)
		{
			myConnection = new MySqlConnection(String.Format("Database={0};Data Source={1};User Id={2};Password={3}", database, host, user, password));
			myConnection.Open();
		}


		public class RealizeCollection : ObservableCollection<Realize>
		{
			public RealizeCollection(List<Realize> list)
				: base(list)
			{
			}

			protected override void SetItem(int index, Realize item)
			{
				base.SetItem(index, item);
			}

			protected override void InsertItem(int index, Realize item)
			{
				base.InsertItem(index, item);

			}

			//protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
			//{
			//	base.OnCollectionChanged(e);
			//	e.Action == 
			//}
		}
		public interface ISubmitRemove
		{
			void Submit(Db database);
			void Remove(Db database);
		}


		[Magic]
		public class Realize : NotifyPropertyChanged, ISubmitRemove
		{
			private bool Existing = false;
			public Realize()
			{
				//	throw new NotImplementedException();
			}
			public Realize(MySqlDataReader reader)
			{
				reader.Extract(this);
				Existing = true;
			}

			public string id_realize { get; set; }
			public string prep_name { get; set; }
			public double price { get; set; }
			public DateTime date_realize { get; set; }

			public void Submit(Db database)
			{
				//myCommand = new MySqlCommand(query, connection);

				//database.myConnection.
				Existing = true;
			}

			public void Remove(Db database)
			{
				if (!Existing) return;

				//remove request

				Existing = false;
			}
		}

		public class Preparat : ISubmitRemove
		{
			private bool Existing = false;

			public Preparat()
			{
			}

			public Preparat(MySqlDataReader reader)
			{
				reader.Extract(this);
				Existing = true;
			}
			public int id_preparat { get; set; }
			public string prep_name { get; set; }
			public string proizvod { get; set; }
			public string izmerenie { get; set; }
			public string prep_country { get; set; }
			public int col { get; set; }

			public void Submit(Db database)
			{
				//myCommand = new MySqlCommand(query, connection);

				//database.myConnection.
				Existing = true;
			}

			public void Remove(Db database)
			{
				if (!Existing) return;

				//remove request

				Existing = false;
			}
		}


		public class Provider : ISubmitRemove
		{
			private bool Existing = false;

			public Provider()
			{
			}

			public Provider(MySqlDataReader reader)
			{
				reader.Extract(this);
				Existing = true;
			}
			public int id_provider { get; set; }
			public string provider_name { get; set; }
			public string country { get; set; }
			public string phone { get; set; }
			public string adress { get; set; }

			public void Submit(Db database)
			{
				//myCommand = new MySqlCommand(query, connection);

				//database.myConnection.
				Existing = true;
			}

			public void Remove(Db database)
			{
				if (!Existing) return;

				//remove request

				Existing = false;
			}
		}

		public class Income : ISubmitRemove
		{
			private bool Existing = false;

			public Income()
			{
			}

			public Income(MySqlDataReader reader)
			{
				reader.Extract(this);
				Existing = true;
			}
			public int id_income { get; set; }
			public int id_preparat { get; set; }

			public int id_provider { get; set; }
			public string prep_name { get; set; }

			public string provider_name { get; set; }


			public DateTime date_inc { get; set; }
			public Double incum_price { get; set; }
			public void Submit(Db database)
			{
				//myCommand = new MySqlCommand(query, connection);

				//database.myConnection.
				Existing = true;
			}

			public void Remove(Db database)
			{
				if (!Existing) return;

				//remove request

				Existing = false;
			}
		}

		public IEnumerable GetIncome(DateTime from, DateTime to)
		{
			using (var results = Query("SELECT * FROM income "
			+ "INNER JOIN preparat on income.id_preparat=preparat.id_preparat "
			+ "INNER JOIN provider on income.id_provider=provider.id_provider  WHERE (date_inc BETWEEN " + String.Format("\"{0:yyyy/MM/dd}\" AND \"{1:yyyy/MM/dd}\"", from, to) + " )"))

				//+ " WHERE ("
				//+ " provider_name LIKE \"%" + name + "%\""
				//	))
				return new ObservableCollection<Income>(results.Select(x => new Income(x)).ToList());
		}

		public IEnumerable GetProviders(string name)
		{
			using (var results = Query("SELECT * FROM provider WHERE"
			+ " provider_name LIKE \"%" + name + "%\""
				))
				return new ObservableCollection<Provider>(results.Select(x => new Provider(x)).ToList());
		}

		public IEnumerable GetRealizes(DateTime from, DateTime to)
		{
			using (var results = Query("SELECT prep_name, price, date_realize FROM realize "
				+ "INNER JOIN preparat on realize.id_preparat=preparat.id_preparat WHERE (date_realize BETWEEN " + String.Format("\"{0:yyyy/MM/dd}\" AND \"{1:yyyy/MM/dd}\"", from, to) + " )"))
				return new RealizeCollection(results.Select(x => new Realize(x)).ToList());
		}

		public IEnumerable GetRealizes(string name, decimal priceMin, decimal priceMax, DateTime dateFrom, DateTime dateTo)
		{
			using (var results = Query("SELECT realize.id_preparat, prep_name, price, date_realize FROM realize "
			+ "INNER JOIN preparat on realize.id_preparat=preparat.id_preparat WHERE ("
			+ "date_realize BETWEEN " + String.Format("\"{0:yyyy/MM/dd}\" AND \"{1:yyyy/MM/dd}\" AND ", dateFrom, dateTo)
			+ "prep_name LIKE \"%" + name + "%\" AND "
			+ "price BETWEEN " + priceMin + " AND " + priceMax
			+ " )"))
				return new RealizeCollection(results.Select(x => new Realize(x)).ToList());
		}

		public IEnumerable<Preparat> GetPreparats(string prep_name, string proizvod, string izmerenie, string prep_country, int colfrom, int colto)
		{
			using (var results = Query("SELECT prep_name, proizvod, izmerenie, prep_country, col FROM preparat "
									   + " WHERE ("
									   + "prep_name LIKE \"%" + prep_name + "%\" AND "
									   + "proizvod LIKE \"%" + proizvod + "%\" AND "
									   + "izmerenie LIKE \"%" + izmerenie + "%\" AND "
									   + "prep_country LIKE \"%" + prep_country + "%\" AND "
									   + "col BETWEEN " + colfrom + " AND " + colto
									   + " )"))
				return new ObservableCollection<Preparat>(results.Select(x => new Preparat(x)).ToList());
		}

		public IEnumerable<Preparat> GetPreparats()
		{
			using (var results = Query("SELECT prep_name, proizvod, izmemrenie, prep_country FROM preparat"))
				return new ObservableCollection<Preparat>(results.Select(x => new Preparat(x)).ToList());
		}


		#region old
		//public class Dish
		//{
		//	public Dish(MySqlDataReader reader)
		//	{
		//		reader.Extract(this);
		//	}

		//	public int idDishes { get; set; }
		//	public string Name { get; set; }
		//	public string Cuisine { get; set; }
		//	public string Type { get; set; }
		//	public string Image { get; set; }
		//	public string Description { get; set; }

		//}

		//public class Recipe
		//{
		//	public Recipe(MySqlDataReader reader)
		//	{
		//		reader.Extract(this);
		//	}

		//	public int idRecipe { get; set; }
		//	public string Author { get; set; }
		//	public string Algorithm { get; set; }
		//	public string Image { get; set; }
		//	public int Dishes_idDishes { get; set; }

		//	public string Products { get; set; }
		//}

		//public IEnumerable<Dish> GetDishes(string name, string author, IEnumerable<string> cuisines,
		//	IEnumerable<string> products)
		//{
		//	string query = "SELECT * FROM Dishes WHERE Name LIKE \"%" + name + "%\""

		//				   + "AND Cuisine IN (\""
		//				   + String.Join("\" , \"", cuisines) + "\")"

		//				   + "AND idDishes IN ("
		//				   + "	SELECT Dishes_idDishes FROM Recipe WHERE Author LIKE \"%" + author + "%\" AND idRecipe IN ("
		//				   + "		SELECT Recipe_idRecipe FROM Recipe_has_Products WHERE Products_idProducts IN ("
		//				   + "				SELECT idProducts FROM Products WHERE Name IN (\"" +
		//										 String.Join("\" , \"", products) + "\")"
		//				   + "		)"
		//				   + "	)"
		//				   + ")";

		//	using (var results = Query(query))
		//		return results.Select(x => new Dish(x));
		//}

		//public IEnumerable<Recipe> GetRecipes(int idDishes, string author)
		//{
		//	string query = "SELECT * FROM Recipe WHERE Dishes_idDishes = " + idDishes
		//				   + " AND Author LIKE \"%" + author + "%\"";
		//	IEnumerable<Recipe> recipes;
		//	using (var results = Query(query))
		//		recipes = results.Select(x => new Recipe(x)).ToList();


		//	foreach (var recipe in recipes)
		//	{

		//		query =
		//			"SELECT * FROM Recipe_has_Products LEFT JOIN Products ON Recipe_has_Products.Products_idProducts = Products.idProducts "
		//			+ "WHERE Recipe_idRecipe = " + recipe.idRecipe;

		//		using (var results = Query(query))
		//			recipe.Products = String.Join(", ", results.Select(x => x.SafeGetString("Name")));

		//	}
		//	return recipes;
		//}

		//public IEnumerable<string> GetProducts()
		//{
		//	using (var results = Query("SELECT * FROM Products"))
		//		return results.Select(x => x.SafeGetString("Name"));
		//}

		//public IEnumerable<string> GetCuisines()
		//{
		//	using (var results = Query("SELECT * FROM Cuisines"))
		//		return results.Select(x => x.SafeGetString("Cuisine"));
		//} 
		#endregion
		public void Dispose()
		{
			myConnection.Dispose();
		}

	}
}
