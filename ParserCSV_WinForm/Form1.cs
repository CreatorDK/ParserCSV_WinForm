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
        public static Form1 form;
        public Form1()
        {
            InitializeComponent();
            form = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox_connection_string.Text = "Data Source=ADMINDK-PC\\MSSQLSERVER_DK;Initial Catalog=Ukraine_address;Integrated Security=True";
            comboBox1.Text = "1";
        }

        //Нажатие на кнопку Обзор (для выбора расположения CSV-файла)
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

        //Нажатие на копку Go
        private void Button2_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            SetFormLoad();
            Parse();
            ResetForm();
            timer1.Interval = 5000;
            timer1.Start();
        }

        private void Parse()
        {
            int number_of_threads = Convert.ToInt32(comboBox1.Text);
            string[] temp;

            //Создаём безразмерный массив для строк
         
            //Проверяем существует ли файл
            if (!File.Exists(textBox_file.Text))
            {
                //Возвращаем ошибку если файл не найден
                MessageBox.Show("Выбраный файл не существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Считываем все строки в массив
            temp = File.ReadAllLines(textBox_file.Text, Encoding.GetEncoding(1251));

            StaticClass.work_lines = new string[temp.Length];

            for (int i = 1; i <= StaticClass.work_lines.Length-1; i++)
            {
                StaticClass.work_lines[i] = temp[i];
            }

            //Узнаём количество устрок в файле
            StaticClass.total_lines = StaticClass.work_lines.Length-1;

            var classes = new List<ThreadClass>();

            for (int i = 1; i <= number_of_threads; i++)
            {
                classes.Add(new ThreadClass(i, number_of_threads, StaticClass.total_lines));
            }

            classes.ForEach(t => t.Inntitialize(textBox_connection_string.Text));
            classes.ForEach(t => t.Connect());

            var threads = new List<Thread>();

            for (int i = 1; i <= number_of_threads; i++)
            {
                threads.Add(new Thread(new ThreadStart(classes[i-1].Parse)));
            }

            threads.ForEach(t => t.Start());
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
            timer1.Stop();

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

        }

        private void TextBox_file_TextChanged(object sender, EventArgs e)
        {
            if (textBox_file.Text.Length > 0)
            {
                button2.Enabled = true;
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void ShowErrorMessage(string text, string title)
        {
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            label_speed.Text = "Скорость: " + ((StaticClass.current_line - StaticClass.current_line_buf) * 12).ToString() + " с/м";
            StaticClass.current_line_buf = StaticClass.current_line;
        }
    }
}
