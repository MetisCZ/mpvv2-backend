using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Dynamic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace mpvv2.Models.DBModels
{
    public class Database : IDisposable
    {
        private MySqlConnection connection = null;
        private string server = "localhost";
        private string database = "mpv";
        private string username = "root";
        //private string username = "mpv-admin";
        private string password = "";
        //private string password = "Jakub321654";

        public Database()
        {
            Connect();
        }

        public bool IsConnected()
        {
            if (connection == null)
                return false;
            if (connection.State != ConnectionState.Open)
                return false;
            return true;
        }

        private bool Connect()
        {
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + 
                               database + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";Connection Timeout=300;default command timeout=300;";
            
            connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Database connection error: "+e.Number+" - "+e.Message);
                return false;
            }
        }

        public void Disconnect()
        {
            connection?.Close();
        }
        
        //Insert statement
        public int Insert(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            return cmd.ExecuteNonQuery();
        }

        public int Update(string query)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = query;
            cmd.Connection = connection;
            return cmd.ExecuteNonQuery();
        }

        public int Delete(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            return cmd.ExecuteNonQuery();
        }
        
        public List<Dictionary<string, object>> Select(string query, Dictionary<string, dynamic> parameters) 
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            if(parameters != null && parameters.Count() > 0)
                foreach (var item in parameters)
                    cmd.Parameters.AddWithValue(item.Key, item.Value);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                var obj = new ExpandoObject();
                var d = obj as IDictionary<String, object>;
                for (int index = 0; index < dataReader.FieldCount; index++)
                {
                    try {
                        d[dataReader.GetName(index)] = dataReader.GetString(index);
                    } catch (SqlNullValueException) { }
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (var pair in obj)
                {
                    dic[pair.Key] = pair.Value;
                }

                list.Add(dic);
            }
            return list;
        }

        public void Dispose()
        {
            connection?.Close();
        }
    }
}