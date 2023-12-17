using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using FluentEmail.Core;
using FluentEmail.Smtp;
using System.Windows.Forms;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text;
using QRCoder;
using System.Threading;

using Image = System.Drawing.Image;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using ZXing;
using HospitalApp.Admin;
using Paragraph = iTextSharp.text.Paragraph;
using HospitalApp.DGVPrinterHelper;
using System.Net.Mail;
using System.Threading.Tasks;
using HospitalApp.Employee;

namespace HospitalApp.Patient
{
    public partial class PatientHomePage : Form
    {
        DbConnection db = new DbConnection();
        EmployeeHomePage emp = new EmployeeHomePage();

        string patientId = LoginForm.username;
        string patientFullName;
        int rowId;
        public static string bookedServiceDetail;
        string readBarcodeText;
        string typeOfSatistics;
        string satisticsReportText;
        string patientIdFromBooking;
        string patientEmail;
        string startingTime;
        string subjectEmail;
        string bodyEmail;
        string bootEmail;
        int rowIdForReschedle;
        bool reschedule = false;
        bool barcodeMatches = false;
        bool serviceBooking = false;
        bool equipmentBooking = false;
        bool serviceFilter = false;
        bool equipmentFilter = false;
        bool printAll = false;
        bool printByName = false;
        bool printByDate = false;
       
        public PatientHomePage()
        {
            InitializeComponent();
        }

        private void PacientHomePage_Load(object sender, EventArgs e)
        {
            updateGridSchedule();
            VisibleFalsePanels();           
            populateServiceGrid();        
            welcomeLabel();
            emp.getScheduleServicesOrEquipmentsThatPassedDeadlineOrFinished();
            rescheduleServiceEquipmentListNotifyPatient();
            alertForPatientDelays();
            alertForPatientDelaysEquipment();


        }
         
        void welcomeLabel()
        {
            panelHome.Visible = true;
            lblPatientHome.Text = patientId;
            lblWelcome.Visible = true;
            lblWelcome.Text = $"Welcome patient {patientId}";
        }
        public void FillScheduleGrid()
        {
            try
            {
                using (db.DbConnectionn())
                {
                   
                        if (db.DbConnectionn().State == ConnectionState.Closed)
                        {
                            db.DbConnectionn().Open();
                        }
                    List<EmployeScheduleList> list = new List<EmployeScheduleList>();
                    if (serviceBooking)
                    {

                       
                        DataTable dt = new DataTable();
                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "select * from tb_empScheduleList where service = @service  ORDER BY orderByStatus ASC ";
                        cmd.Parameters.AddWithValue("@service", comboSelectServiceForBooking.Text);


                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);




                        foreach (DataRow dr in dt.Rows)
                        {


                            if (dr["bookedBy"].ToString() == patientId || dr["bookedBy"].ToString() == "")
                            {

                                EmployeScheduleList empSchedule = new EmployeScheduleList();
                                empSchedule.Id = dr["Id"].ToString();
                                empSchedule.Service = dr["service"].ToString();
                                empSchedule.Doctor = dr["doctor"].ToString();
                                empSchedule.StartingTime = dr["startingTime"].ToString();
                                empSchedule.EndingTime = dr["endingTime"].ToString();
                                empSchedule.Duration = dr["duration"].ToString();
                                empSchedule.Status = dr["status"].ToString();
                                empSchedule.BookedBy = dr["bookedBy"].ToString();
                                list.Add(empSchedule);
                            }
                        }

                    }

                    else if(equipmentBooking)
                    {

                        DataTable dt = new DataTable();
                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "select * from tb_empScheduleListEquipment  where equipment = @equipment  ORDER BY orderByStatus ASC  ";
                        cmd.Parameters.AddWithValue("@equipment", comboSelectServiceForBooking.Text);


                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);




                        foreach (DataRow dr in dt.Rows)
                        {


                            if (dr["bookedBy"].ToString() == patientId || dr["bookedBy"].ToString() == "")
                            {

                                EmployeScheduleList empSchedule = new EmployeScheduleList();
                                empSchedule.Id = dr["Id"].ToString();
                                empSchedule.Service = dr["equipment"].ToString();
                                empSchedule.Doctor = dr["doctor"].ToString();
                                empSchedule.StartingTime = dr["startingTime"].ToString();
                                empSchedule.EndingTime = dr["endingTime"].ToString();
                                empSchedule.Duration = dr["duration"].ToString();
                                empSchedule.Status = dr["status"].ToString();
                                empSchedule.BookedBy = dr["bookedBy"].ToString();
                                list.Add(empSchedule);
                            }
                        }

                    }

                    gridPacientBooking.DataSource = list;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
   
        void VisibleFalsePanels()
        {
            
            panelPacientBooking.Visible = false;
            panelUpdatePatientProfile.Visible = false;
            panelViewHospitalServices.Visible = false;
            panelViewOnlineEquipment.Visible = false;
            panelHome.Visible = false;
            panelPrintBookedServOrEquip.Visible = false;
            panelSatisticsReport.Visible = false;
            lblWelcome.Visible = false;
        }
        private void btnHomeUpdateEmpProfile_Click(object sender, EventArgs e)
        {
            VisibleFalsePanels();
            
            FillTextBoxesForPatientUpdate();
            panelUpdatePatientProfile.Visible = true;
            
        }

        private void btnHomeLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void btnHomeBookVisited_Click(object sender, EventArgs e)
        {
            VisibleFalsePanels();
            panelPacientBooking.Visible = true;
          panenSecondForBooking.Visible = false;
        }
       
        void updateGridSchedule()
        {
           
            FillScheduleGrid();
            
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
                    cmd.CommandText = "select  email from tbPatientDetail where patientID = '" + patientId + "' ";
                   
                    patientEmail = (string)cmd.ExecuteScalar();
                    db.con.Close();
                }
                return patientEmail;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public  string  GetDetailBookedService()
        {
            //Detajet e mara nga kjo row do i perdorim si text per barcode ku do jene detajet e sherbimit te rezerevuar
            try
            {
            
                StringBuilder sb = new StringBuilder();
             //   getPatientFullNameById();
                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }
                    if(serviceBooking)
                    {
                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                        cmd.CommandType = CommandType.Text;
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
                            sb.Append(patientId).ToString();
                        }
                    }
                    else if (equipmentBooking)
                    {
                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "select * from tb_empScheduleListEquipment where Id = '" + rowId + "' ";
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        //  bookedServiceDetail = (string)cmd.ExecuteScalar().ToList();
                        // bookedServiceDetail = dt.ToString();


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
                            sb.Append(patientFullName).ToString();
                        }
                    }
                   
