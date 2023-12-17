using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using FluentEmail.Core;
using FluentEmail.Smtp;


namespace HospitalApp.Admin
{
    
    public partial class EmailForm : Form
    {
        DbConnection db = new DbConnection();
        public static string userEmail;
        string userId = LoginForm.username;

       
        public EmailForm()
        {
            InitializeComponent();
        }


        private void AdminEmail_Load(object sender, EventArgs e)
        {
            if (db.DbConnectionn().State == ConnectionState.Closed)
            {
                db.con.Open();
            }


            visibleFalsePanels();
          
        }
        void visibleFalsePanels()
        {
            panelSendEmail.Visible = false;
            panelInbox.Visible = false;
        }

        private void btnFormHomeSendEmail_Click(object sender, EventArgs e)
        {
            panelInbox.Visible = false;
            panelSendEmail.Visible = true;
        }

        private void btnHomeInbox_Click(object sender, EventArgs e)
        {
            populateInboxGrid();
            panelSendEmail.Visible = false; 
            panelInbox.Visible = true;
         

        }

        public string GetUserEmail()
        {
            try
            {
                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select * from tbLogin where username = '" + userId + "' ";
                    SqlDataReader rd = cmd.ExecuteReader();                  

                    if (rd.HasRows)
                    {
                        rd.Read();
                        if (rd[3].ToString() == "Admin")
                        {
                            rd.Close();
                            userEmail = "admin@admin.com";
                          
                        }
                        else  if (rd[3].ToString() == "Doctor" || rd[3].ToString() == "Nurse")
                        {
                          
                            SqlCommand cmd2 = db.DbConnectionn().CreateCommand();
                            cmd2.CommandType = CommandType.Text;
                            cmd2.CommandText = "select empEmail from tb_EmployeeDetail where empId = '" + userId + "' ";
                            rd.Close();
                            userEmail = (string)cmd2.ExecuteScalar();

                        }

                            else  if (rd[3].ToString() == "Patient")
                            {
                           
                                SqlCommand cmd3 = db.DbConnectionn().CreateCommand();
                                cmd3.CommandType = CommandType.Text;
                                cmd3.CommandText = "select email from tbPatientDetail where patientID = '" + userId + "' ";
                            rd.Close();
                            userEmail = (string)cmd3.ExecuteScalar();

                            }                      
                    }


                }
                return userEmail;
            }

            catch (Exception)
            {

                throw;
            }
        }

       public void SendAutomaticEmail()
        {
            try
            {
                GetUserEmail();
                var senderEmail = new SmtpSender(() => new SmtpClient("localhost")
                {
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Port = 25
                });

                Email.DefaultSender = senderEmail;
                var email = Email

                    .From(userEmail)
                    .To(txtSendTo.Text)
                    .Subject(txtSubject.Text)
                    .Body(txtMessage.Text)
                    .Send();
                MessageBox.Show("Email sent successfuly...");
                InsertDataInEmailTable();

            }


            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        private void btnSendEmail_Click_1(object sender, EventArgs e)
        {

            try
            {
                GetUserEmail();
                var senderEmail = new SmtpSender(() => new SmtpClient("localhost")
                {
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Port = 25
                });

                Email.DefaultSender = senderEmail;
                var email = Email

                    .From(userEmail)
                    .To(txtSendTo.Text)
                    .Subject(txtSubject.Text)
                    .Body(txtMessage.Text)
                    .Send();
                MessageBox.Show("Email sent successfuly...");
                InsertDataInEmailTable();

            }

                
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }
        void InsertDataInEmailTable()
        {
            try
            {
                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.con.Open();
                    }
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "insert into tb_Email (subject,sender,time,body) values (@subject, @sender, @time, @body)";
                    cmd.Parameters.AddWithValue("@subject", txtSubject.Text);
                    cmd.Parameters.AddWithValue("@body", txtMessage.Text);
                    cmd.Parameters.AddWithValue("@sender", userEmail);
                    cmd.Parameters.AddWithValue("@time", DateTime.UtcNow);
                    cmd.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        void populateInboxGrid()
        {
           
            try
            {
                GetUserEmail();
                using (db.DbConnectionn())  
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.con.Open();
                    }

                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select * from tb_Email where receiver = '"+ userEmail + "'";
                    cmd.ExecuteNonQuery();
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    gridRecieveEmail.DataSource = dt;


                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void gridRecieveEmail_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            int i = Convert.ToInt32(gridRecieveEmail.SelectedCells[0].Value.ToString());
            try
            {

                if (db.DbConnectionn().State == ConnectionState.Closed)
                {

                    db.DbConnectionn().Open();
                }

                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from tb_Email  where Id  = " + i + " ";
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    txtShowRecieved.Text = dr["body"].ToString();

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void btbBackToAdminForm_Click(object sender, EventArgs e)
        {
            
                this.Hide();
                AdminHomePagecs loginForm = new AdminHomePagecs();
                loginForm.Show();
            
        }



    
    }
}
