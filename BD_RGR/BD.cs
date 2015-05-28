using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace shitproject
{
	public class Db : IDisposable
	{
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
			string CommandText = "Наш SQL скрипт";
			myConnection = new MySqlConnection(String.Format("Database={0};Data Source={1};User Id={2};Password={3}", database, host, user, password));
		

			//myConnection.Open();
			//MySqlCommand myCommand = new MySqlCommand("SELECT * FROM Meals", myConnection);
			//MySqlDataReader myDataReader = myCommand.ExecuteReader();

			//while (myDataReader.Read())
			//{
			//	string Name = myDataReader.GetString(1); //Получаем строку
			//	string Kitchen = myDataReader.GetString(2); //Получаем строку
			//	string Type = myDataReader.GetString(3); //Получаем строку
			//	//string Image = myDataReader.GetString(4); //Получаем строку
			//	//string Description = myDataReader.GetString(4); //Получаем строку

			//	//int id = myDataReader.GetInt32(1); //Получаем целое число
			//}

		}

		public void Dispose()
		{
			myConnection.Dispose();
		}
	}
}
