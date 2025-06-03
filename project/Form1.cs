using lab4;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;

namespace lab4
{
    public partial class Form1 : Form
    {
        private sintaxLR sinLR;
        public Form1()
        {
            InitializeComponent();
        }

        private void Analyz()
        {
            //Очистка перед добавлением новых данных
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox4.Items.Clear();

            string text = textBox1.Text;
            var tokens = analiz.CheckString(text, listBox2);

            listBox1.Items.Add($"Индекс:      Лексема:           Классификация: ");
            int j = 0;
            for (int i = 0; i < tokens.Count; ++i)
            {
                listBox1.Items.Add($"{j}                       {tokens[i].Value}                         {tokens[i].Type}, {tokens[i].Number}");
                j++;
            }

            listBox3.Items.Add($"Операция:          Операнд 1:          Операнд 2:          Результат:");

            sinLR = new sintaxLR(tokens, listBox2, listBox2, listBox3, listBox4);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Analyz();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Открытие диалога выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выберите файл для анализа";
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK) // Если файл выбран
            {
                try
                {
                    // Чтение всего текста из файла
                    string text = File.ReadAllText(openFileDialog.FileName);

                    // Отображение содержимого файла в textBox1
                    textBox1.Text = text;


                    // Выполняем лексический анализ после загрузки файла
                    Analyz();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке файла: " + ex.Message,
                                  "Ошибка", MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}