using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;
using FluentEmail.Core;
using FluentEmail.Smtp;
using HospitalApp.Admin;
using Exception = System.Exception;
using Form = System.Windows.Forms.Form;
using Padding = System.Windows.Forms.Padding;
using Color = System.Drawing.Color;
using HospitalApp.Patient;
using HospitalApp.DGVPrinterHelper;
using ZXing;
using Newtonsoft.Json.Linq;
using ActiveUp.Net.Security.OpenPGP.Packets;

namespace HospitalApp.Employee
{
    public partial class EmployeeHomePage : Form
    {

        DbConnection db = new DbConnection();
        LoginForm loginForm = new LoginForm();

        EmailForm emailForm = new EmailForm();
        int rowId;
        string role;
        readonly string employeeId = LoginForm.username;
        public static string serviceBookedBy;
        public static string empEmail;
        public static string patientEmail;
        public static int rowIdEmployeeGrid;
        string bookedServiceDetail;
        string labReportResult;
        string employeeFullName;
        bool serviceSchedule = false;
        bool equipmentSchedule = false;
        bool updateEmpProfile = false;
        bool serviceFilter = false;
        bool equipmentFilter = false;
        bool labResultsService = false;
        bool printAll = false;
        bool printByName = false;
        bool printByDate = false;
        bool serviceChangeSchedule = false;
        bool barcodeMatches = false;
        string typeOfSatistics;
        string satisticsReportText;
        string subjectEmail;
        string bodyEmail;
        string readBarcodeText;


        public EmployeeHomePage()
        {
            InitializeComponent();
        }

