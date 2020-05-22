using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        //use this code in the comment bellow to read your connections string from app.config file
        //string cons_smsdb = ConfigurationManager.ConnectionStrings["connection_smsdb"].ConnectionString;


        SqlConnection connection = new SqlConnection(@"Data Source = Meecaad ; Initial Catalog = testing ; User ID = sa ; Password = ahmed@1212");
        int bookid;
        //TODO: validations

        private void clearfeilds()
        {
            pictureBox1.Image = null;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            comboBox1.Text = "";
           
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //save image as bytes
            byte[] images = null;
            FileStream streem = new FileStream(pictureBox1.ImageLocation, FileMode.Open, FileAccess.Read);
            BinaryReader brs = new BinaryReader(streem);
            images = brs.ReadBytes((int)streem.Length);
            try
            {
                SqlCommand command = new SqlCommand("insert into books values ( @title , @isbn , @category, @cover)", connection);
                command.Parameters.AddWithValue("@title", textBox2.Text);
                command.Parameters.AddWithValue("@isbn", textBox3.Text);
                command.Parameters.AddWithValue("@category", comboBox1.Text);
                command.Parameters.AddWithValue("@cover", images);

                connection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show("Book Info Saved");
                connection.Close();
                //call the load function of the this form to reload the form after saving item into the database
                Form1_Load(sender, e);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
          

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            filldatagridviews("");
        }

        private void filldatagridviews(string value)
        {
            connection.Open();
            SqlDataAdapter sqlda = new SqlDataAdapter("select * from books where title like '%"+ value + "%'", connection);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            dataGridView1.RowTemplate.Height = 60;
            dataGridView1.DataSource = dt;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DataGridViewImageColumn im = new DataGridViewImageColumn();
            im = (DataGridViewImageColumn)dataGridView1.Columns[4];
            im.ImageLayout = DataGridViewImageCellLayout.Stretch;
            connection.Close();
            clearfeilds();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SqlCommand command = new SqlCommand("update books set title = @title , isbn = @isbn , category = @category where bookid =  @id ", connection);
                command.Parameters.AddWithValue("@id", bookid);
                command.Parameters.AddWithValue("@title", textBox2.Text);
                command.Parameters.AddWithValue("@isbn", textBox3.Text);
                command.Parameters.AddWithValue("@category", comboBox1.Text);
                connection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show("Book Info Updated");
                connection.Close();
                Form1_Load(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                SqlCommand command = new SqlCommand("delete from books where bookid =  @id ", connection);
                command.Parameters.AddWithValue("@id", bookid);
                connection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show("Book Deleted");
                connection.Close();
                Form1_Load(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            filldatagridviews(textBox4.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //browse the image into picturebox 
            //declare wanted_path as string 
           string wanted_path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            DialogResult result = openFileDialog1.ShowDialog();
            openFileDialog1.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files(*.png)|*.png | JPG Files (*.jpg ) | *.jpg |GIF Files (*.gif) |*.gif ";
            if (result == DialogResult.OK)
            {
                pictureBox1.ImageLocation = openFileDialog1.FileName;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            Byte[] img = (Byte[])dataGridView1.CurrentRow.Cells[4].Value; //this is the row that image lies on dataTable
            MemoryStream ms = new MemoryStream(img);
            pictureBox1.Image = Image.FromStream(ms);

            textBox1.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            bookid = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            textBox2.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            comboBox1.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
          
        }

        private void button4_Click(object sender, EventArgs e)
        {
            loginForm login = new loginForm();
            this.Hide();
            login.ShowDialog();
        }

        internal int countbooksincategory()
        {
            SqlCommand command = new SqlCommand("Select Count(bookid) from books where category = @cat ", connection);
            command.Parameters.AddWithValue("@cat", comboBox2.Text.Trim());
            connection.Open();
            int bks = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return bks;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int num = countbooksincategory();
            label7.Text = num.ToString();
        }
    }
}
