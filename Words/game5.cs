using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Words
{
    public partial class game5 : Form
    {
        private SqlConnection sqlConnection = null;
        private List<string> wordList = new List<string>(); // Список слів для гри
        private Random random = new Random(); // Для вибору випадкового слова комп'ютером                                   
        private List<string> enteredWords = new List<string>(); // Глобальна змінна для зберігання введених слів
        private List<string> computerWords = new List<string>(); // Глобальна змінна для зберігання введених слів комп'ютером
        public game5()
        {
            InitializeComponent();
        }

        private void game5_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Перший хід за Вами! Напишіть слово, яке підходить до обраної тематики з маленької літери :)");
            // Підключення до БД WorrdList
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["WordListDB"].ConnectionString);
            sqlConnection.Open();

            // Отримати слова з бази даних і додати їх до wordList
            string selectQuery = "SELECT word FROM [Words5]";
            SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                string word = reader["word"].ToString();
                wordList.Add(word);
            }
            reader.Close();

            // Визначення, хто починає гру (гравець або комп'ютер)
            bool isPlayerTurn = true; // Змінна, що вказує, чи починає гравець гру

            if (!isPlayerTurn)
            {
                // Якщо комп'ютер починає гру, отримуємо слово комп'ютера та встановлюємо його у поле вводу
                string computerWord = GetComputerWord("");
                textBox2.Text = computerWord;
                textBox1.Enabled = true; // Робимо поле вводу активним для користувача
                textBox2.Enabled = false; // Робимо поле виводу комп'ютера неактивним для користувача
                UpdatePointsAfterPlayerMove(); // Виклик методу після ходу гравця
            }
            else
            {
                // Якщо гравець починає гру, активуємо поле вводу для гравця
                textBox1.Enabled = true; // Робимо поле вводу активним для користувача
                textBox2.Enabled = false; // Робимо поле виводу комп'ютера неактивним для користувача
                UpdatePointsAfterComputerMove(); // Виклик методу після ходу комп'ютера
            }
        }

        // Гравець вводить слово та натискає кнопку
        private void button1_Click(object sender, EventArgs e)
        {
            string userWord = textBox1.Text.Trim(); // Отримуємо слово, введене користувачем

            // Перевірка, чи слово вже було введено гравцем
            if (enteredWords.Contains(userWord))
            {
                MessageBox.Show("Це слово вже було введено!");
                return;
            }

            // Перевірка, чи слово вже було введено комп'ютером
            if (computerWords.Contains(userWord))
            {
                MessageBox.Show("Комп'ютер вже використовував це слово!");
                return;
            }

            // Додаємо слово до списку введених слів гравцем
            enteredWords.Add(userWord);

            // Перевірка, чи існує слово вже в БД
            string selectQuery = $"SELECT COUNT(*) FROM [Words5] WHERE word = N'{userWord}'";
            SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);
            int existingCount = (int)selectCommand.ExecuteScalar();

            if (existingCount == 0)
            {
                // Перевірка правильності введеного слова користувачем
                DialogResult result = MessageBox.Show("Чи правильно ви ввели слово?", "Перевірка", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Виконуємо вставку, якщо слово не існує і користувач підтвердив його правильність
                    string insertQuery = $"INSERT INTO [Words5] (word) VALUES (N'{userWord}')";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnection);
                    insertCommand.ExecuteNonQuery();
                }
                else
                {
                    textBox1.Text = ""; // Очищаємо поле вводу
                    MessageBox.Show("Уведіть слово ще раз!");
                    return; // Завершуємо обробник події
                }
            }
            textBox1.Text = "";

            string computerWord = GetComputerWord(userWord); // Отримуємо слово комп'ютера

            if (!string.IsNullOrEmpty(computerWord))
            {
                // Перевірка, чи слово вже було введено комп'ютером
                if (computerWords.Contains(computerWord))
                {
                    MessageBox.Show("Комп'ютер вже використовував це слово!");
                    return;
                }

                // Додаємо слово до списку введених слів комп'ютером
                computerWords.Add(computerWord);

                textBox2.Text = computerWord; // Встановлюємо слово комп'ютера у поле вводу

                UpdatePointsAfterComputerMove(); // Оновлення очок після ходу комп'ютера
                UpdatePointsAfterPlayerMove(); // Оновлення очок після ходу гравця
            }
        }

        // Комп'ютер видає слово та передає хід гравцю
        private string GetComputerWord(string userWord)
        {
            string computerWord = null;

            // Перевіряємо, чи слово користувача закінчується на "ь" або "ї"
            if (userWord.EndsWith("ь") || userWord.EndsWith("ї") || userWord.EndsWith("й") || userWord.EndsWith("и"))
            {
                // Перевіряємо, чи слово користувача має довжину більше або рівну 2
                if (userWord.Length >= 2)
                {
                    // Отримуємо передостанню літеру з введеного слова користувача
                    char secondLastLetter = userWord[userWord.Length - 2];

                    // Шукаємо слово в списку, яке починається з передостанньої літери
                    var matchingWords = wordList.Where(w => w.StartsWith(secondLastLetter.ToString(), StringComparison.OrdinalIgnoreCase)).ToList();

                    // Перевіряємо наявність слів в списку введених слів гравцем і комп'ютером
                    var availableWords = matchingWords.Where(w => !enteredWords.Contains(w) && !computerWords.Contains(w)).ToList();

                    // Перевіряємо, чи є доступні слова
                    if (availableWords.Count > 0)
                    {
                        int randomIndex = random.Next(availableWords.Count); // Вибираємо випадкове слово зі списку доступних слів
                        computerWord = availableWords[randomIndex];

                        wordList.Remove(computerWord); // Видаляємо слово зі списку, щоб не повторювати його
                    }
                }
            }
            else
            {
                // Перевіряємо, чи слово користувача має хоча б одну літеру
                if (userWord.Length > 0)
                {
                    // Шукаємо слово в списку, яке починається з останньої літери введеного користувачем слова
                    var matchingWords = wordList.Where(w => w.StartsWith(userWord.Last().ToString(), StringComparison.OrdinalIgnoreCase)).ToList();

                    // Перевіряємо наявність слів в списку введених слів гравцем і комп'ютером
                    var availableWords = matchingWords.Where(w => !enteredWords.Contains(w) && !computerWords.Contains(w)).ToList();

                    // Перевіряємо, чи є доступні слова
                    if (availableWords.Count > 0)
                    {
                        int randomIndex = random.Next(availableWords.Count); // Вибираємо випадкове слово зі списку доступних слів
                        computerWord = availableWords[randomIndex];
                    }
                    // Якщо в списку доступних слів немає, комп'ютер не може знайти слово
                    // Визначаємо, чи гравець має більше очок, ніж комп'ютер
                    else if (playerPoints > computerPoints)
                    {
                        MessageBox.Show($"Гравець переміг! Рахунок: Гравець - {playerPoints}, Комп'ютер - {computerPoints}");
                        this.Close();
                    }
                    else if (computerPoints > playerPoints)
                    {
                        MessageBox.Show($"Комп'ютер переміг! Рахунок: Гравець - {playerPoints}, Комп'ютер - {computerPoints}");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show($"Нічия! Рахунок: Гравець - {playerPoints}, Комп'ютер - {computerPoints}");
                        this.Close();
                    }
                }
            }
            return computerWord;
        }

        // Вихід з форми game5
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            this.Close();
            if (playerPoints > computerPoints)
            {
                MessageBox.Show($"Гравець переміг! Рахунок: Гравець - {playerPoints}, Комп'ютер - {computerPoints}");
                this.Close();
            }
            else if (computerPoints > playerPoints)
            {
                MessageBox.Show($"Комп'ютер переміг! Рахунок: Гравець - {playerPoints}, Комп'ютер - {computerPoints}");
                this.Close();
            }
            else
            {
                MessageBox.Show($"Нічия! Рахунок: Гравець - {playerPoints}, Комп'ютер - {computerPoints}");
                this.Close();
            }
        }

        // Відкриття вікна rules
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            // Створення нового вікна rules
            rules rulesForm = new rules();
            // Відображення нового вікна rules
            rulesForm.Show();
        }
        // Максимальна кількість очок для перемоги
        int maxPoints = 100;

        // Змінні для відстеження кількості очок гравця та комп'ютера
        int playerPoints = 0;
        int computerPoints = 0;

        // Підрахунок очок при кожному ході гравця
        void UpdatePointsAfterPlayerMove()
        {
            // Гравець отримує 10 очок за хід:
            playerPoints += 10;

            // Перевірка, чи гравець досяг максимальної кількості очок для перемоги
            if (playerPoints >= maxPoints)
            {
                // Гравець переміг
                MessageBox.Show("Гравець переміг!");
                // Додайте код для виконання дій після перемоги гравця
            }
        }

        // Підрахунок очок при кожному ході комп'ютера
        void UpdatePointsAfterComputerMove()
        {
            // Комп'ютер отримує 10 очок за хід:
            computerPoints += 10;

            // Перевірка, чи комп'ютер досяг максимальної кількості очок для перемоги
            if (computerPoints >= maxPoints)
            {
                // Комп'ютер переміг
                MessageBox.Show("Комп'ютер переміг!");
                // Додайте код для виконання дій після перемоги комп'ютера
            }
        }
    }
}
