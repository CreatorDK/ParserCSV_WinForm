using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NotVisualBasic.FileIO;
using System.IO;

namespace ParserCSV_WinForm
{
    public partial class Form1 : Form
    {
        public int lines = 0;
        public int line = 0;

        public static int region_id;
        public static int area_id;
        public static int locality_id;
        public static int index_id;
        public static int street_id;
        public static int building_id;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox_connection_string.Text = "Data Source=ADMINDK-PC\\MSSQLSERVER_DK;Initial Catalog=Ukraine_address;Integrated Security=True";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Выберите CSV-файл",
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "CSV files (*.csv)|*.csv"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox_file.Text = ofd.FileName;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            SetFormLoad();

            Thread a = new Thread(Parse);
            a.Start();
        }

        private void Parse()
        {
            //Проверяем существует ли файл
            if (!File.Exists(textBox_file.Text))
            {
                //Возвращаем ошибку если файл не найден
                MessageBox.Show("Выбраный файл не существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Узнаём количество устрок в файле
            lines = System.IO.File.ReadAllLines(textBox_file.Text).Length;

            //Создаем екземпляр класса CsvTextFieldParser
            CsvTextFieldParser csv = new CsvTextFieldParser(textBox_file.Text, Encoding.GetEncoding(1251));
            //Указываем знак разделителя
            csv.SetDelimiter(';');

            //Указываем разделять ли строку по пустому простанству
            csv.TrimWhiteSpace = false;

            SQLClass.Inntitialize(textBox_connection_string.Text);

            try
            {
                SQLClass.TestSql();
            }
            catch(SqlException ex)
            {
                MessageBox.Show("Нувозможно подключиться к указанной базе данных. Подробнее:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetForm();
                return;
            }
            
            SQLClass.Connect();

            bool firstline = false;

            while (!csv.EndOfData)
            {
                string[] fields = csv.ReadFields();

                line++;

                //Заполняем прогрес-бар
                if (progressBar1.InvokeRequired)
                {
                    progressBar1.Invoke(new Action(() => { progressBar1.Value = (line / lines) * 100; }));
                }
                else
                {
                    progressBar1.Value = (line / lines) * 100;
                }

                if (status_label.InvokeRequired)
                {
                    status_label.Invoke(new Action(() => { status_label.Text = "Строка: " + line + "/" + lines; }));
                }

                if (!firstline)
                {
                    firstline = true;
                    continue;
                }

                for (int i = 0; i <= fields.Length - 1; i++)
                {
                    switch (i)
                    {
                        case 0:
                            try
                            {
                                region_id = SQLClass.InsertRegion(fields[i]);
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("Невозможно добавить запись в таблицу Областей. Подробнее:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        case 1:
                            try
                            {
                                area_id = SQLClass.InsertArea(fields[i], region_id);
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("Невозможно добавить запись в таблицу Районов. Подробнее:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        case 2:
                            try
                            {
                                locality_id = SQLClass.InsertLocality(fields[i], region_id, area_id);
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("Невозможно добавить запись в таблицу Населенных Пунктов. Подробнее:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        case 3:
                            try
                            {
                                index_id = SQLClass.InsertIndex(fields[i]);
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("Невозможно добавить запись в таблицу Почтовых Индексов. Подробнее:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        case 4:
                            try
                            {
                                street_id = SQLClass.InsertStreet(fields[i], locality_id, index_id);
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("Невозможно добавить запись в таблицу Улиц. Подробнее:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        case 5:
                            string[] streets_list = fields[i].Split(',');
                            foreach(string street in streets_list)
                            {
                                try
                                {
                                    building_id = SQLClass.InsertBulding(street, street_id);
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("Невозможно добавить запись в таблицу Номера дома. Подробнее:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                
                            }
                            break;
                    }
                }
            }
            SQLClass.Disconnect();
            ResetForm();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SetFormLoad()
        {
            textBox_connection_string.Enabled = false;
            textBox_file.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
        }

        private void ResetForm()
        {
            textBox_connection_string.Enabled = true;
            textBox_file.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            SQLClass.Inntitialize(textBox_connection_string.Text);
            try
            {
                SQLClass.Connect();
                MessageBox.Show("Соединение выполнено успешно!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Невозможно подключиться к БД. Подробнее:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            SQLClass.Disconnect();
        }

        private void TextBox_file_TextChanged(object sender, EventArgs e)
        {
            if (textBox_file.Text.Length > 0)
            {
                button2.Enabled = true;
            }
        }
    }
}
