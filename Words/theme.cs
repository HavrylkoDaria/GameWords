using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Words
{
    public partial class theme : Form
    {
        public theme()
        {
            InitializeComponent();
        }

        // Користувач обрав тему "Небесні об'єкти"
        private void button1_Click(object sender, EventArgs e)
        {
            // Створення нового вікна game1
            game1 game1Form = new game1();
            // Відображення нового вікна game1
            game1Form.Show();
            // Закриття вікна theme
            this.Close();
        }

        // Користувач обрав тему "Література"
        private void button2_Click(object sender, EventArgs e)
        {
            // Створення нового вікна game2
            game2 game2Form = new game2();
            // Відображення нового вікна game2
            game2Form.Show();
            // Закриття вікна theme
            this.Close();
        }

        // Користувач обрав тему "Подорожі"
        private void button3_Click(object sender, EventArgs e)
        {
            // Створення нового вікна game3
            game3 game3Form = new game3();
            // Відображення нового вікна game3
            game3Form.Show();
            // Закриття вікна theme
            this.Close();
        }

        // Користувач обрав тему "Живопис"
        private void button4_Click(object sender, EventArgs e)
        {
            // Створення нового вікна game4
            game4 game4Form = new game4();
            // Відображення нового вікна game4
            game4Form.Show();
            // Закриття вікна theme
            this.Close();
        }

        // Користувач обрав тему "Спорт"
        private void button5_Click(object sender, EventArgs e)
        {
            // Створення нового вікна game5
            game5 game5Form = new game5();
            // Відображення нового вікна game5
            game5Form.Show();
            // Закриття вікна theme
            this.Close();
        }
        // Закриття вікна з вибором тем
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Відкриття вікна з правилами
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Створення нового вікна rules
            rules rulesForm = new rules();
            // Відображення нового вікна rules
            rulesForm.Show();
        }

        private void theme_Load(object sender, EventArgs e)
        {

        }
    }
}
