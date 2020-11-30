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
    public partial class Login : Form
    {
        DataSet bd;
        SqlDataAdapter sql;
        string podkServer = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True";

        public Login()
        {
            InitializeComponent();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataBank.Login = textBox8.Text;
            DataBank.Pasword = textBox7.Text;

            string exit = "select login, pas from Users where login='" + textBox8.Text + "' and pas='" + textBox7.Text + "'";
            using (SqlConnection podkl = new SqlConnection(podkServer))
            {
                podkl.Open();
                bd = new DataSet();
                sql = new SqlDataAdapter(exit, podkl);
                sql.Fill(bd);
            }

            if (bd.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("Не верный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (bd.Tables[0].Rows[0]["pas"].ToString() == "admin")
                {
                    Admin admin = new Admin();
                    admin.Show();
                    this.Hide();
                }
                else if (bd.Tables[0].Rows[0]["pas"].ToString() == textBox7.Text)
                {                    
                    Test test = new Test();
                    test.Show();
                    this.Hide();
                }               
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox7.UseSystemPasswordChar = false;
                checkBox1.BackColor = Color.FromArgb(192, 255, 192);
            }
            else
            {
                textBox7.UseSystemPasswordChar = true;
                checkBox1.BackColor = Color.LightSalmon;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fam = textBox1.Text;
            string name = textBox2.Text;           
            string otch = textBox3.Text;
            string login = textBox4.Text;
            string pas1 = textBox5.Text;
            string pas2 = textBox6.Text;
            if (String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(fam) || String.IsNullOrWhiteSpace(login) || String.IsNullOrWhiteSpace(pas1) || String.IsNullOrWhiteSpace(pas2))
            {
                MessageBox.Show("Заполните поля!");
            }
            else
            {
                if (textBox5.Text.Length > 6)
                {
                    DataSet bd1, bd2;
                    if (pas1 == pas2)
                    {
                        string id_us = "select MAX(id_user) from Users";
                        using (SqlConnection podkl = new SqlConnection(podkServer))
                        {
                            podkl.Open();
                            bd1 = new DataSet();
                            sql = new SqlDataAdapter(id_us, podkl);
                            sql.Fill(bd1);
                        }
                        int us = Convert.ToInt32(bd1.Tables[0].Rows[0][0]) + 1;

                        string insert = "insert into Users (id_user,fio,login,pas) values(" + Convert.ToString(us) + ", '" + fam + " " + name + " " + otch + "', '" + login + "', '" + pas1 + "') ";
                        using (SqlConnection podkl = new SqlConnection(podkServer))
                        {
                            podkl.Open();
                            bd2 = new DataSet();
                            sql = new SqlDataAdapter(insert, podkl);
                            sql.Fill(bd2);
                        }
                        MessageBox.Show("Вы успешно зарегестрированы!", "", MessageBoxButtons.OK);
                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();
                        textBox4.Clear();
                        textBox5.Clear();
                        textBox6.Clear();

                    }
                    else MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                else
                {
                    MessageBox.Show("Пароль должен быть больше 6 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
