using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Тест
{
    public partial class Otchet : Form
    {
        public Otchet()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Вывод на график
            chart1.Visible = true;
            chart1.Series["Res"].XValueMember = "id_test";
            chart1.Series["Res"].YValueMembers = "result";
            chart1.DataSource = testDataSet.Result;//Данные таблицы
            chart1.DataBind();
        }

        SqlDataAdapter sql;
        string podkServer = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True";
        private void Otchet_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "testDataSet.Result". При необходимости она может быть перемещена или удалена.
            this.resultTableAdapter.Fill(this.testDataSet.Result);
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Перед Назад
            Admin admin = new Admin();
            admin.Show();
            this.Hide();
        }
    }
}
