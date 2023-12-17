using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace HospitalApp.Admin
{
    public partial class AdminHomePagecs : Form
    {
        DbConnection db = new DbConnection();
      public static  List<DataTable> gridServiceData = new List<DataTable>();
        readonly string adminId = LoginForm.username;
        string typeOfSatistics;
        string satisticsReportText;
        string role;
        public AdminHomePagecs()
        {
            InitializeComponent();
        }

        private void AdminHomePagecs_Load(object sender, EventArgs e)
        {
             if (db.DbConnectionn().State == ConnectionState.Open)
            {
                db.DbConnectionn().Close();
            }
            else
            {
                db.DbConnectionn().Open();
            }

          
            VisibleFalsePanels();        
            populateEmployeeGrid();
            populateOnlineServicesGrid();
            populateEquipmentForServiceGrid();
            populatePatientGrid();
            populateEmployeeCombo();
            populateRoomCombo();
            fixTimeShow();
            welcomeLabel();

        }

        void welcomeLabel()
        {
            lblAdmin.Text = adminId;
            panelHome.Visible = true;
            panelHome.Visible = true;
            lblWelcome.Visible = true;       
            lblWelcome.Text = $"Welcome {adminId}";
        }
        void VisibleFalsePanels()
        {
            panelAddNewEmployee.Visible = false;
            panelViewEmployee.Visible = false;
            panelViewPatient.Visible = false;
            panelAddNewService.Visible = false;
            panelOnlineServices.Visible = false;
            panelEquipment.Visible = false;
            panelAddRoom.Visible = false;
            panelEquipmentForService.Visible = false;
            panelSatisticsReport.Visible = false;
            panelHome.Visible = false;
            lblWelcome.Visible = false;

        }
        private void btnAddEmploy_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (db.con)
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }
                    //shtojme te dhenat tek table e employee nga register form
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "insert into tb_EmployeeDetail (empId, empName, empPassword, empQualification, empSpecilisation, empEmail, empPhone, empAddress, empRole) values (@empId, @empName, @empPassword, @empQualification, @empSpecilisation, @empEmail, @empPhone, @empAddress, @empRole )";
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

                    //2 te dhena nga forma register do i vendosim dhe tek tabela Login
                    SqlCommand cmd2 = db.con.CreateCommand();
                    cmd2.CommandText = "insert into tbLogin  (username, password, role) values (@username, @password, @role) ";
                    cmd2.Parameters.AddWithValue("@username", txtEmpId.Text);
                    cmd2.Parameters.AddWithValue("@password", txtPassword.Text);
                    cmd2.Parameters.AddWithValue("@role", this.role);
                    cmd2.ExecuteNonQuery();

                    MessageBox.Show("Employee details added succesfuly");
                  
                    populateEmployeeGrid();
                    typeOfSatistics = "Employee";
                    satisticsReportText = $"You added new employee with Id: {txtEmpId.Text}";
                    populateStatisticReport();
                    ClearTextBoxes();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        void ClearTextBoxes()
        {
            txtEmpId.Text = string.Empty;
            txtEmpName.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtEmpQualification.Text = string.Empty;
            txtEmplSpecialist.Text = string.Empty;
            txtEmpEmail.Text = string.Empty;
            txtEmpPhone.Text = string.Empty;
            txtEmpAddress.Text = string.Empty;
            chDoctor.Checked = false;
            chNurse.Checked = false;
        }

        private void btnAddEmploye_Click(object sender, EventArgs e)
        {

            VisibleFalsePanels();
            panelAddNewEmployee.Visible = true;
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            VisibleFalsePanels();

            panelViewEmployee.Visible = true;
          

        }
        private void btnHomeViewPatient_Click(object sender, EventArgs e)
        {

            VisibleFalsePanels();
            panelViewPatient.Visible = true;
          
        }

        private void chDoctor_CheckedChanged(object sender, EventArgs e)
        {
            role = chDoctor.Text;
        }

        private void chNurse_CheckedChanged(object sender, EventArgs e)
        {
            role = chNurse.Text;
        }

        void populateEmployeeGrid()
        {
            try
            {
                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from tb_EmployeeDetail";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                gridEmployee.DataSource = dt;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

     void populateEquipmentForServiceGrid()
        {
            try
            {
                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from tb_EquipmentDetail";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                gridEquipmentForService.DataSource = dt;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
       public void populatePatientGrid()
        {
            try
            {
                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from tbPatientDetail";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                gridPatient.DataSource = dt;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        void populateOnlineServicesGrid()
        {
            try
            {
                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from tb_ServiceDetail";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                gridViewOnlineServises.DataSource = dt;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

      void populateServiceGrid()
        {        
            try
            {
                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from tb_ServiceDetail";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                gridViewOnlineServises.DataSource= dt;
               
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnSearchEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                if (db.DbConnectionn().State == ConnectionState.Closed)
                {
                    db.DbConnectionn().Open();
                }
                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from tb_EmployeeDetail where empId like('%" + txtSearch.Text + "%') ";
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                gridEmployee.DataSource = dt;
           
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
                if (db.DbConnectionn().State == ConnectionState.Closed)
                {
                    db.DbConnectionn().Open();
                }
                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from tbPatientDetail where patientID like('%" + txtPatientSearch.Text + "%') ";
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                gridPatient.DataSource = dt;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }        
        void populateEmployeeCombo()
        {
            try
            {           
                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select empName from tb_EmployeeDetail";
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
               
                comboAllDoctors.DataSource = dt;
                comboEmployeeForEquipment.DataSource = dt;
                comboAllDoctors.DisplayMember = "empName";
                comboEmployeeForEquipment.DisplayMember = "empName";
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
           
        }
     
        void populateRoomCombo()
        {
            try
            {
                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select roomNum, roomFloor from tb_room";
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
             
                comboAllRooms.DataSource = dt;
                comboRoomEqupment.DataSource = dt;
                comboAllRooms.DisplayMember = "roomNum";
                comboRoomEqupment.DisplayMember = "roomNum";


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            VisibleFalsePanels();
            panelEquipment.Visible = true;           
        }
        private void btnViewOnlineService_Click(object sender, EventArgs e)
        {
            VisibleFalsePanels();
            panelOnlineServices.Visible = true;          
        }

        private void btnHomeAddRoom_Click(object sender, EventArgs e)
        {
            VisibleFalsePanels();
            panelAddRoom.Visible = true;
        }

        private void btAddService_Click_1(object sender, EventArgs e)
        {

            try
            {
                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }

                    //shtojme te dhenat tek table e serviceDetail nga register form
                    SqlCommand cmd = db.con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "insert into tb_ServiceDetail (serviceName, specialistDoctor, equipment, room, startingTime,endingTime, durationOfService) values (@serviceName, @specialistDoctor, @equipment, @room,@startingTime,@endingTime, @durationOfService)";
                    cmd.Parameters.AddWithValue("@serviceName", txtServiceName.Text);
                    cmd.Parameters.AddWithValue("@specialistDoctor", comboAllDoctors.Text);
                    cmd.Parameters.AddWithValue("@equipment", txtEquipment.Text);
                    cmd.Parameters.AddWithValue("@room", comboAllRooms.Text);
                    cmd.Parameters.AddWithValue("@startingTime", sTime.Text);
                    cmd.Parameters.AddWithValue("@endingTime", eTime.Text);
                    cmd.Parameters.AddWithValue("@durationOfService", txtDurationService.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Service details added succesfuly");
                    // ClearTextBoxes();
                    typeOfSatistics = "Service";
                    satisticsReportText = $"You added new service {(txtServiceName.Text)}";
                    populateStatisticReport();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnAddRoom_Click(object sender, EventArgs e)
        {
            try
            {
                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }
                    SqlCommand cmd = db.con.CreateCommand();
                    cmd.CommandType= CommandType.Text;
                    cmd.CommandText = "insert into tb_room (roomNum, roomFloor) values (@roomNum, @roomFloor)";
                    cmd.Parameters.AddWithValue("@roomNum", txtRoomNum.Text);
                    cmd.Parameters.AddWithValue("@roomFloor", txtRoomFloor.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Room details added successfuly");
                    typeOfSatistics = "Room";
                    satisticsReportText = $"You added new room, Room Num: {txtRoomNum.Text}, Room Floor: {txtRoomFloor.Text}";
                    populateStatisticReport();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnHomeLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void btnHomeEmail_Click(object sender, EventArgs e)
        {
            this.Hide();
            EmailForm email = new EmailForm();
            email.Show();
        }

        private void btnHomeAddService_Click(object sender, EventArgs e)
        {
            VisibleFalsePanels();
            panelAddNewService.Visible = true;
        }
     
        private void empStartingTime_ValueChanged(object sender, EventArgs e)
        {
            startingTime.Format = DateTimePickerFormat.Custom;
            startingTime.CustomFormat = "  hh:mm:ss";
            startingTime.ShowUpDown = true;
        }

        private void empEndingTime_ValueChanged(object sender, EventArgs e)
        {
            endingTime.Format = DateTimePickerFormat.Custom;
            endingTime.CustomFormat = "  hh:mm:ss";
            endingTime.ShowUpDown = true;
        }

        void fixTimeShow()
        {
            sTime.Format = DateTimePickerFormat.Custom;
            sTime.CustomFormat = "  hh:mm:ss";
            sTime.ShowUpDown = true;
            eTime.Format = DateTimePickerFormat.Custom;
            eTime.CustomFormat = "  hh:mm:ss";
            eTime.ShowUpDown = true;
            startingTime.Format = DateTimePickerFormat.Custom;
            startingTime.CustomFormat = "  hh:mm:ss";
            startingTime.ShowUpDown = true;
            endingTime.Format = DateTimePickerFormat.Custom;
            endingTime.CustomFormat = "  hh:mm:ss";
            endingTime.ShowUpDown = true;
        }
        private void sTime_ValueChanged(object sender, EventArgs e)
        {
            sTime.Format = DateTimePickerFormat.Custom;
            sTime.CustomFormat = "  hh:mm:ss";
            sTime.ShowUpDown = true;
        }

        private void eTime_ValueChanged(object sender, EventArgs e)
        {
            eTime.Format = DateTimePickerFormat.Custom;
            eTime.CustomFormat = "  hh:mm:ss";
            eTime.ShowUpDown = true;
        }

        private void btnAddEquipement_Click(object sender, EventArgs e)
        {
            try
            {
                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }
                    //shtojme te dhenat tek table e equpmentDetail nga register form
                    SqlCommand cmd = db.con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "insert into tb_EquipmentDetail (equipmentName, specialistEmployee, room, startingTime,endingTime, eqDurationOfService) values (@equipmentName, @specialistEmployee,  @room,@startingTime,@endingTime, @eqDurationOfService)";
                    cmd.Parameters.AddWithValue("@equipmentName", txtServiceName.Text);
                    cmd.Parameters.AddWithValue("@specialistEmployee", comboAllDoctors.Text);
                    cmd.Parameters.AddWithValue("@room", comboAllRooms.Text);
                    cmd.Parameters.AddWithValue("@startingTime", startingTime.Text);
                    cmd.Parameters.AddWithValue("@endingTime", endingTime.Text);
                    cmd.Parameters.AddWithValue("@eqDurationOfService", txtDuratEqService.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Equipment details added succesfuly");
                    // ClearTextBoxes();
                    populateEquipmentForServiceGrid();
                    typeOfSatistics = "Equipment";
                    satisticsReportText = $"You added new equipment {(txtServiceName.Text)}";
                    populateStatisticReport();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
 
        private void btnHomeEquipmentForService_Click(object sender, EventArgs e)
        {
            VisibleFalsePanels();
            panelEquipmentForService.Visible = true;
        }
     
        private void btnHomeAdmin_Click(object sender, EventArgs e)
        {
            VisibleFalsePanels();
            panelHome.Visible = true;
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
                    cmd.Parameters.AddWithValue("@userId", adminId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }
        void getSatisticsReportData()
        {
            SqlCommand cmd = db.DbConnectionn().CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select satisticsReport, dateTime  from tb_SatisticsReport where typeOfSatistics = @typeOfSatistics and userId =@userId ";
            cmd.Parameters.AddWithValue("typeOfSatistics", typeOfSatistics);
            cmd.Parameters.AddWithValue("userId", "admin");
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            dataGrgridSatisticsReport.DataSource = dt;
        }
        private void btnSatistics_Click(object sender, EventArgs e)
        {
            VisibleFalsePanels();
            panelSatisticsReport.Visible = true;
            dataGrgridSatisticsReport.Visible = false;
        }

        private void btnServiceSatistics_Click(object sender, EventArgs e)
        {
            typeOfSatistics = "Service";
            getSatisticsReportData();
            dataGrgridSatisticsReport.Visible = true;
        }

        private void btnEquipmentSatistics_Click(object sender, EventArgs e)
        {
            typeOfSatistics = "Equipment";
            getSatisticsReportData();
            dataGrgridSatisticsReport.Visible = true;
        }

        private void btnRoomSatisitcs_Click(object sender, EventArgs e)
        {
            typeOfSatistics = "Room";
            getSatisticsReportData();
            dataGrgridSatisticsReport.Visible = true;
        }

        private void btnEmployeeSatistics_Click(object sender, EventArgs e)
        {
            typeOfSatistics = "Employee";
            getSatisticsReportData();
            dataGrgridSatisticsReport.Visible = true;
        }

      
    }
}
