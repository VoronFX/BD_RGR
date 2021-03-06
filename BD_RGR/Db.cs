﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace shitproject
{
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
			else return false;
			return true;
		}
	}

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

		public class Resourse
		{
			public Resourse(MySqlDataReader reader)
			{
				reader.Extract(this);
			}

			public int idPResourse { get; set; }
			public string Name { get; set; }
            public string Creator { get; set; }
			public string Link { get; set; }
			public string Type { get; set; }
			public string Image { get; set; }
			public string Description { get; set; }
            public DateTime Date { get; set; }

		}

		public class Recipe
        {
            public Recipe(MySqlDataReader reader)
            {
                reader.Extract(this);
            }

            public int idRecipe { get; set; }
            public string Author { get; set; }
            public string Algorithm { get; set; }
            public string Image { get; set; }
            public int Dishes_idDishes { get; set; }

            public string Products { get; set; }
		}

		public IEnumerable<Resourse> GetResourses(string name, string author, IEnumerable<string> entities,
			IEnumerable<string> type, DateTime from, DateTime to)
		{
			string query = "SELECT * FROM PleasureResourse WHERE Name LIKE \"%" + name + "%\""

                           + "AND Creator LIKE \"%" + author + "%\" "
						   + "AND Type IN (\""
						   + String.Join("\" , \"", type) + "\")"

						   + "AND idPResourse IN ("
                           + "	SELECT PleasureResourse_idPResourse FROM PleasureResourse_has_Entities WHERE Entities_idEntities IN ("
						   + "		SELECT idEntities FROM Entities WHERE Name IN (\"" +
												 String.Join("\" , \"", entities) + "\")"
						   + "		)"
						   + "	)";

			using (var results = Query(query))
				return results.Select(x => new Db.Resourse(x));
		}

		public IEnumerable<Recipe> GetRecipes(int idDishes, string author)
		{
            string query = "SELECT * FROM Recipe WHERE Dishes_idDishes = " + idDishes
                           + " AND Author LIKE \"%" + author + "%\"";
            IEnumerable<Recipe> recipes;
            using (var results = Query(query))
                recipes = results.Select(x => new Recipe(x)).ToList();


            foreach (var recipe in recipes)
            {

                query =
                    "SELECT * FROM Recipe_has_Products LEFT JOIN Products ON Recipe_has_Products.Products_idProducts = Products.idProducts "
                    + "WHERE Recipe_idRecipe = " + recipe.idRecipe;

                using (var results = Query(query))
                    recipe.Products = String.Join(", ", results.Select(x => x.SafeGetString("Name")));

            }
            return recipes;
		}

		public IEnumerable<string> GetTypes()
		{
			using (var results = Query("SELECT * FROM ResourseType"))
				return results.Select(x => x.SafeGetString("Type"));
		}

		public IEnumerable<string> GetEntities()
		{
			using (var results = Query("SELECT * FROM Entities"))
				return results.Select(x => x.SafeGetString("Name"));
		}

		public void Dispose()
		{
			myConnection.Dispose();
		}
	}
}