                     bookedServiceDetail = sb.ToString();
                }
                MessageBox.Show(bookedServiceDetail);
                return bookedServiceDetail;
                
            }
            catch (Exception)
            {

                throw;
            }

        }
       


        private void gridPacientBooking_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {     
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            int yourindexColumn = 8;
            int btnBook = 0;
            int btnCancel = 1;
            DataGridViewCell booklButtonRow = gridPacientBooking.Rows[e.RowIndex].Cells[btnBook];
            DataGridViewCell cancelButtonRow = gridPacientBooking.Rows[e.RowIndex].Cells[btnCancel];

            DataGridViewCell statusCell = gridPacientBooking.Rows[e.RowIndex].Cells[yourindexColumn];
            string value = statusCell.Value == null ? string.Empty : statusCell.Value.ToString();

            string firstColumn = booklButtonRow.Value == null ? string.Empty : booklButtonRow.Value.ToString();
            string secondColumn = cancelButtonRow.Value == null ? string.Empty : cancelButtonRow.Value.ToString();


            if (value.ToLower().Equals("unbooked") == true)
            {
                gridPacientBooking.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.CornflowerBlue;
                dataGridViewCellStyle.Padding = new Padding(0, 0, 1000, 0);
                cancelButtonRow.Style = dataGridViewCellStyle;
                cancelButtonRow.ReadOnly = true;
            }

            else if (value.ToLower().Equals("booked") == true)
            {
                gridPacientBooking.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                dataGridViewCellStyle.Padding = new Padding(0, 0, 1000, 0);
                booklButtonRow.Style = dataGridViewCellStyle;
                booklButtonRow.ReadOnly = true;
            }

            else if (value.ToLower().Equals("started") == true)
            {
                gridPacientBooking.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                dataGridViewCellStyle.Padding = new Padding(0, 0, 1000, 0);
                booklButtonRow.Style = dataGridViewCellStyle;
                booklButtonRow.ReadOnly = true;
                cancelButtonRow.Style = dataGridViewCellStyle;

            }
            else if (value.ToLower().Equals("cancel") == true)
            {
                gridPacientBooking.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                dataGridViewCellStyle.Padding = new Padding(0, 0, 1000, 0);
                booklButtonRow.Style = dataGridViewCellStyle;
                booklButtonRow.ReadOnly = true;
                cancelButtonRow.Style = dataGridViewCellStyle;
                cancelButtonRow.ReadOnly = true;
            }
            else if (value.ToLower().Equals("deadline passed") == true)
            {
                gridPacientBooking.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Gray;
                dataGridViewCellStyle.Padding = new Padding(0, 0, 1000, 0);
                booklButtonRow.Style = dataGridViewCellStyle;
                booklButtonRow.ReadOnly = true;
                cancelButtonRow.Style = dataGridViewCellStyle;
                cancelButtonRow.ReadOnly = true;
            }
            else if (value.ToLower().Equals("finished") == true)
            {
                gridPacientBooking.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.PowderBlue;
                dataGridViewCellStyle.Padding = new Padding(0, 0, 1000, 0);
                booklButtonRow.Style = dataGridViewCellStyle;
                booklButtonRow.ReadOnly = true;
                cancelButtonRow.Style = dataGridViewCellStyle;
                cancelButtonRow.ReadOnly = true;
            }
        }

        string  getPatientFullNameById()
        {
            try
            {
                using(db.DbConnectionn())
                {

                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select patientName from tbPatientDetail where patientID = '" + patientId + "'";

                    patientFullName = (string)cmd.ExecuteScalar();
                }
                return patientFullName;
            }
            catch (Exception)
            {
                throw;
            }
        }
     
        public void print()
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
        public void generateBarcode()
        {
            getPatientFullNameById();
            Image img;
            string ticketHeaderText = $"Hello {patientFullName} please use barcode as a ticket for service that you have booked!";
            string create = bookedServiceDetail;
            QRCodeGenerator qrCode = new QRCodeGenerator();
            PictureBox picBarcode = new PictureBox();
            QRCodeData data = qrCode.CreateQrCode(create, QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);
            picBarcode.Image = code.GetGraphic(5);
            img = picBarcode.Image;

          SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPG(*.JPG)|*.jpg";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                img.Save(saveFileDialog.FileName);
            }

            string pathForImageAndPdfTicket = saveFileDialog.FileName;

            int i = 0;
           

            string stdDir2 =  string.Concat(pathForImageAndPdfTicket.Reverse().Skip(4).Reverse());
            string stdDir1 = stdDir2 + ".pdf";

            while (File.Exists(stdDir1))
            {
                i++;
                stdDir1 = stdDir2 + i.ToString() + ".pdf";

            }


            Document pdoc = new Document(PageSize.A4, 20f, 20f, 30f, 30f);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdoc, new FileStream(stdDir1, FileMode.CreateNew));
            pdoc.Open();
            System.Drawing.Image pImage = picBarcode.Image;
            
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(pImage, System.Drawing.Imaging.ImageFormat.Jpeg);
            pdoc.Add(image);
            image.Alignment = Element.ALIGN_CENTER;

            iTextSharp.text.Font headerFont = FontFactory.GetFont(iTextSharp.text.Font.FontFamily.TIMES_ROMAN.ToString(), 20,
                iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE);
            Paragraph p1 = new Paragraph(ticketHeaderText, headerFont);
            p1.Alignment = Element.ALIGN_CENTER;
            pdoc.Add(p1);
            pdoc.Close();
        }

      public  string DecodeBarcode()
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
           
