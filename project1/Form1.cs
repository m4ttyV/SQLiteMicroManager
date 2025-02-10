using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace project1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //Словари для хранения данных дублирование которых не допускается
        Dictionary<string, int> variables = new Dictionary<string, int>();
        Dictionary<string, int> units = new Dictionary<string, int>();
        Dictionary<string, int> waterObjects = new Dictionary<string, int>();
        //списки объектов классов для хранения данных
        List<main_table> main_Table = new List<main_table>();
        List<site> site = new List<site>();
        List<Settings> settings = new List<Settings>();
        int waterObjects_table_count = 0;
        int units_table_count = 0;
        int variables_table_count = 0;
        bool dbfile = false; //это надо чтобы пользователь не загружал данные до БД
        bool datafile = false; //это надо чтобы пользователь не пытался отправить пустой файл в БД
        string filePath = "";
        private void Load_data_button_Click(object sender, EventArgs e)
        {
            if (dbfile == false)
            {
                MessageBox.Show("Для работы программы необходимо загрузить файл БД.", "Призошла какая-то ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл",
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                DataProcessing(filePath);
            }
            else
            {
                MessageBox.Show("Для работы программы необходимо выбрать файл с данными.","Призошла какая-то ошибка!",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async void DataProcessing(string filePath)
        {
            
            string line_river_name = "";
            string line_chainage = "";            
            DateTime DCD = File.GetCreationTime(filePath);
            string line_item = "";
            string line_unit = "";
            List<string> lines5 = new List<string>();
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    bool data_region = false;
                    while ((line = await reader.ReadLineAsync()) != null) // разбиваем файл на блоки и запоминаем содержимое
                    {
                        line = Regex.Replace(line.Trim(), @"\s+", " "); //убериаем лишние пробелы
                        string[] words = line.Split(' ');
                        if (line.StartsWith("River Name"))
                            line_river_name = line.Trim();
                        if (line.StartsWith("Chainage"))
                        {
                            line_chainage = line.Trim();
                        }
                        if (line.StartsWith("Item"))
                        {
                            line_item = line.Trim();
                            line_item = line_item.Remove(0,5);
                        }
                        if (line.StartsWith("Unit"))
                        {
                            line_unit = line.Trim();
                        }
                        if (line.StartsWith("Date / time"))
                        {
                            data_region = true;
                            continue;
                        }
                        if (data_region && !line.StartsWith("-")) 
                            lines5.Add(line);
                    }
                    datafile = true;
                }
                int i = 0;
                //разбиваем строки на слова
                var river_names = line_river_name.Split(' ');
                var chainages = line_chainage.Split(' ');
                var items = line_item.Split(' ');
                var units_lines = line_unit.Split(' ');
                //заполняем словарь waterObjects
                foreach (string river_name in river_names)
                {
                    if (river_name == "River" || river_name == "Name") continue;
                    if (river_name.Length ==  0) continue;
                    if (waterObjects.ContainsKey(river_name)) continue;
                    i++;
                    waterObjects.Add(river_name, waterObjects.Count + 1);
                }
                //заполняем site
                for (int j = 2; j < river_names.Length - 1; j++)                
                    site.Add(new site { Id = j - 1, Chainage = Convert.ToDouble(chainages[j]), WO_id = waterObjects[river_names[j]] });
                //заполняем словарь units
                i = 0;
                foreach (string units_item in units_lines)
                {
                    if (units_item == "Unit") continue;
                    if (units.ContainsKey(units_item) && (i == 0))
                    {
                        i++;
                        continue; 
                    }
                    if (units.ContainsKey(units_item)) continue;
                    units.Add(units_item, units.Count + 1);
                    i++;
                }
                //заполняем словарь variables
                for (i = 1; i < items.Count(); i++)
                    try
                    {
                        if (!variables.ContainsKey(items[i * 2].ToString() + " " + items[i * 2 + 1].ToString() + "|" + units[units_lines[i]]))
                            variables.Add(items[i * 2].ToString() + " " + items[i * 2 + 1].ToString() + "|" + units[units_lines[i]], variables.Count + 1);
                    }
                    catch { }
                i = 0;
                //заполняем main_Table
                foreach (string line in lines5)
                {
                    if (line == "") continue;
                    var words = line.Split(' ');
                    for (int j = 2; j < words.Length; j++)                    
                        main_Table.Add(new main_table { Id = j, dtS = DCD, dtA = Convert.ToDateTime(words[0] + " " + words[1]), site_ID = j - 1, var_ID = units[units_lines[j - 1]], value = Convert.ToDouble(words[j]), setting_ID = null });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}");
            }
        }

        private void Send_data_to_database_Click(object sender, EventArgs e)
        {
            if (dbfile == false)
            {
                MessageBox.Show("Для работы программы необходимо загрузить файл БД.", "Призошла какая-то ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (datafile == false)
            {
                MessageBox.Show("Для работы программы необходимо загрузить файл с данными.", "Призошла какая-то ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DatabaseManager db = new DatabaseManager(filePath);
            db.InsertWaterObjectData(waterObjects, waterObjects_table_count);
            db.InsertUnitsData(units, units_table_count);
            db.InsertVariableData(variables, variables_table_count);
            db.InsertSiteData(site);
            db.InsertMainTableData(main_Table);
        }

        private void Load_db_file_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл",
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                dbfile = true;
                //Очищаем данные чтобы они не накладывались друг на друга после предыдущих загрузок
                waterObjects_table_count = 0;
                units_table_count = 0;
                variables_table_count = 0;
                variables.Clear();
                units.Clear();
                waterObjects.Clear();
                main_Table.Clear();
                site.Clear();
                settings.Clear();
                //Загружаем файл БД
                filePath = openFileDialog.FileName;
                DatabaseManager db = new DatabaseManager(filePath);
                //Сохраням имеющиеся данные в словари чтобы добавленные данные не конфликтовали
                variables = db.LoadVariables();
                units = db.LoadUnits();
                waterObjects = db.LoadWaterObjects();
                //Запоминаем количество добавленных данных на этом этапе
                waterObjects_table_count = waterObjects.Count;
                units_table_count = units.Count;
                variables_table_count = variables.Count;
            }
            else
            {
                dbfile = false;
                MessageBox.Show("Для работы программы необходимо выбрать файл с данными.", "Призошла какая-то ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
