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
    public partial class Result : Form
    {
        DataSet bd, bd1, bd2, bd4;
        DataSet v, v1;
        SqlDataAdapter sql;
        string podkServer = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True";

        string log, pas;
        public Result()
        {
            InitializeComponent();
        }

        private void Result_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "testDataSet.Predmet". При необходимости она может быть перемещена или удалена.
            this.predmetTableAdapter.Fill(this.testDataSet.Predmet);
            log = DataBank.Login;
            pas = DataBank.Pasword;

            //Проверка пользователя
            string exit = "select fio from Users where login='" + log + "' and pas='" + pas + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd = new DataSet();
                sql = new SqlDataAdapter(exit, podkl);
                sql.Fill(bd);
            }
            string fio = bd.Tables[0].Rows[0][0].ToString(); //Пользователь
            label1.Text = fio;            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label3.Visible = true;
            dataGridView1.Visible = true;
            string p = "select Test.id_test,Predmet.id_predmet from Test inner join Predmet on Test.id_predmet = Predmet.id_predmet " +
                "where Predmet.predmet = '" + comboBox1.Text + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd1 = new DataSet();
                sql = new SqlDataAdapter(p, podkl);
                sql.Fill(bd1);
            }

            string room = "select Test.test,Result.result, Result.ball,Result.time,Result.date " +
                "from Test join Result on Test.id_test = Result.id_test join Users on Users.id_user = Result.id_user " +
                "where Test.id_predmet = "+ bd1.Tables[0].Rows[0]["id_predmet"].ToString()+" and Users.fio = '" + bd.Tables[0].Rows[0][0].ToString()+"'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd2 = new DataSet();
                sql = new SqlDataAdapter(room, podkl);
                sql.Fill(bd2);
                dataGridView1.DataSource = bd2.Tables[0];
            }
        }

    }
}
