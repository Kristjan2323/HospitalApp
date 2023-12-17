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

namespace HospitalApp
{
    public partial class PatientRegisterForm : Form
    {
        DbConnection db = new DbConnection();
        string gender;
        public PatientRegisterForm()
        {
            InitializeComponent();
        }

        private void PatientRegisterForm_Load(object sender, EventArgs e)
        {
            if (db.DbConnectionn().State == ConnectionState.Open)
            {
                db.con.Close();
            }
            else
            {
                db.con.Open();
            }
        }
      

        private void btnUserRegister_Click(object sender, EventArgs e)
        {
            try
            {
                using(db.DbConnectionn())
                {

                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }
                    //shtojme te dhenat tek table e pacientit nga register form
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "insert into tbPatientDetail (patientId, patientName, password, age, gender, email, phoneNo, address) values (@patientId, @patientName, @password, @age, @gender, @email, @phoneNo, @address )";
                    cmd.Parameters.AddWithValue("@patientId", txtPatientID.Text);
                    cmd.Parameters.AddWithValue("@patientName", txtPatientName.Text);
                    cmd.Parameters.AddWithValue("@password",txtPassword.Text);
                    cmd.Parameters.AddWithValue("@age",txtAge.Text);
                    cmd.Parameters.AddWithValue("@gender", this.gender);
                    cmd.Parameters.AddWithValue("@email",txtEmail.Text);
                    cmd.Parameters.AddWithValue("@phoneNo",txtPhone.Text);
                    cmd.Parameters.AddWithValue("@address",txtAddress.Text);
                    cmd.ExecuteNonQuery();


                    //2 te dhena nga forma register do i vendosim dhe tek tabela Login
                    SqlCommand cmd2 = db.DbConnectionn().CreateCommand();
                    cmd2.CommandText = "insert into tbLogin  (username, password, role) values (@username, @password, @role) ";
                    cmd2.Parameters.AddWithValue("@username", txtPatientID.Text);
                    cmd2.Parameters.AddWithValue("@password",txtPassword.Text);
                    cmd2.Parameters.AddWithValue("@role","Patient");
                    cmd2.ExecuteNonQuery();

                    //3 keto te dhena do i ruajm by default ne fillim qe pacienti nuk ka ber asnje anullim per bookings
                    SqlCommand cmd3 = db.DbConnectionn().CreateCommand();
                    cmd3.CommandType = CommandType.Text;
                    cmd3.CommandText = "insert into tb_CancellationsReport (numberOfCancellations, patientId) values (@numberOfCancellations, @patientId)  ";
                    cmd3.Parameters.AddWithValue("numberOfCancellations", 0);
                    cmd3.Parameters.AddWithValue("patientId", txtPatientID.Text);
                    MessageBox.Show("Patient details added succesfuly");
                    ClearTextBoxes();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
         

        void ClearTextBoxes()
        {
            txtPatientID.Text = string.Empty;
            txtPatientName.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtAge.Text = string.Empty;
            chMale.Checked = false;
            chFemale.Checked = false;
            txtEmail.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtAddress.Text = string.Empty;
        }

        private void chMale_CheckedChanged(object sender, EventArgs e)
        {
            gender = chMale.Text;
        }

        private void chFemale_CheckedChanged(object sender, EventArgs e)
        {
            gender = chFemale.Text;
        }

        private void btbBackToLoginForm_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }
    }
}
