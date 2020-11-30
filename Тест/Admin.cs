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
using Word = Microsoft.Office.Interop.Word;

namespace Тест
{
    public partial class Admin : Form
    {
        DataSet p,p1,p2,p3;
        DataSet del, del1, del2;
        DataSet bd,bd1,bd2,bd3,bd4,bd5,bd6,bd7,bd8;
        DataSet v,v1,v2,v3,v4,v5,v6,v7;
        DataSet i, i1, i2;
        DataSet n, vop,val;

        SqlDataAdapter sql;
        string podkServer = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True";

        private void button18_Click(object sender, EventArgs e)
        {
            Otchet otchet = new Otchet();
            otchet.Show();
            this.Hide();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            string fio = "select fio,id_user from Users where fio='" + comboBox8.Text + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                n = new DataSet();
                sql = new SqlDataAdapter(fio, podkl);
                sql.Fill(n);
            }

            name = Convert.ToString(n.Tables[0].Rows[0]["fio"]);
            if (name != "Admin")//Пользователь не должен быть админом
            {
                string pred = "select Users.fio, Test.test from Test join Result on Test.id_test = " +
                    "Result.id_test join Users on Users.id_user = Result.id_user where Users.fio='"+name+"'";
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    v7 = new DataSet();
                    sql = new SqlDataAdapter(pred, podkl);
                    sql.Fill(v7);
                    dataGridView1.DataSource = v7.Tables[0];
                    dataGridView1.Columns[0].HeaderCell.Value = "Ученик";
                    dataGridView1.Columns[1].HeaderCell.Value = "Тест";
                    panel5.Visible = true;
                }
                
            }
            else
            {
                MessageBox.Show("Вы не можете выбрать себя. \n Выбирите ученика ", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        public string name, test, pred, value;

        //ВЫВОД ИНФОРМАЦИИ ДЛЯ ПЕЧАТИ 
        public void chek() 
        {
            try
            {
                string pr = "select Test.test, Test.id_test, Predmet.predmet from Test inner join" +
                    " Predmet on Test.id_predmet=Predmet.id_predmet where Test.test='" + comboBox9.Text + "'";
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    vop = new DataSet();
                    sql = new SqlDataAdapter(pr, podkl);
                    sql.Fill(vop);
                }
                test = Convert.ToString(vop.Tables[0].Rows[0]["test"]);
                pred = Convert.ToString(vop.Tables[0].Rows[0]["predmet"]);

                string res = "select DISTINCT result from Result where id_test=" + vop.Tables[0].Rows[0]["id_test"].ToString()
                    + " and id_user=" + n.Tables[0].Rows[0]["id_user"].ToString();
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    val = new DataSet();
                    sql = new SqlDataAdapter(res, podkl);
                    sql.Fill(val);
                }
                value = Convert.ToString(val.Tables[0].Rows[0]["result"]);
     
            }
            catch 
            {
                MessageBox.Show("Учащийся не решал тест!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           

        }
        /***********************************************************/

        //Вывод печати результатов
        private void button16_Click(object sender, EventArgs e)
        {
            chek();
            var WordApp = new Word.Application();
            WordApp.Visible = false;
            //путь к шаблону
            var Worddoc = WordApp.Documents.Open(Application.StartupPath + @"\1.docx");
            //заполнение                       
            Repwo("{name}", name.ToString(), Worddoc);
            Repwo("{pred}", pred.ToString(), Worddoc);
            Repwo("{test}", test.ToString(), Worddoc);
            Repwo("{value}", value.ToString(), Worddoc);
            //сохранение
            Worddoc.SaveAs2(Application.StartupPath + $"\\{name}, {test} {DateTime.Now.ToLongDateString()}" + ".docx");
            WordApp.Visible = true;
        }
        private void Repwo(string subToReplace, string text, Word.Document worddoc)
        {
            var range = worddoc.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: subToReplace, ReplaceWith: text);
        }


        //*****************ИЗМЕНЕНИЕ ФОРМЫ*************
        private void button6_Click(object sender, EventArgs e)
        {
            string id = "select vopros from Vopros where vopros='" + comboBox7.Text + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd7 = new DataSet();
                sql = new SqlDataAdapter(id, podkl);
                sql.Fill(bd7);
            }
            panel4.Visible = true;
        }
        private void button15_Click(object sender, EventArgs e)
        {               
            int otvet = Convert.ToInt32(textBox8.Text);
            if (otvet > 3 || otvet < 0)
            {
                MessageBox.Show("Ответ должен совпадать с номером варианта!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string pred = "UPDATE Vopros set vopros = '"+ textBox12.Text  + "', var1 = '"+textBox11.Text+ "', " +
                    "var2 = '" + textBox10.Text + "', var3 = '" + textBox9.Text + "', otvet = '" + textBox8.Text + "'" +
                    " where vopros = '" + bd7.Tables[0].Rows[0][0].ToString() + "'";
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    bd8 = new DataSet();
                    sql = new SqlDataAdapter(pred, podkl);
                    sql.Fill(bd8);
                }
                string vivod = "select Predmet.predmet, Test.test, Vopros.vopros, Vopros.var1, Vopros.var2, Vopros.var3, Vopros.otvet " +
                    "from Test join Predmet on Test.id_predmet = Predmet.id_predmet join Vopros on Vopros.id_test = Test.id_test";
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    v6 = new DataSet();
                    sql = new SqlDataAdapter(vivod, podkl);
                    sql.Fill(v6);
                    dataGridView1.DataSource = v6.Tables[0];
                    dataGridView1.Columns[0].HeaderCell.Value = "Дисциплина";
                    dataGridView1.Columns[1].HeaderCell.Value = "Тест";
                    dataGridView1.Columns[2].HeaderCell.Value = "Вопрос";
                    dataGridView1.Columns[3].HeaderCell.Value = "Вариант №1";
                    dataGridView1.Columns[4].HeaderCell.Value = "Вариант №2";
                    dataGridView1.Columns[5].HeaderCell.Value = "Вариант №3";
                    dataGridView1.Columns[6].HeaderCell.Value = "Ответ";
                }
                textBox8.Clear();
                textBox9.Clear();
                textBox10.Clear();
                textBox11.Clear();
                textBox12.Clear();
            }

        }
        //***************************************************************8

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }     

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }

        public Admin()
        {
            InitializeComponent();
        }
        private void Admin_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "testDataSet.Users". При необходимости она может быть перемещена или удалена.
            this.usersTableAdapter.Fill(this.testDataSet.Users);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "testDataSet.Predmet". При необходимости она может быть перемещена или удалена.
            this.predmetTableAdapter.Fill(this.testDataSet.Predmet);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "testDataSet.Vopros". При необходимости она может быть перемещена или удалена.
            this.voprosTableAdapter.Fill(this.testDataSet.Vopros);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "testDataSet.Test". При необходимости она может быть перемещена или удалена.
            this.testTableAdapter.Fill(this.testDataSet.Test);

            //ПОИСК МАКСИМАЛЬНОГО ИНДЕКСА
            string max = "select max(Predmet.id_predmet), max(Test.id_test) from Predmet join Test on Test.id_predmet = Predmet.id_predmet";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd3 = new DataSet();
                sql = new SqlDataAdapter(max, podkl);
                sql.Fill(bd3);
            }

        }      

        //*********ФОРМА ВЫВОДА************** 
        private void button1_Click(object sender, EventArgs e)//Вывод УЧЕНИКОВ
        {
            string user = "select fio from Users where fio!='Admin'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                p = new DataSet();
                sql = new SqlDataAdapter(user, podkl);
                sql.Fill(p);
                dataGridView1.DataSource = p.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "Ученик";
            }
        }
        private void button4_Click(object sender, EventArgs e)//Вывод РЕЗУЛЬТАТОВ ТЕСТИРОВАНИЯ
        {
            string pred = "select Users.fio,Test.test,Result.result, Result.ball,Result.time,Result.date " +
                "from Test join Result on Test.id_test = Result.id_test join Users on Users.id_user = Result.id_user";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                p3 = new DataSet();
                sql = new SqlDataAdapter(pred, podkl);
                sql.Fill(p3);
                dataGridView1.DataSource = p3.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "Ученик";
                dataGridView1.Columns[1].HeaderCell.Value = "Тест";
                dataGridView1.Columns[2].HeaderCell.Value = "Оценка";
                dataGridView1.Columns[3].HeaderCell.Value = "Балл";
                dataGridView1.Columns[4].HeaderCell.Value = "Время прохождения";
                dataGridView1.Columns[5].HeaderCell.Value = "Дата тестирования";

            }
        }

        private void button5_Click(object sender, EventArgs e)//Вывод ВОПРОСОВ
        {
            string pred = "select Predmet.predmet, Test.test, Vopros.vopros, Vopros.var1, Vopros.var2, Vopros.var3, Vopros.otvet " +
                "from Test join Predmet on Test.id_predmet = Predmet.id_predmet join Vopros " +
                "on Vopros.id_test = Test.id_test where Test.test = '" + comboBox1.Text + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                p2 = new DataSet();
                sql = new SqlDataAdapter(pred, podkl);
                sql.Fill(p2);
                dataGridView1.DataSource = p2.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "Дисциплина";
                dataGridView1.Columns[1].HeaderCell.Value = "Тест";
                dataGridView1.Columns[2].HeaderCell.Value = "Вопрос";
                dataGridView1.Columns[3].HeaderCell.Value = "Вариатн №1";
                dataGridView1.Columns[4].HeaderCell.Value = "Вариатн №2";
                dataGridView1.Columns[5].HeaderCell.Value = "Вариатн №3";
                dataGridView1.Columns[6].HeaderCell.Value = "Ответ";
            }
        }

        private void button2_Click(object sender, EventArgs e)//Вывод ПРЕДМЕТОВ
        {
            string pred = "select predmet from Predmet";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                p1 = new DataSet();
                sql = new SqlDataAdapter(pred, podkl);
                sql.Fill(p1);
                dataGridView1.DataSource = p1.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "Дисциплина";
            }
        }
        private void button3_Click(object sender, EventArgs e)//Вывод ТЕСТОВ
        {
            string pred = "select test from Test";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                p1 = new DataSet();
                sql = new SqlDataAdapter(pred, podkl);
                sql.Fill(p1);
                dataGridView1.DataSource = p1.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "Тест";
            }
            panel1.Visible = true;
        }
        //****************************************************************

        //*********ФОРМА УДАЛЕНИЯ************** 
        private void button9_Click(object sender, EventArgs e)//Удаление вопроса
        {
            string id = "select id_vopros from Vopros where vopros='" + comboBox4.Text + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd2 = new DataSet();
                sql = new SqlDataAdapter(id, podkl);
                sql.Fill(bd2);
            }
            string delete = "delete from Vopros where id_vopros=" + bd2.Tables[0].Rows[0][0].ToString();
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                del2 = new DataSet();
                sql = new SqlDataAdapter(delete, podkl);
                sql.Fill(del2);
            }
            string pred = "select vopros, var1, var2, var3, otvet from Vopros";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                v2 = new DataSet();
                sql = new SqlDataAdapter(pred, podkl);
                sql.Fill(v2);
                dataGridView1.DataSource = v2.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "Вопрос";
                dataGridView1.Columns[0].HeaderCell.Value = "Вариатн №1";
                dataGridView1.Columns[0].HeaderCell.Value = "Вариатн №2";
                dataGridView1.Columns[0].HeaderCell.Value = "Вариатн №3";
                dataGridView1.Columns[0].HeaderCell.Value = "Ответ";
            }
        }
        private void button7_Click(object sender, EventArgs e)//Удаление ПРЕДМЕТА
        {
            string id = "select id_predmet from Predmet where predmet='" + comboBox2.Text + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd = new DataSet();
                sql = new SqlDataAdapter(id, podkl);
                sql.Fill(bd);
            }
            string delete = "delete from Predmet where id_predmet=" + bd.Tables[0].Rows[0][0].ToString();
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                del = new DataSet();
                sql = new SqlDataAdapter(delete, podkl);
                sql.Fill(del);
            }
            string pred = "select predmet from Predmet";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                v = new DataSet();
                sql = new SqlDataAdapter(pred, podkl);
                sql.Fill(v);
                dataGridView1.DataSource = v.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "Дисциплина";
            }
        }
        private void button8_Click(object sender, EventArgs e)//Удаление ТЕСТА
        {
            string id = "select id_test from Test where test='" + comboBox3.Text + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd1 = new DataSet();
                sql = new SqlDataAdapter(id, podkl);
                sql.Fill(bd1);
            }
            string delete = "delete from Test where id_test=" + bd1.Tables[0].Rows[0][0].ToString();
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                del1 = new DataSet();
                sql = new SqlDataAdapter(delete, podkl);
                sql.Fill(del1);
            }
            string pred = "select test from Test";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                v1 = new DataSet();
                sql = new SqlDataAdapter(pred, podkl);
                sql.Fill(v1);
                dataGridView1.DataSource = v1.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "Тест";
            }
        }
        //***************************************************

       //**********ФОРМА ДОБАВЛЕНИЯ*********************
        private void button10_Click(object sender, EventArgs e)//ДОБАВЛЕНИЕ ПРЕДМЕТА
        {
            int id_pred = Convert.ToInt32(bd3.Tables[0].Rows[0][0]) + 1;
            string pred = "insert into Predmet(id_predmet,predmet) values(" + id_pred.ToString() + ", '" + textBox1.Text + "')";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                i = new DataSet();
                sql = new SqlDataAdapter(pred, podkl);
                sql.Fill(i);
            }
            string vivod = "select predmet from Predmet";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                v3 = new DataSet();
                sql = new SqlDataAdapter(vivod, podkl);
                sql.Fill(v3);
                dataGridView1.DataSource = v3.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "Дисциплина";
            }
            textBox1.Clear();
        }
        private void button12_Click(object sender, EventArgs e)
        {
            string id = "select id_predmet from Predmet where predmet='" + comboBox5.Text + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd4 = new DataSet();
                sql = new SqlDataAdapter(id, podkl);
                sql.Fill(bd4);
            }
            panel2.Visible = true;
        }
        private void button11_Click(object sender, EventArgs e)
        {
            int id_test = Convert.ToInt32(bd3.Tables[0].Rows[0][1]) + 1;
            string pred = "insert into Test(id_test,test,id_predmet) values(" + id_test.ToString() + ", '" + textBox2.Text + "', " + bd4.Tables[0].Rows[0][0].ToString() + ")";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                i1 = new DataSet();
                sql = new SqlDataAdapter(pred, podkl);
                sql.Fill(i1);
            }
            string vivod = "select Predmet.predmet, Test.test from Predmet join Test on Test.id_predmet = Predmet.id_predmet";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                v4 = new DataSet();
                sql = new SqlDataAdapter(vivod, podkl);
                sql.Fill(v4);
                dataGridView1.Visible = true;
                dataGridView1.DataSource = v4.Tables[0];
                dataGridView1.Columns[0].HeaderCell.Value = "Дисциплина";
                dataGridView1.Columns[1].HeaderCell.Value = "Тест";
            }
            textBox2.Clear();
        }
        private void button14_Click(object sender, EventArgs e)
        {
            string test = "select id_test from Test where test='" + comboBox6.Text + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd5 = new DataSet();
                sql = new SqlDataAdapter(test, podkl);
                sql.Fill(bd5);
            }
            panel3.Visible = true;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string max = "select MAX(id_vopros) from Vopros where id_test=" + bd5.Tables[0].Rows[0][0].ToString();
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd6 = new DataSet();
                sql = new SqlDataAdapter(max, podkl);
                sql.Fill(bd6);
            }
            int id_vop = Convert.ToInt32(bd6.Tables[0].Rows[0][0]) + 1;
            int otvet = Convert.ToInt32(textBox7.Text);
            if (otvet > 3 || otvet < 0)
            {
                MessageBox.Show("Ответ должен совпадать с номером варианта!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string pred = "insert into Vopros(id_vopros,id_test,vopros,var1,var2,var3,otvet) values(" + id_vop.ToString() + ", " + bd5.Tables[0].Rows[0]["id_test"].ToString() +
                ", '" + textBox3.Text + "', '" + textBox4.Text + "', '" + textBox5.Text + "', '" + textBox6.Text + "', " + textBox7.Text + ")";
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    i2 = new DataSet();
                    sql = new SqlDataAdapter(pred, podkl);
                    sql.Fill(i2);
                }
                string vivod = "select Predmet.predmet, Test.test, Vopros.vopros, Vopros.var1, Vopros.var2, Vopros.var3, Vopros.otvet " +
                    "from Test join Predmet on Test.id_predmet = Predmet.id_predmet join Vopros on Vopros.id_test = Test.id_test where Test.id_test=" + bd5.Tables[0].Rows[0]["id_test"].ToString();
                using (SqlConnection podkl = new SqlConnection(podkServer))
                {
                    podkl.Open();
                    v5 = new DataSet();
                    sql = new SqlDataAdapter(vivod, podkl);
                    sql.Fill(v5);
                    dataGridView1.DataSource = v5.Tables[0];
                    dataGridView1.Columns[0].HeaderCell.Value = "Дисциплина";
                    dataGridView1.Columns[1].HeaderCell.Value = "Тест";
                    dataGridView1.Columns[2].HeaderCell.Value = "Вопрос";
                    dataGridView1.Columns[3].HeaderCell.Value = "Вариант №1";
                    dataGridView1.Columns[4].HeaderCell.Value = "Вариант №2";
                    dataGridView1.Columns[5].HeaderCell.Value = "Вариант №3";
                    dataGridView1.Columns[6].HeaderCell.Value = "Ответ";
                }
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                textBox6.Clear();
                textBox7.Clear();
            }
        }
        //**********************************************



    }
}
