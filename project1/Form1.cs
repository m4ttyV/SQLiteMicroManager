using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace project1
{
    public partial class Form1 : Form
    {       
        #region Переменные, массивы и прочее
        //Словари для хранения данных дублирование которых не допускается
        Dictionary<string, int> variables = new Dictionary<string, int>();
        Dictionary<string, int> units = new Dictionary<string, int>();
        Dictionary<string, int> waterObjects = new Dictionary<string, int>();
        Dictionary<string, int> site_dict = new Dictionary<string, int>();
        //списки объектов классов для хранения данных
        List<main_table> main_Table = new List<main_table>();
        List<Settings> settings = new List<Settings>();
        bool dbfile = false; //это надо чтобы пользователь не загружал данные до БД
        string filePath = "";
        string database_filepath = "";
        //названия иаблиц
        public readonly string water_object_table_name = "Water_object";
        private readonly string site_table_name = "Site";
        private readonly string variables_table_name = "variable";
        private readonly string units_table_name = "unit";
        private readonly string settings_table_name = "settings";
        private readonly string main_table_table_name = "Value_data";
        #endregion

        #region Кнопки и загрузка формы
        private void Load_data_button_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл",
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                DataProcessing(filePath, database_filepath);
            }
            else
            {
                MessageBox.Show("Для работы программы необходимо выбрать файл с данными.","Призошла какая-то ошибка!",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Load_db_file_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл Базы Данных",
                Filter = "Все файлы (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                dbfile = true;
                //Очищаем данные чтобы они не накладывались друг на друга после предыдущих загрузок
                variables.Clear();
                units.Clear();
                waterObjects.Clear();
                main_Table.Clear();
                site_dict.Clear();
                settings.Clear();
                //Загружаем файл БД
                database_filepath = openFileDialog.FileName;
                DatabaseManager db = new DatabaseManager(database_filepath);
                //Сохраням имеющиеся данные в словари чтобы добавленные данные не конфликтовали
                variables = db.LoadVariables();
                units = db.LoadUnits();
                waterObjects = db.LoadWaterObjects();
                site_dict = db.LoadSites();

                //Запоминаем количество добавленных данных на этом этапе
                Load_data_button.Enabled = true;
            }
            else
            {
                dbfile = false;
                MessageBox.Show("Для работы программы необходимо выбрать файл с данными.", "Призошла какая-то ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Load_data_button.Enabled = false;
        }
        
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region Прочие функции
        private string[] line_splitter(string line)
        {
            string[] words = new string[(line.Length - 21) / 20];
            int wordcount = 0;
            for (int ii = 21; ii <= line.Length - 20; ii += 20)
            {
                if (line.Substring(ii, 20).Trim() == null) continue;
                //if (wordcount == 43) {
                //    Console.Write(1);
                //}                
                words[wordcount++] = line.Substring(ii, 20).Trim();
            }
            return words;
        }

        private async void DataProcessing(string filePath, string database_filepath)
        {
            DateTime DCD = File.GetCreationTime(filePath); //  будет сохраняться как dts - дата старта расчета
            DatabaseManager db = new DatabaseManager(database_filepath);
            string line_river_name = "";
            string line_chainage = "";            
            string line_item = "";
            string line_unit = "";
            List<string> dataLines = new List<string>();
            //try
            //{
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    bool data_region = false;
                    while ((line = await reader.ReadLineAsync()) != null) // разбиваем файл на блоки и запоминаем содержимое
                    {
                        if (line.Length == 0 || line.StartsWith("Static")) continue;
                        if (line.Trim().StartsWith("River Name"))
                        {
                            line_river_name = line;
                        }
                        if (line.Trim().StartsWith("Chainage"))
                        {
                            line_chainage = line;
                        }
                        if (line.Trim().StartsWith("Item"))
                        {
                            line_item = line;
                        }
                        if (line.Trim().StartsWith("Unit"))
                        {
                            line_unit = line;
                        }
                        if (line.Trim().StartsWith("Date / time"))
                        {
                            data_region = true;
                            continue;
                        }
                        if (data_region && !line.StartsWith("-"))
                            dataLines.Add(Regex.Replace(line.Trim(), @"\s+", " "));                        
                    }
                }
                //разбиваем строки на слова
                var river_names = line_splitter(line_river_name);
                
                var chainages = line_splitter(line_chainage);

                var items = line_splitter(line_item);

                var units_lines = line_splitter(line_unit);

                //заполняем словарь waterObjects
                foreach (string river_name in river_names)
                {
                    if (river_name == null) continue;
                    if (river_name == "River Name") continue;
                    if (river_name.Length ==  0) continue;
                    if (waterObjects.ContainsKey(river_name)) continue;
                    int id = db.InsertWaterObjectData(river_name);
                    waterObjects.Add(river_name, id);
                }
                //заполняем словарь units
                foreach (string units_item in units_lines)
                {
                    if (units_item == null) continue;
                    if (units_item == "Unit") continue;
                    if (units.ContainsKey(units_item)) continue;
                    int id = db.InsertUnitsData(units_item);
                    units.Add(units_item, id);
                }
                //заполняем словарь variables
                for (int i = 0; i < items.Count(); i++)
                {
                    if (items[i] == null) continue;
                    if (!variables.ContainsKey(items[i].ToString() + "|" + units[units_lines[i]]))
                    { 
                        int id = db.InsertVariableData(items[i].ToString() + "|" + units[units_lines[i]]);
                        variables.Add(items[i].ToString() + "|" + units[units_lines[i]], id);
                    }
                }
                //заполняем site
                for (int j = 0; j < river_names.Length; j++)
                {
                    if (site_dict.ContainsKey(waterObjects[river_names[j]].ToString() + "|" + chainages[j])) continue;                    
                    int id = db.InsertSiteData(chainages[j], waterObjects[river_names[j]].ToString());
                    site_dict.Add(waterObjects[river_names[j]].ToString() + "|" + chainages[j], id);
                }

            string decSep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            //CultureInfo.CurrentCulture.
            //заполняем main_Table
            foreach (string line in dataLines)
            {
                if (line == "") continue;
                var words = line.Split(' ');
                for (int j = 2; j < words.Length; j++)
                {
                    string key = waterObjects[river_names[j - 2]] + "|" + chainages[j - 2];
                    int temp = site_dict[key];
                    main_table data = new main_table
                    {
                        Id = -1, //далее нигде не используется (рудимент предыдущих версий кода, можно избавится если передавать знчения напрямую)
                        dtS = DCD,
                        dtA = Convert.ToDateTime(words[0] + " " + words[1]),
                        site_ID = site_dict[waterObjects[river_names[j - 2]] + "|" + chainages[j - 2]],
                        var_ID = units[units_lines[j - 2]],
                        value = Convert.ToDouble(
                            decSep == "." ? words[j].Replace(",", ".") : words[j].Replace(".", ",")
                            ),
                        setting_ID = null
                    };
                    int id = db.InsertMainTableData(data);
                    data.Id = id;
                    main_Table.Add(data);
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка при чтении файла: {ex.Message}");
            //}
        }
        #endregion
    }
}
