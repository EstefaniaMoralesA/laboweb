using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;

namespace ABC
{
    public class DatabaseConnection
    {
        private MySqlConnection conn;
        public string DatabaseConfigFile { get; private set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Server { get; set; }
        public string Password { get; set; }

        public string ConfigFilePath => Path.Combine(SharedContainer.AppConfigInstance.FullDirectoryPath,DatabaseConfigFile);

        public DatabaseConnection(string databaseName, string userName, string server, string password)
        {
            conn = new MySqlConnection();
            DatabaseConfigFile = "database.php";
            DatabaseName = databaseName;
            UserName = userName;
            Server = server;
            Password = password;
            conn.ConnectionString = $"SERVER={server};DATABASE={databaseName};UID={userName};PASSWORD={password}";
        }

        public Dictionary<string, Table> GetTables()
        {
            Dictionary<string, Table> tables = new Dictionary<string, Table>();
            try
            {
                conn.Open();
                Console.WriteLine(conn.State);
                DataTable dt = conn.GetSchema("Tables");
                foreach(DataRow row in dt.Rows)
                {
                    tables.Add(row[2].ToString(), new Table(row[2].ToString()));
                }
                conn.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            return tables;
        }

        public Dictionary<string, Column> GetColumns(string tableName)
        {
            Dictionary<string, Column> columns = new Dictionary<string, Column>();
            try
            {
                conn.Open();
                Console.WriteLine(conn.State);
                string query = $"SELECT COLUMN_NAME, DATA_TYPE, TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = '{tableName}' AND table_schema = '{DatabaseName}'";
                MySqlCommand cmd= new MySqlCommand(query ,conn);
                MySqlDataReader sdr = cmd.ExecuteReader();
                Console.WriteLine(tableName);
                while(sdr.Read())
                {
                    columns.Add(sdr[0].ToString(), new Column(sdr[0].ToString(), sdr[1].ToString()));
                }

                conn.Close();

                conn.Open();
                string query2 = $"SELECT k.column_name, t.constraint_type, k.referenced_table_name FROM information_schema.table_constraints t JOIN information_schema.key_column_usage k USING(constraint_name,table_schema,table_name) WHERE (t.constraint_type='PRIMARY KEY' OR t.constraint_type='FOREIGN KEY') AND t.table_schema='{DatabaseName}' AND t.table_name='{tableName}';";
                cmd = new MySqlCommand(query2 ,conn);
                sdr = cmd.ExecuteReader();
                while(sdr.Read())
                {
                    string columnName = sdr[0].ToString();
                    if(sdr[1].ToString().Equals("PRIMARY KEY"))
                    {
                        columns[columnName].IsPrimaryKey = true;
                    }
                    else
                    {
                        columns[columnName].IsForeignKey = true;
                        columns[columnName].ForeignKeyMap = new ForeignKey { ToTable = new Table(sdr[2].ToString()), Column = null};
                    }
                }
                conn.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            return columns;
        }

        public void CreateDatabaseConnectionFile()
        {
            StreamWriter sw = new StreamWriter(ConfigFilePath);
            string text = File.ReadAllText(Path.Combine(SharedContainer.AppDirectory, "Database.php"));
            text = text.Replace("@dbname", DatabaseName);
            text = text.Replace("@dbuser", UserName);
            text = text.Replace("@dbpassword", Password);
            text = text.Replace("@dbserver", Server);
            sw.WriteLine(text);
            sw.Close();
        }
    }
}
