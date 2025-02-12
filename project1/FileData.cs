using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.Data.Sqlite;

namespace project1
{

    
    #region Классы по таблицам
    //id, name
    //int, str
    public class Water_Object
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
    }
    // id, chainage, river_id
    // int, double, int
    public class site
    {
        public int Id { get; set; }
        public double Chainage { get; set; }
        public int WO_id { get; set; }
    }
    // id, Name, UnitID
    // int, str, int
    public class variable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UnitID { get; set; }
    }
    // id, unit_Name
    // int, str
    public class unit
    {
        public int Id { get; set; }
        public string unit_Name { get; set; }
    }
    // id, dtS, dtA, site_ID, var_ID, value, setting_ID
    // int, datetime, datetime, int, int, double, int
    public class main_table
    {
        public int Id { get; set; }
        public DateTime dtS { get; set; }
        public DateTime dtA { get; set; }
        public int site_ID { get; set; }
        public int var_ID { get; set; }
        public double value { get; set; }
        public int? setting_ID { get; set; }
    }
    // id, name, content
    // int, str, str
    public class Settings
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }
    #endregion
    
    public class DatabaseManager
    {
        private readonly string _connectionString;
        // Для удобства вынес названия таблиц в переменные чтобы проще было менять (название колонок можно также вынести, но мне было лень)
        public readonly string water_object_table_name = "Water_object";
        private readonly string site_table_name = "Site";
        private readonly string variables_table_name = "variable";
        private readonly string units_table_name = "unit";
        private readonly string settings_table_name = "settings";
        private readonly string main_table_table_name = "main_table";

        public DatabaseManager(string databasePath)
        {
            _connectionString = $"Data Source={databasePath};";
            
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                
                using (var cmd = new SqliteCommand("", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        // Загрузка данных из таблиц в словари
        public Dictionary<string, int> LoadVariables()
        {
            var variables = new Dictionary<string, int>();

            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Name, UnitID FROM " + variables_table_name;  
                using (var cmd = new SqliteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);   // Чтение id
                            string name = reader.GetString(1); // Чтение name
                            string UnitID = reader.GetString(2); // Чтение name
                            variables[name + "|" + UnitID] = id; // Добавление в словарь
                        }
                    }
                }
            }

            return variables;
        }
        public Dictionary<string, int> LoadUnits()
        {
            var units = new Dictionary<string, int>();

            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, unit_Name FROM " + units_table_name; 
                using (var cmd = new SqliteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);   // Чтение id
                            string name = reader.GetString(1); // Чтение name
                            units[name] = id; // Добавление в словарь
                        }
                    }
                }
            }

            return units;
        }
        public Dictionary<string, int> LoadWaterObjects()
        {
            var waterObjects = new Dictionary<string, int>();

            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Name FROM " + water_object_table_name; 
                using (var cmd = new SqliteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);   // Чтение id
                            string name = reader.GetString(1); // Чтение name
                            waterObjects[name] = id; // Добавление в словарь
                        }
                    }
                }
            }

            return waterObjects;
        }

        public void InsertMainTableData(main_table data)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                string query = $@"
                INSERT INTO {main_table_table_name} (dtS, dtA, site_ID, var_ID, value, setting_ID)
                VALUES (@dtS, @dtA, @site_ID, @var_ID, @value, @setting_ID)";
                using (var cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@dtS", data.dtS.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@dtA", data.dtA.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@site_ID", data.site_ID);
                    cmd.Parameters.AddWithValue("@var_ID", data.var_ID);
                    cmd.Parameters.AddWithValue("@value", data.value);
                    cmd.Parameters.AddWithValue("@setting_ID", data.setting_ID.HasValue ? (object)data.setting_ID.Value : DBNull.Value);
                    cmd.ExecuteNonQuery();                    
                }
            }
        }
        //site maintable
        public int get_last_index(string table_name)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                bool check = false;
                int id = 0;
                conn.Open();
                string query = "SELECT MAX(Id) FROM " + table_name; 
                using (var cmd = new SqliteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            check = reader.IsDBNull(0);
                            if  (!check)
                                id = reader.GetInt32(0);   // Чтение id      
                        }                     
                return id;
            }
        }
        public void InsertWaterObjectData(string data) //Здесь, в Variable и в Units целочисленная переменная нужна чтобы
                                                                                    //сделать поправку на уже имеющиеся данные чтобы избежать их дублирования
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                string query = $@"
                INSERT INTO {water_object_table_name} (Name)
                VALUES (@Name)";
                using (var cmd = new SqliteCommand(query, conn))
                {                    
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Name", data);
                    cmd.ExecuteNonQuery();                    
                }
            }
        }
        public void InsertVariableData(string data)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                string query = $@"
                INSERT INTO {variables_table_name} (Name, UnitID)
                VALUES (@Name, @UnitID)";

                using (var cmd = new SqliteCommand(query, conn))
                {
                    string name = data.Split('|')[0];
                    string UnitID = data.Split('|')[1];
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@UnitID", UnitID);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.ExecuteNonQuery();                    
                }
            }
        }
        public void InsertUnitsData(string data)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                string query = $@"
                INSERT INTO {units_table_name} (unit_Name)
                VALUES (@unit_Name)";

                using (var cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@unit_Name", data);
                    cmd.ExecuteNonQuery();
                    
                }
            }
        }
        public void InsertSiteData(site data)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                conn.Open();
                string query = $@"
                    INSERT INTO {site_table_name} (Chainage, WO_id)
                    VALUES (@Chainage, @WO_id)";

                using (var cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Chainage", data.Chainage);
                    cmd.Parameters.AddWithValue("@WO_id", data.WO_id);
                    cmd.ExecuteNonQuery();                    
                }
            }
        }
    }
}
