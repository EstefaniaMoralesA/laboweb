using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ABC
{
    public class ReadFromFile
    {

        private static Dictionary<string, Table> tables;

        static JObject parseXML(string path)
        {
            XmlDocument doc = new XmlDocument();
            if(!File.Exists(path))
            {
                throw new ArgumentException("El archivo " + path + " no fue encontrado");
            }
            doc.Load(path);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            return JObject.Parse(jsonText)["Abc"].Value<JObject>();
        }

        static string getPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory.ToString(), "config.xml");
        }

        static void InitForeignKeys()
        {
            Table[] tableArray = new Table[tables.Values.Count];
            tables.Values.CopyTo(tableArray, 0);
            foreach(Table table in tableArray)
            {
                Column[] array = new Column[table.Columns.Values.Count];
                table.Columns.Values.CopyTo(array, 0);
                foreach (var column in array)
                {
                    if (column.IsForeignKey)
                    {
                        Table toTable = tables[column.ForeignKeyMap.ToTable.Name];
                        ForeignKey key = tables[table.Name].Columns[column.Name].ForeignKeyMap;
                        tables[table.Name].Columns[column.Name].ForeignKeyMap = new ForeignKey
                        {
                            ToTable = toTable,
                            Column = key.Column ?? toTable.PrimaryKey.Name
                        };
                    }
                }
            }
        }

        static void Run()
        {
            JObject json = parseXML(getPath());
            InitAppConfig(json["Configurations"].Value<JObject>());
            InitDatabaseConnection(json["ConnectionProperties"].Value<JObject>());
           
            tables = SharedContainer.DbInstance.GetTables();
            readTablesFromXML(json); 
            InitForeignKeys();
            
            foreach(Table table in tables.Values)
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(table.FullDirectoryPath, $"{table.Name}.php")))
                {
                    ReadTableVisitor visitor = new ReadTableVisitor(sw, tables.Values);
                    table.Accept(visitor);
                }
                if(table.ShowNew)
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(table.FullDirectoryPath, $"add{table.Name}.php")))
                    {
                        AddVisitor visitor = new AddVisitor(sw, table);
                        table.Accept(visitor);
                    }
                }
                if(table.ShowEdit)
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(table.FullDirectoryPath, $"edit{table.Name}.php")))
                    {
                        EditVisitor visitor = new EditVisitor(sw, table);
                        table.Accept(visitor);
                    }
                }
                
            }

            SharedContainer.DbInstance.CreateDatabaseConnectionFile();
            SharedContainer.AppConfigInstance.CreateConfigFile();
            createHeadersFile();
            createFooterFile();
        }

        static void parseOption(JToken option, ref Table table)
        {
            string nameField = option["@Campo"].Value<string>();
            string value = option["#text"].Value<string>();
            string[] options = value.Split(',');
            foreach(var o in options)
            {
                string[] vals = o.Split('-');
                table.Columns[nameField].Options.Add(vals[0], vals[1]);
            }
        }

        static void parseForeignKey(string[] vals, ref Table table)
        {
            string nameField = vals[0];
            string value = vals[1];
            if (!table.Columns[nameField].IsForeignKey)
            {
                Console.WriteLine($"Error: column: {nameField} in table: {table.Name} is not a foreign key");
                Environment.Exit(1);
            }
            table.Columns[nameField].IsForeignKeyMapped = true;
            table.Columns[nameField].ForeignKeyMap = new ForeignKey
            {
                ToTable = tables[table.Columns[nameField].ForeignKeyMap.ToTable.Name],
                Column = value
            };

        }

        static void parseTable(JToken table)
        {
            string nameTable = table["@Name"].Value<string>();
            Table currentTable;

            if(!tables.TryGetValue(nameTable, out currentTable))
            {
                Console.WriteLine($"ERROR -- Table {nameTable} in config file doesn't exist in database.");
                Environment.Exit(1);
            }

            if (table["Disable"] != null)
            {
                string value = table["Disable"].Value<string>();
                string[] columns = value.Split(',');
                foreach (var column in columns)
                {
                    currentTable.Columns[column].ReadOnly = true;
                }
            }

            if (table["Hide"] != null)
            {
                string value = table["Hide"].Value<string>();
                string[] columns = value.Split(',');
                foreach (var column in columns)
                {
                    currentTable.Columns[column].Hide = true;
                }
            }

            if (table["HideReg"] != null)
            {
                string value = table["HideReg"].Value<string>();
                string[] regs = value.Split(',');
                foreach (var reg in regs)
                {
                    currentTable.HideReg.Add(reg);
                }
            }

            if (table["ShowId"] != null)
            {
                bool value = table["ShowId"].Value<bool>();
                currentTable.ShowId = value;
            }

            if (table["ShowNew"] != null)
            {
                bool value = table["ShowNew"].Value<bool>();
                currentTable.ShowNew = value;
            }

            if (table["ShowEdit"] != null)
            {
                bool value = table["ShowEdit"].Value<bool>();
                currentTable.ShowEdit = value;
            }

            if (table["ShowDelete"] != null)
            {
                bool value = table["ShowDelete"].Value<bool>();
                currentTable.ShowDelete = value;
            }

            if (table["Headers"] != null)
            {
                string value = table["Headers"].Value<string>();
                string[] columns = value.Split(',');
                foreach (var column in columns)
                {
                    string[] vals = column.Split('-');
                    currentTable.Columns[vals[0]].Header = vals[1];
                }
            }
            
            if (table["Options"] != null)
            {
                string optionsArrayString = table["Options"]?.ToString();
                if(optionsArrayString != null)
                {
                    if(optionsArrayString[0] == '[')
                    {
                        JArray optionsArray = JArray.Parse(optionsArrayString);
                        foreach(JToken option in optionsArray)
                        {
                            parseOption(option, ref currentTable);   
                        }
                    }
                    else 
                    {
                        parseOption(JToken.Parse(optionsArrayString), ref currentTable);
                    }
                }
            }

            if (table["ForeignKeys"] != null)
            {
                string value = table["ForeignKeys"].Value<string>();
                string[] columns = value.Split(',');
                foreach (var column in columns)
                {
                    parseForeignKey(column.Split('-'), ref currentTable);
                }
            }
        }

        static void readTablesFromXML(JObject json)
        {
            string tableArrayString = json["Table"]?.ToString();
            if(tableArrayString != null)
            {
                if(tableArrayString[0] == '[')
                {
                    JArray tableArray = JArray.Parse(tableArrayString);
                    foreach(JToken table in tableArray)
                    {
                        parseTable(table);   
                    }
                }
                else 
                {
                    parseTable(JToken.Parse(tableArrayString));
                }
            }
        }

        static void createFooterFile()
        {
            using (StreamWriter sw = new StreamWriter(SharedContainer.FooterPath))
            {
                sw.WriteLine("</div>");
                sw.WriteLine("</div>");
                sw.WriteLine("<script src='https://ajax.googleapis.com/ajax/libs/jquery/3.2.0/jquery.min.js'></script>");
                sw.WriteLine("<script src='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js'></script>");
                sw.WriteLine("</body>");
                sw.WriteLine("</html>");
            }
        }

        static void createHeadersFile()
        {
            using (StreamWriter sw = new StreamWriter(SharedContainer.HeaderPath))
            {
                sw.WriteLine("<?php");
                sw.WriteLine($"include_once(\"../{SharedContainer.AppConfigInstance.ConfigDirectory}/config.php\");");
                sw.WriteLine("?>");
                sw.WriteLine("<html>");
                sw.WriteLine("<head>");
                sw.WriteLine("<meta http-equiv='content-type' content='text/html; charset=UTF-8'/>");
                sw.WriteLine("<meta name='viewport' content='initial-scale=1.0, maximum-scale=1.0, width=device-width'>");
                sw.WriteLine("<link href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css' rel='stylesheet'>");
                sw.WriteLine("</head>");
                sw.WriteLine("<body>");
                sw.WriteLine("<div class='container'>");
                sw.WriteLine("<div class='col-sm-12'>");
            }
        }

        static void InitAppConfig(JObject configurations)
        {
            string configPath = configurations["ConfigPath"].Value<string>();
            JObject salt = configurations["Salt"].Value<JObject>();
            string login = salt["Login"].Value<string>();
            string request = salt["Request"].Value<string>();
            SharedContainer.AppConfigInstance = new AppConfig(login, request, configPath);
        }

        static void InitDatabaseConnection(JObject connectionProperties)
        {
            string dbName = connectionProperties["DatabaseName"].Value<string>();
            string userName = connectionProperties["UserName"].Value<string>();
            string server = connectionProperties["Server"].Value<string>();
            string password = connectionProperties["Password"].Value<string>();
            SharedContainer.DbInstance = new DatabaseConnection(dbName, userName, server, password);
        }
        
        static void Main(string[] args)
        {
           Run();
        }
    }
}
