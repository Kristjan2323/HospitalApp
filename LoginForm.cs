using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HospitalApp.Admin;
using HospitalApp.Employee;
using HospitalApp.Patient;

namespace HospitalApp
{
    public partial class LoginForm : Form
    {
       
        DbConnection db = new DbConnection();
    
          
        public static string username;

        int count = 0;
       
       
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            if(db.DbConnectionn().State == ConnectionState.Open)
            {
                db.DbConnectionn().Close();
            }
            else
            {
                db.DbConnectionn().Open();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (db.DbConnectionn().State == ConnectionState.Closed)
            {
                db.DbConnectionn().Open();
            }
          
            try
            {
             
                using (db.DbConnectionn())
                {
                   

                    SqlCommand cmd = db.con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select * from tbLogin where username ='" + txtUsername.Text + "' and password='" + txtPassword.Text + "'";
                    SqlDataReader rd = cmd.ExecuteReader();
                    username = txtUsername.Text;


            

                    if (rd.HasRows)
                    {
                        rd.Read();
                        if (rd[3].ToString()=="Admin")
                        {
                            this.Hide();
                            AdminHomePagecs adminPAge = new AdminHomePagecs();
                            adminPAge.Show();   
                            
                          //  MessageBox.Show("Admin Loged");
                        }
                        if (rd[3].ToString() == "Doctor")
                        {
                            this.Hide();
                            EmployeeHomePage employeePage = new EmployeeHomePage();
                            employeePage.Show();
                         //   MessageBox.Show("Doctor Loged");
                        }
                        if (rd[3].ToString() == "Patient")
                        {
                            this.Hide();
                            PatientHomePage patientHomePage = new PatientHomePage();
                            patientHomePage.Show();
                          //  MessageBox.Show("Patient Loged");
                        }
                        rd.Close();
                        DataTable dt = new DataTable();
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        ad.Fill(dt);
                        count = Convert.ToInt32(dt.Rows.Count.ToString());
                    }
                    else if(count==0)
                    {
                        lblCredentialsIncorrect.Text = "Username and password doesn't match!";
                    }
                }
               


            }
            catch (Exception)
            {

                throw;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //btnRegisterUser

            try
            {
                this.Hide();
                PatientRegisterForm form = new PatientRegisterForm();   
                form.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }
        
      
        private void txtUsername_TextChanged(object sender, EventArgs e)
       {
           
        }
    }
}
