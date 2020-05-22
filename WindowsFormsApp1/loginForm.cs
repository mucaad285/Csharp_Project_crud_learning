using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class loginForm : Form
    {
        //TODO: validations 

        SqlConnection connection = new SqlConnection(@"Data Source = Meecaad ; Initial Catalog = testing ; User ID = sa ; Password = ahmed@1212");

        public loginForm()
        {
            InitializeComponent();
        }

        
        

    internal int Login()
    {
            //this code "COLLATE Latin1_General_CS_AS" makes column in sql casesensative
            SqlCommand command = new SqlCommand("Select Count(Username) from Logintb where username COLLATE Latin1_General_CS_AS = @user " +
            "and Password COLLATE Latin1_General_CS_AS = @pas and role = @rol", connection);
        command.Parameters.AddWithValue("@user", textBox1.Text.Trim());
        command.Parameters.AddWithValue("@pas", textBox2.Text.Trim());
            command.Parameters.AddWithValue("@rol", comboBox1.Text.Trim());
            connection.Open();
            //use execute scaller if your sql statement returns a value
        int count = Convert.ToInt32(command.ExecuteScalar());
        connection.Close();
        return count;
    }
        //this functioin hides and shows the password 
    private void Showhidepasswordcharacter()
        {
            if (textBox2.UseSystemPasswordChar)
            {
                textBox2.UseSystemPasswordChar = false;
                checkBox1.Text = "Show Password";
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
                checkBox1.Text = "Hide Password";
            }
        }

        private void loginForm_Load(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = true;
            checkBox1.Text = "Show Password";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var logincount = Login();
                if (logincount == 1)
                {
                    Form1 main = new Form1();
                    this.Hide();
                    main.Show();
                }
                else
                {

                    textBox1.Text = "";
                    textBox2.Text = "";
                    comboBox1.Text = "";
                    label4.Text = "invalid username or password";
                    label4.Visible = true;

                }
            }
            catch (Exception) { 
                label4.Text = "There is an error please try again";
                label4.Visible = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Showhidepasswordcharacter();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
