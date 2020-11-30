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
    public partial class Test : Form
    {
        DataSet bd, bd1,bd3,bd4;
        DataSet v,v1;
        SqlDataAdapter sql;
        string podkServer = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True";
        Result result;

        int n, m, s;
        public int i=1;
        int bal =100;
        string var="",otvet="",rez="";
        string log, pas;
        
        public Test()
        {           
            InitializeComponent();
            timer1.Interval = 1000;
            m = 0;
            s = 0;
            result = new Result();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            var = "2";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            var = "3";

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            var = "1";
            
        }

        private void Test_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "testDataSet.Predmet". При необходимости она может быть перемещена или удалена.
            this.predmetTableAdapter.Fill(this.testDataSet.Predmet);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "testDataSet.Test". При необходимости она может быть перемещена или удалена.
            this.testTableAdapter.Fill(this.testDataSet.Test);

        }
        private void button1_Click(object sender, EventArgs e)
        {
            string test = "select Test.id_test,Test.test,Predmet.predmet from Test inner join Predmet on Test.id_predmet = Predmet.id_predmet " +
                "where Test.test = '" + comboBox1.Text + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd = new DataSet();
                sql = new SqlDataAdapter(test, podkl);
                sql.Fill(bd);
            }           
            label1.Text = "" + bd.Tables[0].Rows[0]["predmet"].ToString() + ", Тест: " + bd.Tables[0].Rows[0]["test"].ToString();

            //Вывод числа вопросов в тесте
            string max = "select MAX(id_vopros) from Vopros where id_test=" + bd.Tables[0].Rows[0]["id_test"].ToString();
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd1 = new DataSet();
                sql = new SqlDataAdapter(max, podkl);
                sql.Fill(bd1);
            }
            n = Convert.ToInt32(bd1.Tables[0].Rows[0][0]);//Кол-во вопросов в тесте 
            panel1.Visible = true;
            panel3.Visible = false;
            show();
            timer1.Enabled = true;
            log = DataBank.Login;
            pas = DataBank.Pasword;

        }

        private void button2_Click_1(object sender, EventArgs e)//Далее
        {
            if (i != n)
            {
                if (var != otvet)
                {
                    bal -= Convert.ToInt32((100 / n));//Подсчет баллов
                }
                i++;
                show();   
            }
            else
            {       
                MessageBox.Show("Тест завершен \n Вы набрали "+bal,"",MessageBoxButtons.OK,MessageBoxIcon.Information);               
                timer1.Enabled = false;
                insert();
                result.Show();
                this.Hide();
                
            }
        }   
        //Вывод вопросов и варианттов ответа
        public void show()
        {
            //Вывод вопроса
            string vop ="select vopros, var1, var2, var3, otvet from Vopros where id_test ="+ bd.Tables[0].Rows[0]["id_test"].ToString()+ "and id_vopros=" +i;
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                v = new DataSet(); 
                sql = new SqlDataAdapter(vop, podkl);
                sql.Fill(v);
            }
            label3.Text = v.Tables[0].Rows[0]["vopros"].ToString();            
            radioButton1.Text = v.Tables[0].Rows[0]["var1"].ToString();           
            radioButton2.Text = v.Tables[0].Rows[0]["var2"].ToString();           
            radioButton3.Text = v.Tables[0].Rows[0]["var3"].ToString();
            otvet = v.Tables[0].Rows[0]["otvet"].ToString();
           
        }
        
        public void insert()
        {
            //Оценка теста 
            if (bal >= 85)
                rez = "5";
            else if (bal >= 70 & bal < 85)
                rez = "4";
            else if (bal >= 55 & bal < 70)
                rez = "3";
            else
                rez = "2";

            //Проверка пользователя
            string exit = "select id_user from Users where login='" + log + "' and pas='" + pas + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd3 = new DataSet();
                sql = new SqlDataAdapter(exit, podkl);
                sql.Fill(bd3);
            }
            string id_us = bd3.Tables[0].Rows[0][0].ToString(); //Код пользователя

            string id_rez = "select Max(id_result) from Result";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd4 = new DataSet();
                sql = new SqlDataAdapter(id_rez, podkl);
                sql.Fill(bd4);
            }
            int id = Convert.ToInt32(bd4.Tables[0].Rows[0][0]) + 1;

            //Добавление в таблицу
            string ins = "insert into Result(id_result,id_user,id_test,result,ball,time,date) values" +
                "(" + Convert.ToString(id) + ", " + id_us + ", " + bd.Tables[0].Rows[0]["id_test"].ToString() + ", " + rez + ", " + bal.ToString() + ", '00:" + m.ToString() + ":" + s.ToString() + "', GETDATE())";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                v1 = new DataSet();
                sql = new SqlDataAdapter(ins, podkl);
                sql.Fill(v1);
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (s < 59)
            {
                s++;
                if (s < 10)
                    label5.Text = "0" + s.ToString();
                else
                    label5.Text = s.ToString();
            }
            else
            {
                if (m < 59)
                {
                    m++;
                    if (m < 10)
                        label4.Text = "0" + m.ToString();
                    else
                        label4.Text = m.ToString();
                    s = 0;
                    label5.Text = "00";
                }
                else
                {
                    m = 0;
                    label4.Text = "00";
                }
            }
            if (label4.Text == "10")
            {
                timer1.Enabled = false;
                MessageBox.Show("Время закончилось!\nРезультаты тестирования будут начислины за отвеченые вопросы.", "Тест завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                insert();
                result.Show();
                this.Hide();                
            }
        }
    }
}