;
        }  

        void compareBarcodeWithBookedServiceDetail()
        {
            try
            {
                DecodeBarcode();
                if (readBarcodeText == bookedServiceDetail)
                {
                    lblBookingsByPatient.Text = string.Empty;
                    lblBookingsByPatient.Text = "Barcode that you import was correct, service unbooked!";
                    lblBookingsByPatient.ForeColor = Color.LightGreen;
                    barcodeMatches = true;
                }
                else
                {
                    lblBookingsByPatient.Text = string.Empty;
                    lblBookingsByPatient.Text = "Barcode that you import was not correct, please import the barcode you saved during booking!";
                    lblBookingsByPatient.ForeColor = Color.Red;
                    barcodeMatches = false;
                    return;
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        void FillTextBoxesForPatientUpdate()
        {
            try
            {
                using (db.DbConnectionn())
                {
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select * from tbPatientDetail where  patientID ='" + patientId + "'";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        txtPatientId.Text = dr["patientId"].ToString();
                        txtPFullName.Text = dr["patientName"].ToString();
                        txtPatientPass.Text = dr["password"].ToString();
                        txtPatientAge.Text = dr["age"].ToString();
                        txtPatientEmail.Text = dr["email"].ToString();
                        txtPatientPo.Text = dr["phoneNo"].ToString();
                        txtPatientAddress.Text = dr["address"].ToString();
                        //chDoctor.Checked = dr["empRole"].ToString();
                        //chNurse.Checked = dr["empId"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnUpdatePatientProfile_Click(object sender, EventArgs e)
        {

            try
            {
                using (db.DbConnectionn())
                {
                    db.DbConnectionn().Open();
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "update tbPatientDetail set patientID = @patientID, patientName = @patientName, password =@password, age = @age, email = @email, phoneNo = @phoneNo, address = @address where patientID ='" + patientId + "' ";
                    cmd.Parameters.AddWithValue("@patientID", txtPatientId.Text);
                    cmd.Parameters.AddWithValue("@patientName", txtPFullName.Text);
                    cmd.Parameters.AddWithValue("@password", txtPatientPass.Text);
                    cmd.Parameters.AddWithValue("@age", txtPatientAge.Text);
                    cmd.Parameters.AddWithValue("@email", txtPatientEmail.Text);
                    cmd.Parameters.AddWithValue("@phoneNo", txtPatientPo.Text);
                    cmd.Parameters.AddWithValue("@address", txtPatientAddress.Text);

                    cmd.ExecuteNonQuery();

                    SqlCommand cmd2 = db.con.CreateCommand();
                    cmd2.CommandText = "update tbLogin set username = @username, password = @password where username ='" + patientId + "'  ";
                    cmd2.Parameters.AddWithValue("@username", txtPatientId.Text);
                    cmd2.Parameters.AddWithValue("@password", txtPatientPass.Text);
                    cmd2.ExecuteNonQuery();

                    lblUpdateProfile.Text =  "Your profile is updated!";
                    lblUpdateProfile.ForeColor = Color.Green;
                    satisticsReportText = "You updated your profile";
                    typeOfSatistics = "Update Profile";
                    populateStatisticReport();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       public void populateServiceCombo()
        {
            try
            {
                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select serviceName from tb_ServiceDetail";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                comboSearchForService.DataSource = dt;
                comboSearchForService.DisplayMember = "serviceName";
                comboServicePrint.DataSource = dt;
                comboServicePrint.DisplayMember = "serviceName";
                comboSelectServiceForBooking.DataSource = dt;
                comboSelectServiceForBooking.DisplayMember = "serviceName";
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
                gridPatientViewOnlineServices.DataSource = dt;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       public void populateEquipmentCombo()
        {
            try
            {
                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select equipmentName from tb_EquipmentDetail";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                comboSearchForService.DataSource = dt;
                comboSearchForService.DisplayMember = "equipmentName";
                comboSelectServiceForBooking.DataSource = dt;
                comboSelectServiceForBooking.DisplayMember = "equipmentName";
                comboViewOnlineEquipment.DataSource = dt;
                comboViewOnlineEquipment.DisplayMember = "equipmentName";
                comboServicePrint.DataSource = dt;  
                comboServicePrint.DisplayMember = "equipmentName";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        void populateEquipmentGrid()
        {

            try
            {           
                    if (equipmentFilter)

                    {
                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "select * from tb_EquipmentDetail where equipmentName like( '%"+ comboViewOnlineEquipment.Text + "%')";
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                        gridViewOnlineEquipment.DataSource = dt;
                    }

                    else

                    {
                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "select * from tb_EquipmentDetail";
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                        gridViewOnlineEquipment.DataSource = dt;
                    }         

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearchService_Click_1(object sender, EventArgs e)
        {
            try
            {
                SqlCommand cmd = db.DbConnectionn().CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from tb_ServiceDetail where serviceName like('%" + comboSearchForService.Text + "%') ";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                gridPatientViewOnlineServices.DataSource = dt;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHomeViewPatientProfile_Click(object sender, EventArgs e)
        {
 
            VisibleFalsePanels();
            panelViewHospitalServices.Visible = true;
            populateServiceCombo();
        }

        private void btnHomeEmailEmp_Click(object sender, EventArgs e)
        {
            EmailForm email = new EmailForm();
            email.Show();
        }

        private void btnServiceChoosed_Click(object sender, EventArgs e)
        {
          
            serviceBooking = true;
            equipmentBooking = false;
            panenSecondForBooking.Visible = true;
            gridPacientBooking.Visible = false;
            populateServiceCombo();
        }

        private void btnEquipmentChoosed_Click(object sender, EventArgs e)
        {
            equipmentBooking = true;
            serviceBooking = false;
            panenSecondForBooking.Visible = true;
            gridPacientBooking.Visible = false;
            populateEquipmentCombo();
        }

      

        private void btnHomeViewEquipmentForServices_Click(object sender, EventArgs e)
        {
            VisibleFalsePanels();
            panelViewOnlineEquipment.Visible = true;
            populateEquipmentCombo();
            populateEquipmentGrid();

        }

        private void btnEquipmentFilter_Click(object sender, EventArgs e)
        {
            equipmentFilter = true;
            populateEquipmentGrid();
           
        }

       

        void pupulateGridWithFilterData()
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

                            cmd.CommandText = "select * from tb_empScheduleList where bookedBy = @bookedBy AND status = @status ";
                            cmd.Parameters.AddWithValue("@bookedBy", patientId);
                            cmd.Parameters.AddWithValue("@status", "Booked");

                            DataTable dt = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dt);
                            gridPrint.DataSource = dt;
                        }

                        else if (printByName)
                        {


                            cmd.CommandText = "select * from tb_empScheduleList where bookedBy = @bookedBy AND status = @status AND service = @service ";
                            cmd.Parameters.AddWithValue("@bookedBy", patientId);
                            cmd.Parameters.AddWithValue("@status", "Booked");
                            cmd.Parameters.AddWithValue("@service", comboServicePrint.Text);

                            DataTable dt = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dt);
                            gridPrint.DataSource = dt;
                        }

                        else if (printByDate)
                        {

                            cmd.CommandText = "select * from tb_empScheduleList where bookedBy = @bookedBy AND status = @status ";
                            cmd.Parameters.AddWithValue("@bookedBy", patientId);
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

                        }
                    }  
                    
                    if(equipmentFilter)
                    {
                        if (printAll)
                        {

                            cmd.CommandText = "select * from tb_empScheduleListEquipment where bookedBy = @bookedBy AND status = @status ";
                            cmd.Parameters.AddWithValue("@bookedBy", patientId);
                            cmd.Parameters.AddWithValue("@status", "Booked");

                            DataTable dt = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dt);
                            gridPrint.DataSource = dt;
                        }

                        else if (printByName)
                        {


                            cmd.CommandText = "select * from tb_empScheduleListEquipment where bookedBy = @bookedBy AND status = @status AND equipment = @equipment ";
                            cmd.Parameters.AddWithValue("@bookedBy", patientId);
                            cmd.Parameters.AddWithValue("@status", "Booked");
                            cmd.Parameters.AddWithValue("@equipment", comboServicePrint.Text);

                            DataTable dt = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dt);
                            gridPrint.DataSource = dt;
                        }

                        else if (printByDate)
                        {

                            cmd.CommandText = "select * from tb_empScheduleListEquipment where bookedBy = @bookedBy AND status = @status ";
                            cmd.Parameters.AddWithValue("@bookedBy", patientId);
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

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        


        public void fixDateTimeFormat()
        {
            stTimePrint.Format = DateTimePickerFormat.Custom;
            stTimePrint.CustomFormat = "MM/dd/yyyy     hh:mm:ss";
            stTimePrint.ShowUpDown = true;

            endTimePrint.Format = DateTimePickerFormat.Custom;
            endTimePrint.CustomFormat = "MM/dd/yyyy     hh:mm:ss";
            endTimePrint.ShowUpDown = true;
        }
        private void btnSearchServiceForBooking_Click(object sender, EventArgs e)
        {
            
                    
              FillScheduleGrid();
            gridPacientBooking.Visible = true;
              
        }

        private void gridPacientBooking_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                getPatientFullNameById();
                string i = gridPacientBooking.Rows[e.RowIndex].Cells["Id"].FormattedValue.ToString();
                rowId = Convert.ToInt32(i);
                DataGridViewButtonColumn dataGridViewButtonColumn = new DataGridViewButtonColumn();
                gridPacientBooking.CurrentRow.Selected = true;
                int selectedColumn = gridPacientBooking.CurrentCell.ColumnIndex;

                int btnBook = 0;
                int btnCancel = 1;
                DataGridViewCell booklButtonRow = gridPacientBooking.Rows[e.RowIndex].Cells[btnBook];
                DataGridViewCell cancelButtonRow = gridPacientBooking.Rows[e.RowIndex].Cells[btnCancel];
                if (booklButtonRow.ColumnIndex == selectedColumn)
                {
                    if (MessageBox.Show("Are you sure want to book this service ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
                        int statusColumn = 8;
                        DataGridViewCell statusCell = gridPacientBooking.Rows[e.RowIndex].Cells[statusColumn];
                        string value = statusCell.Value == null ? string.Empty : statusCell.Value.ToString();
                        GetDetailBookedService();
                        generateBarcode();
                        using (db.DbConnectionn())
                        {

                            if (db.DbConnectionn().State == ConnectionState.Closed)
                            {
                                db.DbConnectionn().Open();
                            }
                            if (serviceBooking)
                            {


                                if (value.ToLower().Equals("unbooked") == true)
                                {
                                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = "update tb_empScheduleList set status = @status,bookedBy = @bookedBy,orderByStatus= @orderByStatus  where Id='" + rowId + "'";

                                    cmd.Parameters.AddWithValue("@status", SqlDbType.VarChar).Value = "Booked";
                                    cmd.Parameters.AddWithValue("@bookedBy", patientId);
                                    cmd.Parameters.AddWithValue("@orderByStatus", 2);
                                    cmd.ExecuteNonQuery();

                                    db.DbConnectionn().Close();


                                    MessageBox.Show("Service Booked! Barcode picture and pdf file was created in your file");
                                    satisticsReportText = $"You book service with details: {bookedServiceDetail}";
                                    lblBookingsByPatient.Text = "You Booked a service.";
                                    lblBookingsByPatient.ForeColor = Color.Green;
                                }
                            }

                            else if (equipmentBooking)
                            {

                                if (value.ToLower().Equals("unbooked") == true)
                                {
                                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandText = "update tb_empScheduleListEquipment set status = @status,bookedBy = @bookedBy ,orderByStatus= @orderByStatus  where Id='" + rowId + "'";

                                    cmd.Parameters.AddWithValue("@status", SqlDbType.VarChar).Value = "Booked";
                                    cmd.Parameters.AddWithValue("@bookedBy", patientId);
                                    cmd.Parameters.AddWithValue("@orderByStatus", 2);
                                    cmd.ExecuteNonQuery();

                                    db.DbConnectionn().Close();


                                    MessageBox.Show("Equipment Booked! Barcode was created in yout file");
                                    satisticsReportText = $"You book equipment with details: {bookedServiceDetail}";
                                    lblBookingsByPatient.Text = "You Booked a equipment.";
                                    lblBookingsByPatient.ForeColor = Color.Green;
                                }
                            }
                        }
                        typeOfSatistics = "Booking";
                        populateStatisticReport();
                    }
                   
                }
                else if (cancelButtonRow.ColumnIndex == selectedColumn)
                {
                    if (MessageBox.Show("Are you sure want to cancel this service ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        compareBarcodeWithBookedServiceDetail();
                        if (barcodeMatches)
                        {
                            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
                            int statusColumn = 8;
                            DataGridViewCell statusCell = gridPacientBooking.Rows[e.RowIndex].Cells[statusColumn];
                            string value = statusCell.Value == null ? string.Empty : statusCell.Value.ToString();
                            using (db.DbConnectionn())
                            {
                                if (db.DbConnectionn().State == ConnectionState.Closed)
                                {
                                    db.DbConnectionn().Open();
                                }

                                if (value.ToLower().Equals("booked") == true)
                                {
                                    if (serviceBooking)
                                    {
                                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                                        cmd.CommandType = CommandType.Text;
                                        cmd.CommandText = "update tb_empScheduleList set status = @status ,orderByStatus= @orderByStatus  where Id='" + rowId + "'";

                                        cmd.Parameters.AddWithValue("@status", SqlDbType.VarChar).Value = "Unbooked";
                                        cmd.Parameters.AddWithValue("@orderByStatus", 3);
                                        cmd.ExecuteNonQuery();
                                        db.DbConnectionn().Close();
                                        satisticsReportText = $"You Unbooked service with details: {bookedServiceDetail}";
                                        lblBookingsByPatient.Text = "You Unbooked a service.";
                                        lblBookingsByPatient.ForeColor = Color.Green;
                                    }
                                    else if (equipmentBooking)
                                    {
                                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                                        cmd.CommandType = CommandType.Text;
                                        cmd.CommandText = "update tb_empScheduleListEquipment set status = @status,orderByStatus= @orderByStatus  where Id='" + rowId + "'";

                                        cmd.Parameters.AddWithValue("@status", SqlDbType.VarChar).Value = "Unbooked";
                                        cmd.Parameters.AddWithValue("@orderByStatus", 3);
                                        cmd.ExecuteNonQuery();
                                        db.DbConnectionn().Close();
                                        satisticsReportText = $"You Unbooked equipment with details: {bookedServiceDetail}";
                                        lblBookingsByPatient.Text = "You Unbooked a service.";
                                        lblBookingsByPatient.ForeColor = Color.Green;

                                    }
                                    typeOfSatistics = "Booking";
                                    populateStatisticReport();
                                    cancellationReport();

                                }
                               
                            }

                        }
                       
                    }
                }
                updateGridSchedule();                       
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }    
        private void btnHomePrintFilterList_Click(object sender, EventArgs e)
        {
            VisibleFalsePanels();
            panelPrintBookedServOrEquip.Visible = true;
            secondPanelPrint.Visible = false;
            panelthirdFilterByDate.Visible = false;
            panelthirdFilterByName.Visible = false; 
        }

        private void btnFilterByName_Click(object sender, EventArgs e)
        {
          
            panelthirdFilterByName.Visible=true;
            panelthirdFilterByDate.Visible=false;
            btnPrintByName.Visible = false;
            btnPrintByName.Visible=false;
            if (serviceFilter)
            {
                populateServiceCombo();
            }
            else
            {
                populateEquipmentCombo();   
            }         
        }     
        private void printService_Click(object sender, EventArgs e)
        {
            serviceFilter = true;
            equipmentFilter = false;
            secondPanelPrint.Visible = true;
            panelthirdFilterByName.Visible=false;
            printAllResults.Visible = false;
        }
        private void printEquipment_Click(object sender, EventArgs e)
        {
            equipmentFilter = true;
            serviceFilter = false;
            secondPanelPrint.Visible = true;
            panelthirdFilterByName.Visible = false;
            printAllResults.Visible = false;
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
                    cmd.Parameters.AddWithValue("@userId", patientId);
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
            cmd.Parameters.AddWithValue("userId", patientId);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            dataGrgridSatisticsReport.DataSource = dt;
        }
        private void btnProfileSatistics_Click(object sender, EventArgs e)
        {
            typeOfSatistics = "Update Profile";
            getSatisticsReportData();
            dataGrgridSatisticsReport.Visible = true;
        }
        private void btnBookingSatistics_Click(object sender, EventArgs e)
        {
            typeOfSatistics = "Booking";
            getSatisticsReportData();
            dataGrgridSatisticsReport.Visible = true;
        }

        private void btnSatistics_Click(object sender, EventArgs e)
        {
            VisibleFalsePanels();
            panelSatisticsReport.Visible = true;
            dataGrgridSatisticsReport.Visible = false;
        }

        private void btnPrintSatisitcs_Click(object sender, EventArgs e)
        {
            typeOfSatistics = "Print";
            getSatisticsReportData();
            dataGrgridSatisticsReport.Visible = true;
        }

        private void btnHomeAdmin_Click(object sender, EventArgs e)
        {
            VisibleFalsePanels();
            panelHome.Visible = true;
        }
  
        private void btnGetAllBooked_Click(object sender, EventArgs e)
        {
            printAll = true;
            printByDate = false;
            printByName = false;
            printAllResults.Visible = true;
            pupulateGridWithFilterData();
        }

        private void btnGetByTime_Click(object sender, EventArgs e)
        {
           
            panelthirdFilterByDate.Visible = true;
            panelthirdFilterByName.Visible = false;
            btnPrintByDate.Visible=false;
            fixDateTimeFormat();
        }

        private void btnSearchByName_Click(object sender, EventArgs e)
        {
            printByName = true;
            printAll = false;
            printByDate = false;
            pupulateGridWithFilterData();
            btnPrintByName.Visible = true;
        }

        private void btnSearchBookedByTime_Click(object sender, EventArgs e)
        {
            printByDate = true;
            printAll = false;
            printByName = false;
            pupulateGridWithFilterData();
            btnPrintByDate.Visible = true;
        }

        private void printAllResults_Click(object sender, EventArgs e)
        {
            print();

        }

        private void btnPrintByName_Click(object sender, EventArgs e)
        {
            print();
        }

        private void btnPrintByDate_Click(object sender, EventArgs e)
        {
            print();
        }
      void priorityReservationEquipment()
        {
            try
            {
                bool noRows = false;
                DateTime stTimeForReschedule = default;
                DateTime currentTime = DateTime.UtcNow;
              



                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT TOP 1 * FROM tb_empScheduleListEquipment where status= @status ORDER BY orderByStatus ";
                    cmd.Parameters.AddWithValue("status", "Booked");

                    SqlDataReader rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        if (rd.Read())
                        {
                            patientIdFromBooking = rd[7].ToString();
                            rowIdForReschedle = Convert.ToInt32(rd[0]);
                            stTimeForReschedule = Convert.ToDateTime(rd[3]);                        }
                    }
                    else
                    {
                        noRows = true;
                    }

                    rd.Close();
                    if (noRows == false)
                    {
                        SqlCommand cmd2 = db.DbConnectionn().CreateCommand();
                        cmd2.CommandType = CommandType.Text;
                        cmd2.CommandText = "SELECT email FROM tbPatientDetail where patientId = @patientId ";
                        cmd2.Parameters.AddWithValue("patientId", patientIdFromBooking);

                        SqlDataReader rd2 = cmd2.ExecuteReader();
                        if (rd2.HasRows)
                        {
                            if (rd2.Read())
                            {
                                patientEmail = rd2[0].ToString();
                              
                            }
                        }

                        rd2.Close();
                        reschedule = true;
                        equipmentBooking = true;
                    }
                    TimeSpan ts = currentTime - stTimeForReschedule;
                    double hoursDiff = ts.TotalHours;
                   // DateTime tsTime = Convert.ToDateTime(ts);
                    if (patientIdFromBooking == patientId && 0 < hoursDiff && hoursDiff < 2.00)
                    {
                        sendUserAutomaticEmail();
                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void priorityReservation()
        {
            try
            {
                bool noRows = false;
                DateTime stTimeForReschedule = default;
                DateTime currentTime = DateTime.UtcNow;
                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }
                    SqlCommand cmd = db.DbConnectionn().CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT TOP 1 * FROM tb_empScheduleList where status= @status ORDER BY ID ";
                    cmd.Parameters.AddWithValue("status", "Booked");

                    SqlDataReader rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        if (rd.Read())
                        {
                            patientIdFromBooking = rd[7].ToString();
                             rowIdForReschedle = Convert.ToInt32(rd[0]);
                            stTimeForReschedule = Convert.ToDateTime(rd[3]);
                        }
                    }
                    else
                    {
                        noRows = true;
                    }
                    rd.Close();
                    if (noRows == false)
                    {                      
                        SqlCommand cmd2 = db.DbConnectionn().CreateCommand();
                        cmd2.CommandType = CommandType.Text;
                        cmd2.CommandText = "SELECT email  FROM tbPatientDetail where patientId = @patientId ";
                        cmd2.Parameters.AddWithValue("patientId", patientIdFromBooking);

                        SqlDataReader rd2 = cmd2.ExecuteReader();
                        if (rd2.HasRows)
                        {
                            if (rd2.Read())
                            {
                                patientEmail = rd2[0].ToString();
                            }
                        }

                        rd2.Close();
                        reschedule = true;
                        serviceBooking = true;
                        equipmentBooking = false;
                        TimeSpan ts = currentTime - stTimeForReschedule;
                        double hoursDiff = ts.TotalHours;

                        if (patientIdFromBooking == patientId && 0 < hoursDiff && hoursDiff < 2.00)
                        {
                            sendUserAutomaticEmail();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void sendUserAutomaticEmail()
        {
            var senderEmail = new SmtpSender(() => new SmtpClient("localhost")
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Port = 25
            });
            if (reschedule)
            {

                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }
                    SqlCommand cmd2 = db.DbConnectionn().CreateCommand();
                    cmd2.CommandType = CommandType.Text;
                    if(equipmentBooking)
                    {
                        cmd2.CommandText = "SELECT startingTime  FROM tb_empScheduleListEquipment where Id = @rowIdForReschedle ";
                    }
                    else
                    {
                        cmd2.CommandText = "SELECT startingTime  FROM tb_empScheduleList where Id = @rowIdForReschedle; ";
                    }

                    cmd2.Parameters.AddWithValue("rowIdForReschedle", rowIdForReschedle);

                    SqlDataReader rd2 = cmd2.ExecuteReader();
                    rd2.Read();
                    startingTime = rd2[0].ToString();
                    rd2.Close();
                }
                subjectEmail = "Be ready for your appointment!";
                bodyEmail = $"Hello {patientIdFromBooking} your service/equipment start at {startingTime}. Please be in time. Thank you!";
                bootEmail = "smartBoot@gmail.com";
            }
            else
            {
                getPatientEmail();
                subjectEmail = "Report!!";
                bodyEmail = $"Hello {patientId} You have done several cancellations, please be sure before booking a service/equipment!";
                bootEmail = "smartBoot@gmail.com";
            }
            Email.DefaultSender = senderEmail;
            var email = Email

                .From(bootEmail)
                .To(patientEmail)
                .Subject(subjectEmail)
                .Body(bodyEmail)
                .Send();
        }

        void rescheduleServiceEquipmentListNotifyPatient()
        {      
                priorityReservationEquipment();
                priorityReservation();           
        }

        void cancellationReport()
        {
            try
            {
               
                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }

                    SqlCommand cmd1 = db.DbConnectionn().CreateCommand();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.CommandText = "select numberOfCancellations from tb_CancellationsReport where patientId = @patientId ";
                    cmd1.Parameters.AddWithValue("patientId", patientId);
                    SqlDataReader rd = cmd1.ExecuteReader();

                    //string numberOfCanecllations = rd[2].ToString();
                    int cancel =default;
                    bool readerClosed = false;
                    if (rd.HasRows)
                    {
                       if  (rd.Read())
                       {
                            int numberOfCanecllations = Convert.ToInt32(rd[0]);
                            if (!DBNull.Value.Equals(rd[0]) )
                            {
                                if(numberOfCanecllations == 3)
                                {
                                    MessageBox.Show("You have done several cancellations, please be sure before booking a service/equipment", "Attention",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    rd.Close();
                                    reschedule = false;
                                    sendUserAutomaticEmail();
                                    cancel = 0;
                                     readerClosed = true;
                                }
                                else
                                {
                                    cancel = numberOfCanecllations + 1;
                                }

                            }                                                  
                       }
                    }
                    else
                    {
                        cancel = 0;
                    }
                                   
                    if(readerClosed == false)
                    {
                        rd.Close();
                    }         
                    using(db.DbConnectionn())
                    {
                        if (db.DbConnectionn().State == ConnectionState.Closed)
                        {
                            db.DbConnectionn().Open();
                        }
                        SqlCommand cmd = db.DbConnectionn().CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "update  tb_CancellationsReport set numberOfCancellations = @numberOfCancellations where patientId = @patientId ";
                        cmd.Parameters.AddWithValue("numberOfCancellations", cancel);
                        cmd.Parameters.AddWithValue("patientId", patientId);

                        cmd.ExecuteNonQuery();
                    }
                   
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

      private async void  alertForPatientDelays()
        {
            try
            {
                bool alertDelay =   false;
                DateTime stTime = default;
                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }

                    SqlCommand cmd1 = db.DbConnectionn().CreateCommand();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.CommandText = "select startingTime from tb_empScheduleList where bookedBy = @patientId and status = @status";
                    cmd1.Parameters.AddWithValue("patientId", patientId);
                    cmd1.Parameters.AddWithValue("status", "Booked");
                    SqlDataReader rd = cmd1.ExecuteReader();
                                          
                    if (rd.HasRows)
                    {
                        if (rd.Read())
                        {
                            stTime = Convert.ToDateTime(rd[0]);
                            if (stTime <= DateTime.UtcNow)
                            {

                                alertDelay = true;
                            }
                        }                      
                    }
                    rd.Close();
                    
                    while(alertDelay)
                    {                    
                        MessageBox.Show("Your reservation has started, please don't be more late for your appointment!!", "Attention",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        await Task.Run(() => LongRunningTask());
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void alertForPatientDelaysEquipment()
        {
            try
            {               
                bool alertDelay = false;
                DateTime stTime = default;
                using (db.DbConnectionn())
                {
                    if (db.DbConnectionn().State == ConnectionState.Closed)
                    {
                        db.DbConnectionn().Open();
                    }
                    SqlCommand cmd1 = db.DbConnectionn().CreateCommand();
                    cmd1.CommandType = CommandType.Text;
                    cmd1.CommandText = "select startingTime from tb_empScheduleListEquipment where bookedBy = @patientId and status = @status";
                    cmd1.Parameters.AddWithValue("patientId", patientId);
                    cmd1.Parameters.AddWithValue("status", "Booked");
                    SqlDataReader rd = cmd1.ExecuteReader();

                    if (rd.HasRows)
                    {
                        if (rd.Read())
                        {
                            stTime = Convert.ToDateTime(rd[0]);
                            if (stTime <= DateTime.UtcNow)
                            {
                                alertDelay = true;
                            }
                        }

                    }
                    rd.Close();

                    while (alertDelay)
                    {

                        MessageBox.Show("Your reservation has started, please don't be more late for your appointment!!", "Attention",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        await Task.Run(() => LongRunningTask());

                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LongRunningTask()
        {
            TimeSpan ts = new TimeSpan(0, 5, 0);
            Thread.Sleep(ts);
        }

    }
}