        private void EmployeeHomePage_Load(object sender, EventArgs e)
        {
            if (db.DbConnectionn().State == ConnectionState.Open)
            {
                db.DbConnectionn().Close();
            }
            else
            {
                db.DbConnectionn().Open();
            }

            VisibleFalsePanelsEmpPage();
            welcomeLabel();
            getScheduleServicesOrEquipmentsThatPassedDeadlineOrFinished();
          
            UpdateEmployeeProfile();
            updateGridEmployeeBooking();
        }
        void welcomeLabel()
        {
            lblHomeEmployee.Text = employeeId;
            panelHome.Visible = true;
            lblWelcome.Visible = true;
            lblWelcome.Text = $"Welcome employee {employeeId}";
        }
        public void getScheduleServicesOrEquipmentsThatPassedDeadlineOrFinished()
        {
            try
            {
                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }

                    // update status for empScheduleList
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "update tb_empScheduleList set status = @status,orderByStatus= @orderByStatus  where startingTime < @startingTime and status = @getStatus ";
                    cmd.Parameters.AddWithValue("@startingTime", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@getStatus", "Unbooked");
                    cmd.Parameters.AddWithValue("@status", "Deadline passed");
                    cmd.Parameters.AddWithValue("@orderByStatus", 5);
                    cmd.ExecuteNonQuery();

                    SqlCommand cmd1 = db.DbConnectionn().CreateCommand();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.CommandText = "update tb_empScheduleList set status = @status,orderByStatus= @orderByStatus  where endingTime < @endingTime and status = @getStatus ";
                    cmd1.Parameters.AddWithValue("@endingTime", DateTime.UtcNow);
                    cmd1.Parameters.AddWithValue("@getStatus", "Started");
                    cmd1.Parameters.AddWithValue("@status", "Finished");
                    cmd1.Parameters.AddWithValue("@orderByStatus", 6);
                    cmd1.ExecuteNonQuery();

                    SqlCommand cmd4 = db.DbConnectionn().CreateCommand();
                    cmd4.CommandType = CommandType.Text;
                    cmd4.CommandText = "update tb_empScheduleList set status = @status,orderByStatus= @orderByStatus  where endingTime < @endingTime and status = @getStatus   ";
                    cmd4.Parameters.AddWithValue("@endingTime", DateTime.UtcNow);
                    cmd4.Parameters.AddWithValue("@getStatus", "Booked");
                    cmd4.Parameters.AddWithValue("@status", "UnCompleted");
                    cmd4.Parameters.AddWithValue("@orderByStatus", 8);
                    cmd4.ExecuteNonQuery();

                    SqlCommand cmd3 = db.DbConnectionn().CreateCommand();
                    cmd3.CommandType = CommandType.Text;
                    cmd3.CommandText = "update tb_empScheduleListEquipment set status = @status,orderByStatus= @orderByStatus  where endingTime < @endingTime and status = @getStatus  ";
                    cmd3.Parameters.AddWithValue("@endingTime", DateTime.UtcNow);
                    cmd3.Parameters.AddWithValue("@getStatus", "Unbooked");
                    cmd3.Parameters.AddWithValue("@status", "Deadline passed");
                    cmd3.Parameters.AddWithValue("@orderByStatus", 5);
                    cmd3.ExecuteNonQuery();

                    SqlCommand cmd2 = db.DbConnectionn().CreateCommand();
                    cmd2.CommandType = CommandType.Text;
                    cmd2.CommandText = "update tb_empScheduleListEquipment set status = @status,orderByStatus= @orderByStatus  where endingTime < @endingTime and status = @getStatus or status=@getStatus2 ";
                    cmd2.Parameters.AddWithValue("@endingTime", DateTime.UtcNow);
                    cmd2.Parameters.AddWithValue("@getStatus", "Booked");
                    cmd2.Parameters.AddWithValue("@getStatus2", "Started");
                    cmd2.Parameters.AddWithValue("@status", "Finished");
                    cmd2.Parameters.AddWithValue("@orderByStatus", 6);
                    cmd2.ExecuteNonQuery();

                    SqlCommand cmd6 = db.DbConnectionn().CreateCommand();
                    cmd6.CommandType = CommandType.Text;
                    cmd6.CommandText = "update tb_empScheduleListEquipment set status = @status,orderByStatus= @orderByStatus  where endingTime < @endingTime and status = @getStatus   ";
                    cmd6.Parameters.AddWithValue("@endingTime", DateTime.UtcNow);
                    cmd6.Parameters.AddWithValue("@getStatus", "Booked");
                    cmd6.Parameters.AddWithValue("@status", "UnCompleted");
                    cmd6.Parameters.AddWithValue("@orderByStatus", 8);
                    cmd6.ExecuteNonQuery();

                    db.DbConnectionn().Close();
                   // updateGridEmployeeBooking();
                    //  patient.FillScheduleGrid();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void VisibleFalsePanelsEmpPage()
        {
            panelUpdateEmployeeProfile.Visible = false;
            panelViewPatientProfile.Visible = false;
            panelCreateEmpSchedulee.Visible = false;
            panelBooking.Visible = false;
            panelHome.Visible = false;
            panelPrintBookedServOrEquip.Visible = false;
            panelStatisticsReport.Visible = false;
            lblWelcome.Visible = false;
            panelLabReports.Visible = false;
        }

        string getEmployeeFullNameById()
        {
            try
            {
                using (db.DbConnectionn())
                {

                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }

                    if (employeeId != null)
                    {
                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "select empName from tb_EmployeeDetail where empId = '" + employeeId + "'";
                        employeeFullName = (string)cmd.ExecuteScalar();
                    }

                    else
                    {
                        MessageBox.Show("No user is loged in system");
                    }
                }

                return employeeFullName;
            }
            catch (Exception)
            {

                throw;
            }
        }
        void populateServiceCombo()
        {
            try
            {
                using (db.DbConnectionn())
                {
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select serviceName from tb_ServiceDetail";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    comboServiceOrEquipment.DataSource = dt;
                    comboServiceOrEquipment.DisplayMember = "serviceName";
                    comboServicePrint.DataSource = dt;
                    comboServicePrint.DisplayMember = "serviceName";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        void populateEmployeeIdCombo()
        {
            try
            {
                using (db.DbConnectionn())
                {
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select patientId from tbPatientDetail";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    comboPatientId.DataSource = dt;
                    comboPatientId.DisplayMember = "patientId";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        void populateEquipmentCombo()
        {
            try
            {
                using (db.DbConnectionn())
                {
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select equipmentName from tb_EquipmentDetail";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    comboServiceOrEquipment.DataSource = dt;
                    comboServiceOrEquipment.DisplayMember = "equipmentName";
                    comboServicePrint.DataSource = dt;
                    comboServicePrint.DisplayMember = "equipmentName";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void FillTextBoxesForUpdate()
        {
            try
            {
                using (db.DbConnectionn())
                {
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select * from tb_EmployeeDetail where  empId ='" + employeeId + "'";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        txtEmpId.Text = dr["empId"].ToString();
                        txtEmpName.Text = dr["empName"].ToString();
                        txtPassword.Text = dr["empPassword"].ToString();
                        txtEmpQualification.Text = dr["empQualification"].ToString();
                        txtEmplSpecialist.Text = dr["empSpecilisation"].ToString();
                        txtEmpEmail.Text = dr["empEmail"].ToString();
                        txtEmpPhone.Text = dr["empPhone"].ToString();
                        txtEmpAddress.Text = dr["empAddress"].ToString();

                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        void UpdateEmployeeProfile()
        {
            try
            {
                if (updateEmpProfile)
                {

                    using (db.DbConnectionn())
                    {

                        if (db.DbConnectionn().State == ConnectionState.Closed)
                        {
                            db.DbConnectionn().Open();
                        }

                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "update tb_EmployeeDetail set empId = @empId, empName = @empName, empPassword =@empPassword, empQualification = @empQualification, empSpecilisation = @empSpecilisation, empEmail = @empEmail, empPhone = @empPhone, empAddress = @empAddress, empRole = @empRole where empId ='" + employeeId + "' ";
                        cmd.Parameters.AddWithValue("@empId", txtEmpId.Text);
                        cmd.Parameters.AddWithValue("@empName", txtEmpName.Text);
                        cmd.Parameters.AddWithValue("@empPassword", txtPassword.Text);
                        cmd.Parameters.AddWithValue("@empQualification", txtEmpQualification.Text);
                        cmd.Parameters.AddWithValue("@empSpecilisation", txtEmplSpecialist.Text);
                        cmd.Parameters.AddWithValue("@empEmail", txtEmpEmail.Text);
                        cmd.Parameters.AddWithValue("@empPhone", txtEmpPhone.Text);
                        cmd.Parameters.AddWithValue("@empAddress", txtEmpAddress.Text);
                        cmd.Parameters.AddWithValue("@empRole", this.role);
                        cmd.ExecuteNonQuery();

                        SqlCommand cmd2 = db.con.CreateCommand();
                        cmd2.CommandText = "update tbLogin set username = @username, password = @password, role = @role where username ='" + employeeId + "'  ";
                        cmd2.Parameters.AddWithValue("@username", txtEmpId.Text);
                        cmd2.Parameters.AddWithValue("@password", txtPassword.Text);
                        cmd2.Parameters.AddWithValue("@role", this.role);
                        cmd2.ExecuteNonQuery();

                        MessageBox.Show("Employee profile updated!");
                        typeOfSatistics = "Update Profile";
                        satisticsReportText = "You updated your profile";
                        populateStatisticReport();
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        private void btnUpdateEmployeeProfile_Click(object sender, EventArgs e)
        {
            updateEmpProfile = true;
            UpdateEmployeeProfile();
        }

        private void chDoctor_CheckedChanged(object sender, EventArgs e)
        {
            role = chDoctor.Text;
        }

        private void chNurse_CheckedChanged(object sender, EventArgs e)
        {
            role = chNurse.Text;
        }

        private void btnHomeUpdateEmpProfile_Click(object sender, EventArgs e)
        {
            VisibleFalsePanelsEmpPage();
            panelUpdateEmployeeProfile.Visible = true;
            FillTextBoxesForUpdate();
        }

        void populatePatientGrid()
        {
            try
            {
                using (db.DbConnectionn())
                {
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select * from tbPatientDetail";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    gridPatient.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btmPatientSearch_Click(object sender, EventArgs e)
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
                    cmd.CommandText = "select * from tbPatientDetail where patientID like('%" + txtPatientSearch.Text + "%') ";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    gridPatient.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHomeViewPatientProfile_Click(object sender, EventArgs e)
        {
            VisibleFalsePanelsEmpPage();
            panelViewPatientProfile.Visible = true;
            populatePatientGrid();
        }


        private void btnHomeEnterSchedule_Click(object sender, EventArgs e)
        {
            VisibleFalsePanelsEmpPage();
            panelCreateEmpSchedulee.Visible = true;
            panelSecondForSchedule.Visible = false;
        }

        private void panelViewPatientProfile_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnCreateSchedulee_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime start = stTime.Value;
                DateTime until = enTime.Value;
                string serviceDurationSelected = duratService.Text + " hours";


                double resultDurationNum = Convert.ToDouble(duratService.Text);


                using (db.DbConnectionn())
                {

                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }

                    if (serviceSchedule)
                    {

                        if (resultDurationNum >= 1)

                        {
                            for (DateTime dt = start; dt < until; dt = dt.AddHours(resultDurationNum))
                            {

                                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandText = "insert into tb_empScheduleList (service, doctor, startingTime, endingTime, duration, status,orderByStatus) values (@service,@doctor,@startingTime,@endingTime,@duration, @status,@orderByStatus)";
                                cmd.Parameters.AddWithValue("@service", comboServiceOrEquipment.Text);
                                cmd.Parameters.AddWithValue("@doctor", employeeId);
                                cmd.Parameters.AddWithValue("@startingTime", dt);
                                cmd.Parameters.AddWithValue("@endingTime", dt.AddHours(resultDurationNum));
                                cmd.Parameters.AddWithValue("@duration", serviceDurationSelected);
                                cmd.Parameters.AddWithValue("@status", "Unbooked");
                                cmd.Parameters.AddWithValue("@orderByStatus", 2);

                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            double resultDurationMinute = resultDurationNum * 60;

                            for (DateTime dt = start; dt < until; dt = dt.AddMinutes(resultDurationMinute))
                            {

                                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandText = "insert into tb_empScheduleList (service, doctor, startingTime, endingTime, duration,status, orderByStatus) values (@service,@doctor,@startingTime,@endingTime,@duration,@status,@orderByStatus)";
                                cmd.Parameters.AddWithValue("@service", comboServiceOrEquipment.Text);
                                cmd.Parameters.AddWithValue("@doctor", employeeId);
                                cmd.Parameters.AddWithValue("@startingTime", dt);
                                cmd.Parameters.AddWithValue("@endingTime", dt.AddMinutes(resultDurationMinute));
                                cmd.Parameters.AddWithValue("@duration", serviceDurationSelected);
                                cmd.Parameters.AddWithValue("@status", "Unbooked");
                                cmd.Parameters.AddWithValue("@orderByStatus", 2);
                                cmd.ExecuteNonQuery();
                            }

                        }

                    }

                    if (equipmentSchedule)
                    {

                        if (resultDurationNum >= 1)

                        {
                            for (DateTime dt = start; dt < until; dt = dt.AddHours(resultDurationNum))
                            {

                                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandText = "insert into tb_empScheduleListEquipment (equipment, doctor, startingTime, endingTime, duration, status,orderByStatus) values (@equipment,@doctor,@startingTime,@endingTime,@duration, @status, @orderByStatus)";
                                cmd.Parameters.AddWithValue("@equipment", comboServiceOrEquipment.Text);
                                cmd.Parameters.AddWithValue("@doctor", employeeId);
                                cmd.Parameters.AddWithValue("@startingTime", dt);
                                cmd.Parameters.AddWithValue("@endingTime", dt.AddHours(resultDurationNum));
                                cmd.Parameters.AddWithValue("@duration", serviceDurationSelected);
                                cmd.Parameters.AddWithValue("@status", "Unbooked");
                                cmd.Parameters.AddWithValue("@orderByStatus", 2);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            double resultDurationMinute = resultDurationNum * 60;

                            for (DateTime dt = start; dt < until; dt = dt.AddMinutes(resultDurationMinute))
                            {

                                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandText = "insert into tb_empScheduleListEquipment (equipment, doctor, startingTime, endingTime, duration,status,orderByStatus) values (@equipment,@doctor,@startingTime,@endingTime,@duration,@status, @orderByStatus)";
                                cmd.Parameters.AddWithValue("@equipment", comboServiceOrEquipment.Text);
                                cmd.Parameters.AddWithValue("@doctor", employeeId);
                                cmd.Parameters.AddWithValue("@startingTime", dt);
                                cmd.Parameters.AddWithValue("@endingTime", dt.AddMinutes(resultDurationMinute));
                                cmd.Parameters.AddWithValue("@duration", serviceDurationSelected);
                                cmd.Parameters.AddWithValue("@status", "Unbooked");
                                cmd.Parameters.AddWithValue("@orderByStatus", 2);
                                cmd.ExecuteNonQuery();

                            }

                        }

                    }
                    updateGridEmployeeBooking();
                    typeOfSatistics = "Schedule";
                    satisticsReportText = $" You created a schedule for service/equipment {comboServiceOrEquipment.Text}";
                    populateStatisticReport();
                    MessageBox.Show("Schedule fixed!");

                }
                //FillScheduleGrid();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stTime_ValueChanged(object sender, EventArgs e)
        {
            //if(stTime.Value >= DateTime.UtcNow && stTime.Value <= endingTime.Value)
            //{

            //}

            stTime.Format = DateTimePickerFormat.Custom;
            stTime.CustomFormat = "MM/dd/yyyy     HH:mm:ss tt";
            stTime.ShowUpDown = true;
            //else
            //{
            //    lblTimeValidation.Text = "Starting/Ending time can't be less than current time or ending < starting time ";
            //    lblTimeValidation.ForeColor = Color.Red;
            //    btnCreateSchedulee.Enabled = false;
            //}

        }

        private void enTime_ValueChanged(object sender, EventArgs e)
        {
            enTime.Format = DateTimePickerFormat.Custom;
            enTime.CustomFormat = "MM/dd/yyyy    HH:mm:ss tt";
            enTime.ShowUpDown = true;
            //if (stTime.Value >= DateTime.UtcNow && stTime.Value <= endingTime.Value)
            //{


            //}
            //else
            //{
            //    lblTimeValidation.Text = "Starting/Ending time can't be less than current time or ending < starting time ";
            //    lblTimeValidation.ForeColor = Color.Red;
            //    btnCreateSchedulee.Enabled = false;
            //}
        }

        private void btnHomeBookVisited_Click(object sender, EventArgs e)
        {
            VisibleFalsePanelsEmpPage();
            gridBooking.Visible = false;
            panelBooking.Visible = true;
            panelNewSchedule.Visible = false;

        }

        private void btnHomeLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            loginForm.Visible = true;
        }

        private void panelCreateEmpSchedulee_Paint(object sender, PaintEventArgs e)
        {
            getEmployeeFullNameById();
            lblEmpId.Text = employeeFullName;
        }

  
              
            
        void updateGridEmployeeBooking()
        {
          
                try
                {

                    using (db.DbConnectionn())
                    {
                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                        cmd.CommandType = CommandType.Text;

                        if (serviceSchedule)
                        {
                            cmd.CommandText = "select * from tb_empScheduleList  where doctor  ='" + employeeId + "' order by orderByStatus";
                        }
                        else if (equipmentSchedule)
                        {
                            cmd.CommandText = "select * from tb_empScheduleListEquipment where doctor  ='" + employeeId + "' order by orderByStatus";
                        }
                        else
                        {
                            cmd.CommandText = "select * from tb_empScheduleList  where doctor  ='" + employeeId + "' order by orderByStatus";
                        }
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                        gridBooking.DataSource = dt;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            
            
           
        }

        private void gridBooking_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            int yourindexColumn = 9;
            int startAppointmentButton = 0;
            int changeScgeduleButton = 1;
            int cancelButton = 2;

            DataGridViewCell statusCell = gridBooking.Rows[e.RowIndex].Cells[yourindexColumn];
            DataGridViewCell startAppointmentButtonRow = gridBooking.Rows[e.RowIndex].Cells[startAppointmentButton];
            DataGridViewCell changeScgeduleButtonRow = gridBooking.Rows[e.RowIndex].Cells[changeScgeduleButton];
            DataGridViewCell cancelButtonRow = gridBooking.Rows[e.RowIndex].Cells[cancelButton];
            string value = statusCell.Value == null ? string.Empty : statusCell.Value.ToString();

            if (value.ToLower().Equals("unbooked") == true)
            {
                gridBooking.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.CornflowerBlue;

                dataGridViewCellStyle.Padding = new Padding(0, 0, 1000, 0);
                cancelButtonRow.Style = dataGridViewCellStyle;
                cancelButtonRow.ReadOnly = true;
                startAppointmentButtonRow.Style = dataGridViewCellStyle;
                startAppointmentButtonRow.ReadOnly = true;
            }
            else if (value.ToLower().Equals("booked") == true)
            {
                gridBooking.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
            }
            else if (value.ToLower().Equals("cancel") == true)
            {
                gridBooking.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                dataGridViewCellStyle.Padding = new Padding(0, 0, 1000, 0);
                cancelButtonRow.Style = dataGridViewCellStyle;
                cancelButtonRow.ReadOnly = true;
                changeScgeduleButtonRow.Style = dataGridViewCellStyle;
                changeScgeduleButtonRow.ReadOnly = true;
                startAppointmentButtonRow.Style = dataGridViewCellStyle;
                startAppointmentButtonRow.ReadOnly = true;

            }

            else if (value.ToLower().Equals("deadline passed") == true)
            {
                gridBooking.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Gray;
                dataGridViewCellStyle.Padding = new Padding(0, 0, 1000, 0);
                cancelButtonRow.Style = dataGridViewCellStyle;
                cancelButtonRow.ReadOnly = true;
                changeScgeduleButtonRow.Style = dataGridViewCellStyle;
                changeScgeduleButtonRow.ReadOnly = true;
                startAppointmentButtonRow.Style = dataGridViewCellStyle;
                startAppointmentButtonRow.ReadOnly = true;
            }

            else if (value.ToLower().Equals("started") == true)
            {
                gridBooking.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                dataGridViewCellStyle.Padding = new Padding(0, 0, 1000, 0);
                cancelButtonRow.Style = dataGridViewCellStyle;
                cancelButtonRow.ReadOnly = true;
                changeScgeduleButtonRow.Style = dataGridViewCellStyle;
                changeScgeduleButtonRow.ReadOnly = true;
                startAppointmentButtonRow.Style = dataGridViewCellStyle;
                startAppointmentButtonRow.ReadOnly = true;
            }

            else if (value.ToLower().Equals("finished") == true)
            {
                gridBooking.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.PowderBlue;
                dataGridViewCellStyle.Padding = new Padding(0, 0, 1000, 0);
                cancelButtonRow.Style = dataGridViewCellStyle;
                cancelButtonRow.ReadOnly = true;
                changeScgeduleButtonRow.Style = dataGridViewCellStyle;
                changeScgeduleButtonRow.ReadOnly = true;
                startAppointmentButtonRow.Style = dataGridViewCellStyle;
                startAppointmentButtonRow.ReadOnly = true;
            }
        }

        private void btnHomeEmailEmp_Click(object sender, EventArgs e)
        {

            emailForm.Show();
        }

        public string getPatientIdBookedBy()
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
                    
                    if(serviceSchedule)
                    {
                        cmd.CommandText = "select  bookedBy from tb_empScheduleList where Id = '" + rowId + "' ";
                    }
                    else
                    {
                        cmd.CommandText = "select  bookedBy from tb_empScheduleListEquipment where Id = '" + rowId + "' ";
                    }
                    SqlDataReader rd = cmd.ExecuteReader();            

                    if (rd.HasRows)
                    {
                        rd.Read();
                        if (rd[0].ToString() != "")
                        {
                            rd.Close();
                            serviceBookedBy = (string)cmd.ExecuteScalar();
                           

                            return serviceBookedBy;
                        }
                        rd.Close();
                    }
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string GetUserEmail()
        {
            try
            {

                if (employeeId == "admin")
                {
                    empEmail = "admin@admin.com";
                }
                else
                {
                    using (db.DbConnectionn())
                    {
                        if (db.DbConnectionn().State == ConnectionState.Closed)
                        {
                            db.DbConnectionn().Open();
                        }
                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "select empEmail from tb_EmployeeDetail where empId = '" + employeeId + "' ";

                        empEmail = (string)cmd.ExecuteScalar();
                    }

                }
                return empEmail;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string getPatientEmail()
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
                    cmd.CommandText = "select  email from tbPatientDetail where patientID = '" + serviceBookedBy + "' ";
                    patientEmail = (string)cmd.ExecuteScalar();
                }
                return patientEmail;
            }
            catch (Exception)
            {

                throw;
            }
        }
        void sendUserAutomaticEmail()
        {
            try
            {
                if(labResultsService == false)
                {
                    getPatientIdBookedBy();
                    getPatientEmail();
                }
              
                GetUserEmail();
               
            

                var senderEmail = new SmtpSender(() => new SmtpClient("localhost")
                {
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Port = 25
                });

                if (serviceChangeSchedule)
                {
                    subjectEmail = "Your reservation is postponed";
                    bodyEmail = $" Hello {serviceBookedBy} service you have booked have benn postponed. Here are new details {bookedServiceDetail} ";

                    Email.DefaultSender = senderEmail;
                    var email = Email

                        .From(empEmail)
                        .To(patientEmail)
                        .Subject(subjectEmail)
                        .Body(bodyEmail)
                        .Send();
                }
                else if(equipmentSchedule)
                {
                    subjectEmail = "Reservation Cancel";
                    bodyEmail = $"Hello {serviceBookedBy} service you have booked for some reason was cancel. Please feel free to use our online services again.!";
                    Email.DefaultSender = senderEmail;
                    var email = Email

                        .From(empEmail)
                        .To(patientEmail)
                        .Subject(subjectEmail)
                        .Body(bodyEmail)
                        .Send();
                }
                else if(labResultsService)
                {
                    StringBuilder sb = new StringBuilder();
                    subjectEmail = "Lab Report Results";
                   string interduction = $"Hello {serviceBookedBy} here are your lab results:";
                    sb.AppendLine(interduction);
                    sb.AppendLine(labReportResult);
                    bodyEmail = sb.ToString();
                    Email.DefaultSender = senderEmail;
                    var email = Email

                        .From(empEmail)
                        .To(patientEmail)
                        .Subject(subjectEmail)
                        .Body(bodyEmail)
                        .Send();
                }
               
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
                        db.DbConnectionn().Open();
                    }
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "insert into tb_Email (subject,sender,time,body, receiver) values (@subject, @sender, @time, @body, @receiver)";
                    cmd.Parameters.AddWithValue("@subject", subjectEmail);
                    cmd.Parameters.AddWithValue("@body", bodyEmail);
                    cmd.Parameters.AddWithValue("@sender", empEmail);
                    cmd.Parameters.AddWithValue("@receiver", patientEmail);
                    cmd.Parameters.AddWithValue("@time", DateTime.UtcNow);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public string GetDetailBookedService()
        {
            //Detajet e mara nga kjo row do i perdorim si text per barcode ku do jene detajet e sherbimit te rezerevuar
            try
            {
                StringBuilder sb = new StringBuilder();

                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }

                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    if(serviceSchedule || labResultsService)
                    {
                        cmd.CommandText = "select * from tb_empScheduleList where Id = '" + rowId + "' ";
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            sb.Append("Service:  ").ToString();
                            sb.Append(dr["service"]).ToString();
                            sb.Append(" | Doctor:  ").ToString();
                            sb.Append(dr["doctor"]).ToString();
                            sb.Append(" | Starting Time:  ").ToString();
                            sb.Append(dr["startingTime"]).ToString();
                            sb.Append(" | Ending Time:  ").ToString();
                            sb.Append(dr["endingTime"]).ToString();
                            sb.Append(" | Duration: ").ToString();
                            sb.Append(dr["duration"]).ToString();
                            sb.Append(" | Booked By: ").ToString();
                            sb.Append(dr["bookedBy"]).ToString();
                        }
                    }
                    else
                    {
                        cmd.CommandText = "select * from tb_empScheduleListEquipment where Id = '" + rowId + "' ";
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            sb.Append("Equipment:  ").ToString();
                            sb.Append(dr["equipment"]).ToString();
                            sb.Append(" | Doctor:  ").ToString();
                            sb.Append(dr["doctor"]).ToString();
                            sb.Append(" | Starting Time:  ").ToString();
                            sb.Append(dr["startingTime"]).ToString();
                            sb.Append(" | Ending Time:  ").ToString();
                            sb.Append(dr["endingTime"]).ToString();
                            sb.Append(" | Duration: ").ToString();
                            sb.Append(dr["duration"]).ToString();
                            sb.Append(" | Booked By: ").ToString();
                            sb.Append(dr["bookedBy"]).ToString();
                        }
                    }
                
                 

                }

                bookedServiceDetail = sb.ToString();
                return bookedServiceDetail;

            }
            catch (Exception)
            {

                throw;
            }

        }
        private void gridBooking_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewButtonColumn dataGridViewButtonColumn = new DataGridViewButtonColumn();
                gridBooking.CurrentRow.Selected = true;
                int selectedColumn = gridBooking.CurrentCell.ColumnIndex;

                string i = gridBooking.Rows[e.RowIndex].Cells["Id"].FormattedValue.ToString();
                rowId = Convert.ToInt32(i);
                int btnStartAppointment = 0;
                int btnChangeSchedule = 1;
                int btnCancel = 2;
                DataGridViewCell startAppointmentbuttonRow = gridBooking.Rows[e.RowIndex].Cells[btnStartAppointment];
                DataGridViewCell changeSchedulelButtonRow = gridBooking.Rows[e.RowIndex].Cells[btnChangeSchedule];
                DataGridViewCell cancelButtonRow = gridBooking.Rows[e.RowIndex].Cells[btnCancel];

                if (changeSchedulelButtonRow.ColumnIndex == selectedColumn)
                {
                    if (MessageBox.Show("Are you sure want to change the schedule for this service ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        panelNewSchedule.Visible = true;

                        fillNewDateTimeForSchedule();
                    }
                }
                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }
                    DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
                    int statusColumn = 9;
                    DataGridViewCell statusCell = gridBooking.Rows[e.RowIndex].Cells[statusColumn];
                    string value = statusCell.Value == null ? string.Empty : statusCell.Value.ToString();

                    if (startAppointmentbuttonRow.ColumnIndex == selectedColumn)
                    {
                        if (MessageBox.Show("Are you sure want to start an appointment for this service/equipment ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            MessageBox.Show("To start this appointment you must import Barcode!");
                            compareBarcodeWithBookedServiceDetail();
                            if(barcodeMatches)
                            {
                               if  (value.ToLower().Equals("booked") == true)
                               {
                                    using(db.DbConnectionn())
                                    {
                                        if (db.DbConnectionn().State == ConnectionState.Closed)
                                        {
                                            db.DbConnectionn().Open();
                                        }
                                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                                        cmd.CommandType = CommandType.Text;
                                        if (serviceSchedule)
                                        {
                                            cmd.CommandText = "update tb_empScheduleList set status = @status,orderByStatus= @orderByStatus  where Id='" + rowId + "'";
                                        }
                                        else
                                        {
                                            cmd.CommandText = "update tb_empScheduleListEquipment set status = @status,orderByStatus= @orderByStatus  where Id='" + rowId + "'";
                                        }
                                        cmd.Parameters.AddWithValue("@status", SqlDbType.VarChar).Value = "Started";
                                        cmd.Parameters.AddWithValue("@orderByStatus", 1);
                                        cmd.ExecuteNonQuery();
                                    }
                                                                                                    
                               }
                              
                                GetDetailBookedService();
                                sendUserAutomaticEmail();
                                updateGridEmployeeBooking();
                                satisticsReportText = $"You started an appointment for: {bookedServiceDetail}";
                                typeOfSatistics = "Booking";
                                populateStatisticReport();
                            }
                        }                      
                    }

                    if (cancelButtonRow.ColumnIndex == selectedColumn)
                    {
                        if (MessageBox.Show("Are you sure want to cancel this service ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                                                                                                           
                            if (value.ToLower().Equals("booked") == true)
                            {
                                using(db.DbConnectionn())
                                {

                                    if (db.DbConnectionn().State == ConnectionState.Closed)
                                    {
                                        db.DbConnectionn().Open();
                                    }

                                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                                        cmd.CommandType = CommandType.Text;
                                    if (serviceSchedule)
                                    {
                                        cmd.CommandText = "update tb_empScheduleList set status = @status,orderByStatus= @orderByStatus  where Id='" + rowId + "'";
                                    }
                                    else
                                    {
                                        cmd.CommandText = "update tb_empScheduleListEquipment set status = @status,orderByStatus= @orderByStatus  where Id='" + rowId + "'";
                                    }
                                        cmd.Parameters.AddWithValue("@status", SqlDbType.VarChar).Value = "Cancel";
                                        cmd.Parameters.AddWithValue("@orderByStatus", 4);
                                        cmd.ExecuteNonQuery();
                                }
                                       GetDetailBookedService();
                                        sendUserAutomaticEmail();
                                        updateGridEmployeeBooking();
                                        satisticsReportText = $"You cancel: {bookedServiceDetail}";
                                        typeOfSatistics = "Booking";
                                        populateStatisticReport();

                            }                          
                            }
                        }
                            db.DbConnectionn().Close();                       
                }              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string DecodeBarcode()
        {
            try
            {
                GetDetailBookedService();
                PictureBox imgBarcode = new PictureBox();
                var dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var reader = new BarcodeReader();
                    var imgFile = Image.FromFile(dialog.FileName) as Bitmap;
                    imgBarcode.Image = imgFile;
                    var result = reader.Decode(imgFile);
                    MessageBox.Show(result.ToString());
                    readBarcodeText = result.ToString();                 
                }

                return readBarcodeText;
            }
            catch (Exception)
            {

                throw;
            }


        }
        void compareBarcodeWithBookedServiceDetail()
        {
            try
            {
                DecodeBarcode();
                if (readBarcodeText == bookedServiceDetail)
                {
                    lblBookingNotify.Text = string.Empty;
                    lblBookingNotify.Text = "Barcode that you import was correct, appointment started!";
                    barcodeMatches = true;
                    lblBookingNotify.ForeColor = Color.Green;
                }
                else
                {
                    lblBookingNotify.Text = string.Empty;
                    lblBookingNotify.Text = "Barcode that you import was not correct.";
                    lblBookingNotify.ForeColor = Color.Red;
                    barcodeMatches = false;
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void populateStatisticReport()
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
                    cmd.CommandText = "insert into tb_SatisticsReport (satisticsReport, dateTime, typeOfSatistics, userId) values (@satisticsReport,@dateTime, @typeOfSatistics, @userId) ";
                    cmd.Parameters.AddWithValue("@satisticsReport", satisticsReportText);
                    cmd.Parameters.AddWithValue("@dateTime", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@typeOfSatistics", typeOfSatistics);
                    cmd.Parameters.AddWithValue("@userId", employeeId);
                    cmd.ExecuteNonQuery();
                    db.DbConnectionn().Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void printGrid()
        {
           
            DGVPrinter printer = new DGVPrinter();
            printer.Title = "Your Reservation";
            printer.SubTitle = String.Format("Date :{0}", DateTime.Now.Date);
            printer.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            printer.PageNumbers = true;
            printer.PageNumberInHeader = false;
            printer.PorportionalColumns = true;
            printer.HeaderCellAlignment = StringAlignment.Near;
            printer.PrintDataGridView(gridPrint);
            getSatisticsForPrinting();
            populateStatisticReport();
        }

        void getSatisticsForPrinting()
        {

            if (printAll)
            {
                satisticsReportText = "You printed all your booked serviced/equipment";
            }

            else if (printByName)
            {
                satisticsReportText = $"You printed all  booked services/equipment for {comboServicePrint.Text} ";
            }

            else if (printByDate)
            {
                satisticsReportText = $"You printed all services/equpment from date {stTimePrint.Text} to date {endTimePrint.Text}";
            }

            typeOfSatistics = "Print";
        }
        void cheeckIfGridIsEmpty()
        {
            if (gridPrint.Rows != null && gridPrint.Rows.Count != 0)
            {
                gridPrint.Visible = true;
            }
            else
            {
                lblPrintBookedServiceEq.Text = "For this search do not have any record.";
                lblPrintBookedServiceEq.ForeColor = Color.Green;
                gridPrint.Visible = false;
                btnPrintAllBooked.Enabled = false;
                btnPrintByName.Enabled = false;
                btnPrintBookedByDate.Enabled = false;

            }
        }
        void populateGridWithFilterData()
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

                    if (serviceFilter)
                    {
                        if (printAll)
                        {
                            cmd.CommandText = "select * from tb_empScheduleList where doctor = @doctor AND status = @status ";
                            cmd.Parameters.AddWithValue("@doctor", employeeId);
                            cmd.Parameters.AddWithValue("@status", "Booked");

                            DataTable dt = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dt);
                            gridPrint.DataSource = dt;
                            cheeckIfGridIsEmpty();
                        }

                        else if (printByName)
                        {

                            cmd.CommandText = "select * from tb_empScheduleList where doctor = @doctor AND status = @status AND service = @service ";
                            cmd.Parameters.AddWithValue("@doctor", employeeId);
                            cmd.Parameters.AddWithValue("@status", "Booked");
                            cmd.Parameters.AddWithValue("@service", comboServicePrint.Text);

                            DataTable dt = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dt);
                            gridPrint.DataSource = dt;
                            cheeckIfGridIsEmpty();
                        }

                        else if (printByDate)
                        {

                            cmd.CommandText = "select * from tb_empScheduleList where doctor = @doctor AND status = @status ";
                            cmd.Parameters.AddWithValue("@doctor", employeeId);
                            cmd.Parameters.AddWithValue("@status", "Booked");

                            DataTable dt = new DataTable();

                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dt);
                            List<EmployeScheduleList> list = new List<EmployeScheduleList>();

                            foreach (DataRow dr in dt.Rows)
                            {
                                if (stTimePrint.Value <= Convert.ToDateTime(dr["startingTime"]) && endTimePrint.Value >= Convert.ToDateTime(dr["endingTime"]))
                                {
                                    EmployeScheduleList scheduleList = new EmployeScheduleList();
                                    scheduleList.Id = (dr["Id"].ToString());
                                    scheduleList.Service = (dr["service"].ToString());
                                    scheduleList.Doctor = (dr["doctor"].ToString());
                                    scheduleList.StartingTime = (dr["startingTime"].ToString());
                                    scheduleList.EndingTime = (dr["endingTime"].ToString());
                                    scheduleList.Duration = (dr["duration"].ToString());
                                    scheduleList.Status = (dr["status"].ToString());
                                    scheduleList.BookedBy = (dr["bookedBy"].ToString());

                                    list.Add(scheduleList);
                                }
                            }
                            gridPrint.DataSource = list;
                            cheeckIfGridIsEmpty();
                        }
                    }

                    if (equipmentFilter)
                    {
                        if (printAll)
                        {
                            cmd.CommandText = "select * from tb_empScheduleListEquipment where doctor = @doctor AND status = @status ";
                            cmd.Parameters.AddWithValue("@doctor", employeeId);
                            cmd.Parameters.AddWithValue("@status", "Booked");

                            DataTable dt = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dt);
                            gridPrint.DataSource = dt;
                            cheeckIfGridIsEmpty();
                        }

                        else if (printByName)
                        {
                            cmd.CommandText = "select * from tb_empScheduleListEquipment where doctor = @doctor AND status = @status AND equipment = @equipment ";
                            cmd.Parameters.AddWithValue("@doctor", employeeId);
                            cmd.Parameters.AddWithValue("@status", "Booked");
                            cmd.Parameters.AddWithValue("@equipment", comboServicePrint.Text);

                            DataTable dt = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dt);
                            gridPrint.DataSource = dt;
                            cheeckIfGridIsEmpty();
                        }
                        else if (printByDate)
                        {
                            cmd.CommandText = "select * from tb_empScheduleListEquipment where doctor = @doctor AND status = @status ";
                            cmd.Parameters.AddWithValue("@doctor", employeeId);
                            cmd.Parameters.AddWithValue("@status", "Booked");

                            DataTable dt = new DataTable();

                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dt);
                            List<EmployeScheduleList> list = new List<EmployeScheduleList>();
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (stTimePrint.Value <= Convert.ToDateTime(dr["startingTime"]) && endTimePrint.Value >= Convert.ToDateTime(dr["endingTime"]))
                                {
                                    EmployeScheduleList scheduleList = new EmployeScheduleList();
                                    scheduleList.Id = (dr["Id"].ToString());
                                    scheduleList.Service = (dr["equipment"].ToString());
                                    scheduleList.Doctor = (dr["doctor"].ToString());
                                    scheduleList.StartingTime = (dr["startingTime"].ToString());
                                    scheduleList.EndingTime = (dr["endingTime"].ToString());
                                    scheduleList.Duration = (dr["duration"].ToString());
                                    scheduleList.Status = (dr["status"].ToString());
                                    scheduleList.BookedBy = (dr["bookedBy"].ToString());

                                    list.Add(scheduleList);
                                }
                            }
                            gridPrint.DataSource = list;
                            cheeckIfGridIsEmpty();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnServiceChoosedForSchedule_Click(object sender, EventArgs e)
        {
            serviceSchedule = true;
            panelSecondForSchedule.Visible = true;
            populateServiceCombo();
        }

        private void btnEquipmentChoosedForSchedule_Click(object sender, EventArgs e)
        {
            equipmentSchedule = true;
            serviceSchedule = false;
            panelSecondForSchedule.Visible = true;
            populateEquipmentCombo();
        }
        private void btnHomeEmployee_Click(object sender, EventArgs e)
        {
            VisibleFalsePanelsEmpPage();
            panelHome.Visible = true;
        }

        private void btnHomeLogoutEmp_Click(object sender, EventArgs e)
        {
            this.Hide();
            loginForm.Show();
        }

        private void printService_Click(object sender, EventArgs e)
        {
            serviceFilter = true;
            equipmentFilter = false;
            secondPanelPrint.Visible = true;
            panelthirdFilterByName.Visible = false;
            btnPrintAllBooked.Visible = false;
            lblPrintBookedServiceEq.Text = "Print Booked Service:";
            lblPrintBookedServiceEq.ForeColor = Color.Blue;
        }

        private void printEquipment_Click(object sender, EventArgs e)
        {
            equipmentFilter = true;
            serviceFilter = false;
            secondPanelPrint.Visible = true;
            panelthirdFilterByName.Visible = false;
            btnPrintAllBooked.Visible = false;
            lblPrintBookedServiceEq.Text = "Print Booked Equipment:";
            lblPrintBookedServiceEq.ForeColor = Color.Blue;
        }    
        private void btnFilterByName_Click(object sender, EventArgs e)
        {
           
            panelthirdFilterByName.Visible = true;
            panelthirdFilterByDate.Visible = false;
            btnPrintByName.Visible = false;
            gridPrint.Visible = false;
            lblPrintBookedServiceEq.Text = string.Empty;
            if (serviceFilter)
            {
                populateServiceCombo();
            }
            else
            {
                populateEquipmentCombo();
            }
        }
        private void btnChoosePrintByTime_Click(object sender, EventArgs e)
        {
          
            panelthirdFilterByDate.Visible = true;
            panelthirdFilterByName.Visible = false;
            btnPrintBookedByDate.Visible = false;
            gridPrint.Visible = false;
            lblPrintBookedServiceEq.Text = string.Empty;
        }
      
        private void btnHomePrintFilterList_Click(object sender, EventArgs e)
        {
            VisibleFalsePanelsEmpPage();
            panelPrintBookedServOrEquip.Visible = true;
            secondPanelPrint.Visible = false;
            panelthirdFilterByDate.Visible = false;
            panelthirdFilterByName.Visible = false;
            gridPrint.Visible = false;
        }
   
        void getSatisticsReportData()
        {
            SqlCommand cmd = db.DbConnectionn().CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select satisticsReport, dateTime  from tb_SatisticsReport where typeOfSatistics = @typeOfSatistics and userId =@userId ";
            cmd.Parameters.AddWithValue("typeOfSatistics", typeOfSatistics);
            cmd.Parameters.AddWithValue("userId", employeeId);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            dataGrgridSatisticsReport.DataSource = dt;

            if (dataGrgridSatisticsReport.Rows != null && dataGrgridSatisticsReport.Rows.Count != 0)
            {
                dataGrgridSatisticsReport.Visible = true;
                lblSatisticsReport.Text = "Results:";
                lblSatisticsReport.ForeColor = Color.Green;
            }
            else
            {
                lblSatisticsReport.Text = "For this type of satistics not have any record.";
                lblSatisticsReport.ForeColor = Color.Green;
                dataGrgridSatisticsReport.Visible = false;
            }
        }

        private void btnSatistics_Click(object sender, EventArgs e)
        {
            VisibleFalsePanelsEmpPage();
            panelStatisticsReport.Visible = true;
            dataGrgridSatisticsReport.Visible = false;
        }

        private void btnProfileSatistics_Click(object sender, EventArgs e)
        {
            typeOfSatistics = "Update Profile";
            getSatisticsReportData();
           // dataGrgridSatisticsReport.Visible = true;
        }
        private void btnScheduleSatistics_Click(object sender, EventArgs e)
        {
            typeOfSatistics = "Schedule";
            getSatisticsReportData();
          //  dataGrgridSatisticsReport.Visible = true;
        }

        private void btnPrintSatisitcs_Click(object sender, EventArgs e)
        {
            typeOfSatistics = "Print";
            getSatisticsReportData();
          //  dataGrgridSatisticsReport.Visible = true;
        }

        private void btnBookingSatistics_Click(object sender, EventArgs e)
        {
            typeOfSatistics = "Booking";
            getSatisticsReportData();
           // dataGrgridSatisticsReport.Visible = true;
        }
         
        void fillNewDateTimeForSchedule()
        {
            try
            {
                newStartingTime.Format = DateTimePickerFormat.Custom;
                newStartingTime.CustomFormat = "MM/dd/yyyy   HH:mm:ss tt";
                newStartingTime.ShowUpDown = true;

                newEndingTime.Format = DateTimePickerFormat.Custom;
                newEndingTime.CustomFormat = "MM/dd/yyyy   HH:mm:ss tt";
                newEndingTime.ShowUpDown = true;

                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }
                    if (serviceSchedule)
                    {
                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "select * from tb_empScheduleList where Id = '" + rowId + "' ";
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            newStartingTime.Text = dr["startingTime"].ToString();
                            newEndingTime.Text = dr["endingTime"].ToString();
                        }
                    }
                    else
                    {
                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "select * from tb_empScheduleListEquipment where Id = '" + rowId + "' ";
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            newStartingTime.Text = dr["startingTime"].ToString();
                            newEndingTime.Text = dr["endingTime"].ToString();
                        }
                    }                 
                }
                db.DbConnectionn().Close();
            }
            catch (Exception)
            {

                throw;
            }        
        }
        private void btnApplyNewSchedule_Click(object sender, EventArgs e)
        {
            try
            {         
                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }
                    DateTime start = newStartingTime.Value;
                    DateTime end = newEndingTime.Value;
                    TimeSpan timeSpan = end - start;
                    double diffTime = Convert.ToDouble(timeSpan.TotalHours);                
                    // merre service name qe eshte ne rreshtin e slektuar te dataGrid
                    if (serviceSchedule)
                    {
                        SqlCommand cmd1 = db.DbConnectionn().CreateCommand();
                        cmd1.CommandType = CommandType.Text;
                        cmd1.CommandText = "update tb_empScheduleList set startingTime = @startingTime, endingTime= @endingTime  where Id = '" + rowId + "' ";
                        cmd1.Parameters.AddWithValue("startingTime", start);
                        cmd1.Parameters.AddWithValue("endingTime", end);
                        cmd1.ExecuteNonQuery();                    
                    }
                    else
                    {
                        SqlCommand cmd1 = db.DbConnectionn().CreateCommand();
                        cmd1.CommandType = CommandType.Text;
                        cmd1.CommandText = "update tb_empScheduleListEquipment set startingTime = @startingTime, endingTime= @endingTime  where Id = '" + rowId + "' ";
                        cmd1.Parameters.AddWithValue("startingTime", start);
                        cmd1.Parameters.AddWithValue("endingTime", end);
                        cmd1.ExecuteNonQuery();
                    }
                    serviceChangeSchedule = true;
                    updateGridEmployeeBooking();
                    db.DbConnectionn().Close();
                    getPatientIdBookedBy();
                    if(serviceBookedBy != null)
                    {
                        GetDetailBookedService();
                        sendUserAutomaticEmail();
                    }
                    lblBookingNotify.Text = "Timeline for selected reservation was changed.";
                    lblBookingNotify.ForeColor  = Color.Green;
                    satisticsReportText = $"You changed reservation time for : {bookedServiceDetail}";
                    typeOfSatistics = "Booking";
                    populateStatisticReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSeeServiceGrid_Click(object sender, EventArgs e)
        {
            serviceSchedule = true;
            updateGridEmployeeBooking();
            gridBooking.Visible = true;
        }

        private void btnSeeEquipmentGrid_Click(object sender, EventArgs e)
        {
            equipmentSchedule = true;
            serviceSchedule = false;
            updateGridEmployeeBooking();
            gridBooking.Visible = true;
        }

        void saveLabReportDetails()
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
                    cmd.CommandText = "insert into tb_LabReports (patientId,bookedServiceOrEquipment, testedBy, addLabReportsDetails, heartRate, bloodPreasure, phLevel, " +
                        "cholesterolLevel,surcoseLevel,otherDetails,conclusionOrRecommendation, labTestedDate,typeOfTreatment) values (@patientId,@bookedServiceOrEquipment, " +
                        "@testedBy,@addLabReportDetails, @heartRate, @bloodPreasure, @phLevel, @cholesterolLevel, @surcoseLevel,@otherDetails,@conclusionOrRecommendation, @labTestedDate,@typeOfTreatment)";
                    cmd.Parameters.AddWithValue("@patientId", comboPatientId.Text);
                    cmd.Parameters.AddWithValue("@bookedServiceOrEquipment", txtServiceEquipment.Text);
                    cmd.Parameters.AddWithValue("@testedBy", txtTestedBy.Text);
                    cmd.Parameters.AddWithValue("@addLabReportDetails", txtAddLabDetails.Text);
                    cmd.Parameters.AddWithValue("@heartRate", txtHeartRate.Text);
                    cmd.Parameters.AddWithValue("@bloodPreasure", txtBloodPreasure.Text);
                    cmd.Parameters.AddWithValue("@phLevel", txtPhLevel.Text);
                    cmd.Parameters.AddWithValue("@cholesterolLevel", txtCholsLevel.Text);
                    cmd.Parameters.AddWithValue("@surcoseLevel", txtSurcoseLevel.Text);
                    cmd.Parameters.AddWithValue("@otherDetails", txtAddOtherDetails.Text);
                    cmd.Parameters.AddWithValue("@conclusionOrRecommendation", textBtxtConclusionos.Text);
                    cmd.Parameters.AddWithValue("@labTestedDate", dateLabTested.Value);
                    if (labResultsService)
                    {
                        cmd.Parameters.AddWithValue("@typeOfTreatment", "Service");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@typeOfTreatment", "Equipment");
                    }
                    cmd.ExecuteNonQuery();
                  
                    lblRepLabDetails.Text = "Lab Reports details saved successfuly.";
                    lblRepLabDetails.ForeColor = Color.Green;
                    SqlCommand cmd2 = db.DbConnectionn().CreateCommand();
                    cmd2.CommandType = CommandType.Text;
                    if (labResultsService)
                    {
                        cmd2.CommandText = "update tb_empScheduleList set status = @status where Id = @rowId";
                    }
                    else
                    {
                        cmd2.CommandText = "update tb_empScheduleListEquipment set status = @status, orderByStatus = @orderByStatus where Id = @rowId";
                    }
                    cmd2.Parameters.AddWithValue("status", "Finished/Tested");
                    cmd2.Parameters.AddWithValue("orderByStatus", 7);
                    cmd2.Parameters.AddWithValue("rowId", rowId);
                    cmd2.ExecuteNonQuery();
                    db.DbConnectionn().Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSaveLabReports_Click_1(object sender, EventArgs e)
        {
            saveLabReportDetails();
        }


        private void btnHomeLabReports_Click(object sender, EventArgs e)
        {
            VisibleFalsePanelsEmpPage();
            panelLabReports.Visible = true;
            panelSearchPatientbyId.Visible = false; 
            panelLabDetails.Visible = false;
            panelMorelabDetails.Visible = false;    
            panelSaveAndSend.Visible = false;
            populateEmployeeIdCombo();
        }

        private void comboPatientId_SelectedIndexChanged(object sender, EventArgs e)
        {

            comboPatientId.Text = "Select";
        }
        
        private void btnSearchByIdForAvailableBookings_Click(object sender, EventArgs e)
        {
            try
            {
                serviceBookedBy = comboPatientId.Text;
                getPatientEmail();
                lblUserEmail.Text = patientEmail;
                using (db.DbConnectionn())
                {
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    if(labResultsService)
                    {
                        cmd.CommandText = "select * from tb_empScheduleList where doctor = @doctor and status = @status and bookedBy = @patientId";
                    }
                    else
                    {
                        cmd.CommandText = "select * from tb_empScheduleListEquipment where doctor = @doctor and status = @status and bookedBy = @patientId";
                    }
                  
                    cmd.Parameters.AddWithValue("doctor", employeeId);
                    cmd.Parameters.AddWithValue("status", "Finished");
                    cmd.Parameters.AddWithValue("patientId", comboPatientId.Text);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    gridAllAvailablePatientBooking.DataSource = dt;
                    if (gridAllAvailablePatientBooking.Rows != null && gridAllAvailablePatientBooking.Rows.Count != 0)
                    {
                        panelLabDetails.Visible = true;
                    }
                    else
                    {
                        lblRepLabDetails.Text = "For this patientId don't have booked service/equipment";
                        lblRepLabDetails.ForeColor = Color.PowderBlue;
                    }
                        

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnYesAdd_Click(object sender, EventArgs e)
        {
            panelMorelabDetails.Visible = true;
            panelSaveAndSend.Visible = true;
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            panelMorelabDetails.Visible = false;
            panelSaveAndSend.Visible = true;
        }

        private void gridAllAvailablePatientBooking_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string i = gridAllAvailablePatientBooking.Rows[e.RowIndex].Cells["Id"].FormattedValue.ToString();
            rowId = Convert.ToInt32(i);       
            GetDetailBookedService();
            txtServiceEquipment.Text = bookedServiceDetail;
        }

        private void btnServiceLabReports_Click(object sender, EventArgs e)
        {
            labResultsService = true;
            panelSearchPatientbyId.Visible = true;
            lblUserEmail.Text = string.Empty;
            panelLabDetails.Visible = false;
            panelMorelabDetails.Visible=false;
            lblRepLabDetails.Text = "Lab Raport Results for service:";
            lblRepLabDetails.ForeColor = Color.Blue;
        }

        private void btnEquipmentLabReports_Click(object sender, EventArgs e)
        {
            labResultsService = false;
            panelSearchPatientbyId.Visible = true;
            lblUserEmail.Text = string.Empty;
            panelLabDetails.Visible = false;
            panelMorelabDetails.Visible = false;
            lblRepLabDetails.Text = "Lab Raport Results for equipment:";
            lblRepLabDetails.ForeColor = Color.Blue;
        }

        public string getLabReportDetails()
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }

                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                  
                        cmd.CommandText = "SELECT TOP 1 * FROM tb_LabReports ORDER BY ID DESC ";
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                       foreach (DataRow dr in dt.Rows)
                       {
                            sb.Append("[Patient Id]:  ").ToString();
                            sb.Append(dr["patientId"]).ToString();
                            sb.AppendLine();
                            sb.Append(" | Tested By:  ").ToString();
                            sb.Append(dr["testedBy"]).ToString();
                            sb.AppendLine();
                            sb.Append(" | Service/Equipment:  ").ToString();
                            sb.Append(dr["bookedServiceOrEquipment"]).ToString();
                            sb.AppendLine();
                            sb.Append(" | Lab Report Details:  ").ToString();
                            sb.Append(dr["addLabReportsDetails"]).ToString();
                            sb.AppendLine();
                            sb.Append(" | Heart Rate: ").ToString();
                            sb.Append(dr["heartRate"]).ToString();
                            sb.AppendLine();
                            sb.Append(" | Blood Preasure: ").ToString();
                            sb.Append(dr["bloodPreasure"]).ToString();
                            sb.AppendLine();
                            sb.Append(" | Ph Level: ").ToString();
                            sb.Append(dr["phLevel"]).ToString();
                            sb.AppendLine();
                            sb.Append(" |Cholesterol Level: ").ToString();
                            sb.Append(dr["cholesterolLevel"]).ToString();
                            sb.AppendLine();
                            sb.Append(" | Surcose Level: ").ToString();
                            sb.Append(dr["surcoseLevel"]).ToString();
                            sb.AppendLine();
                            sb.Append(" | Other Details: ").ToString();
                            sb.Append(dr["otherDetails"]).ToString();
                            sb.AppendLine();
                            sb.Append(" | Conclusion/Recommendations: ").ToString();
                            sb.Append(dr["conclusionOrRecommendation"]).ToString();
                            sb.AppendLine();
                            sb.Append(" | Lab Tested Date: ").ToString();
                            sb.Append(dr["labTestedDate"]).ToString();
                            sb.AppendLine();
                            sb.Append(" | Type of Treatment: ").ToString();
                            sb.Append(dr["typeOfTreatment"]).ToString();
                        }                 
                }

                labReportResult = sb.ToString();
                return labReportResult;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void btnSaveAndSendLabReports_Click(object sender, EventArgs e)
        {          
            saveLabReportDetails();
            getLabReportDetails();
            labResultsService = true;
            sendUserAutomaticEmail();
        }

 
        private void btnPrintAllBooked_Click(object sender, EventArgs e)
        {
            printGrid();
        }

        private void btnPrintByName_Click(object sender, EventArgs e)
        {
            printGrid();
        }

        private void btnPrintBookedByDate_Click(object sender, EventArgs e)
        {
            printGrid();
        }

        private void btnSearchByName_Click(object sender, EventArgs e)
        {
            printByName = true;
            printAll = false;
            printByDate = false;
            populateGridWithFilterData();
            btnPrintByName.Visible = true;
        }

        private void btnSearchByDate_Click(object sender, EventArgs e)
        {
            printByDate = true;
            printAll = false;
            printByName = false;
            populateGridWithFilterData();
            btnPrintBookedByDate.Visible = true;
        }

        private void btnGetAllBooked_Click(object sender, EventArgs e)
        {
            printAll = true;
            printByDate = false;
            printByName = false;
            populateGridWithFilterData();
            btnPrintAllBooked.Visible = true;
            lblPrintBookedServiceEq.Text = string.Empty;

        }     

    }
}
