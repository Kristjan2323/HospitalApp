namespace HospitalApp.Admin
{
    partial class EmailForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnFormHomeSendEmail = new System.Windows.Forms.Button();
            this.btnHomeInbox = new System.Windows.Forms.Button();
            this.btbBackToAdminForm = new System.Windows.Forms.Button();
            this.panelSendEmail = new System.Windows.Forms.Panel();
            this.label29 = new System.Windows.Forms.Label();
            this.btnSendEmail = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSubject = new System.Windows.Forms.TextBox();
            this.Subject = new System.Windows.Forms.Label();
            this.txtSendTo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelInbox = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.txtShowRecieved = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.gridRecieveEmail = new System.Windows.Forms.DataGridView();
            this.panelSendEmail.SuspendLayout();
            this.panelInbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridRecieveEmail)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFormHomeSendEmail
            // 
            this.btnFormHomeSendEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFormHomeSendEmail.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnFormHomeSendEmail.Location = new System.Drawing.Point(12, 71);
            this.btnFormHomeSendEmail.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnFormHomeSendEmail.Name = "btnFormHomeSendEmail";
            this.btnFormHomeSendEmail.Size = new System.Drawing.Size(203, 44);
            this.btnFormHomeSendEmail.TabIndex = 8;
            this.btnFormHomeSendEmail.Text = "Send Email";
            this.btnFormHomeSendEmail.UseVisualStyleBackColor = true;
            this.btnFormHomeSendEmail.Click += new System.EventHandler(this.btnFormHomeSendEmail_Click);
            // 
            // btnHomeInbox
            // 
            this.btnHomeInbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHomeInbox.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnHomeInbox.Location = new System.Drawing.Point(12, 159);
            this.btnHomeInbox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnHomeInbox.Name = "btnHomeInbox";
            this.btnHomeInbox.Size = new System.Drawing.Size(203, 44);
            this.btnHomeInbox.TabIndex = 9;
            this.btnHomeInbox.Text = "Inbox";
            this.btnHomeInbox.UseVisualStyleBackColor = true;
            this.btnHomeInbox.Click += new System.EventHandler(this.btnHomeInbox_Click);
            // 
            // btbBackToAdminForm
            // 
            this.btbBackToAdminForm.Location = new System.Drawing.Point(21, 595);
            this.btbBackToAdminForm.Name = "btbBackToAdminForm";
            this.btbBackToAdminForm.Size = new System.Drawing.Size(130, 37);
            this.btbBackToAdminForm.TabIndex = 75;
            this.btbBackToAdminForm.Text = "Back";
            this.btbBackToAdminForm.UseVisualStyleBackColor = true;
            this.btbBackToAdminForm.Click += new System.EventHandler(this.btbBackToAdminForm_Click);
            // 
            // panelSendEmail
            // 
            this.panelSendEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSendEmail.Controls.Add(this.label29);
            this.panelSendEmail.Controls.Add(this.btnSendEmail);
            this.panelSendEmail.Controls.Add(this.txtMessage);
            this.panelSendEmail.Controls.Add(this.label2);
            this.panelSendEmail.Controls.Add(this.txtSubject);
            this.panelSendEmail.Controls.Add(this.Subject);
            this.panelSendEmail.Controls.Add(this.txtSendTo);
            this.panelSendEmail.Controls.Add(this.label1);
            this.panelSendEmail.Location = new System.Drawing.Point(423, 71);
            this.panelSendEmail.Name = "panelSendEmail";
            this.panelSendEmail.Size = new System.Drawing.Size(929, 565);
            this.panelSendEmail.TabIndex = 76;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label29.Location = new System.Drawing.Point(320, 19);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(180, 29);
            this.label29.TabIndex = 74;
            this.label29.Text = "Send an email";
            // 
            // btnSendEmail
            // 
            this.btnSendEmail.Location = new System.Drawing.Point(692, 394);
            this.btnSendEmail.Name = "btnSendEmail";
            this.btnSendEmail.Size = new System.Drawing.Size(101, 32);
            this.btnSendEmail.TabIndex = 13;
            this.btnSendEmail.Text = "Send";
            this.btnSendEmail.UseVisualStyleBackColor = true;
            this.btnSendEmail.Click += new System.EventHandler(this.btnSendEmail_Click_1);
            // 
            // txtMessage
            // 
            this.txtMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessage.Location = new System.Drawing.Point(221, 196);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMessage.Size = new System.Drawing.Size(572, 178);
            this.txtMessage.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(111, 199);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 22);
            this.label2.TabIndex = 11;
            this.label2.Text = "Message:";
            // 
            // txtSubject
            // 
            this.txtSubject.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSubject.Location = new System.Drawing.Point(221, 136);
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Size = new System.Drawing.Size(318, 28);
            this.txtSubject.TabIndex = 10;
            // 
            // Subject
            // 
            this.Subject.AutoSize = true;
            this.Subject.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Subject.Location = new System.Drawing.Point(123, 139);
            this.Subject.Name = "Subject";
            this.Subject.Size = new System.Drawing.Size(75, 22);
            this.Subject.TabIndex = 9;
            this.Subject.Text = "Subject:";
            // 
            // txtSendTo
            // 
            this.txtSendTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSendTo.Location = new System.Drawing.Point(221, 88);
            this.txtSendTo.Name = "txtSendTo";
            this.txtSendTo.Size = new System.Drawing.Size(318, 28);
            this.txtSendTo.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(161, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 22);
            this.label1.TabIndex = 7;
            this.label1.Text = "To:";
            // 
            // panelInbox
            // 
            this.panelInbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelInbox.Controls.Add(this.label5);
            this.panelInbox.Controls.Add(this.txtShowRecieved);
            this.panelInbox.Controls.Add(this.label4);
            this.panelInbox.Controls.Add(this.label3);
            this.panelInbox.Controls.Add(this.gridRecieveEmail);
            this.panelInbox.Location = new System.Drawing.Point(423, 71);
            this.panelInbox.Name = "panelInbox";
            this.panelInbox.Size = new System.Drawing.Size(929, 565);
            this.panelInbox.TabIndex = 77;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label5.Location = new System.Drawing.Point(290, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(277, 29);
            this.label5.TabIndex = 74;
            this.label5.Text = "Show recieved emails ";
            // 
            // txtShowRecieved
            // 
            this.txtShowRecieved.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtShowRecieved.Location = new System.Drawing.Point(178, 325);
            this.txtShowRecieved.Multiline = true;
            this.txtShowRecieved.Name = "txtShowRecieved";
            this.txtShowRecieved.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtShowRecieved.Size = new System.Drawing.Size(491, 223);
            this.txtShowRecieved.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(58, 352);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 22);
            this.label4.TabIndex = 14;
            this.label4.Text = "Message:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(37, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(285, 20);
            this.label3.TabIndex = 1;
            this.label3.Text = "Click in grid row to read email details";
            // 
            // gridRecieveEmail
            // 
            this.gridRecieveEmail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridRecieveEmail.Location = new System.Drawing.Point(41, 122);
            this.gridRecieveEmail.Name = "gridRecieveEmail";
            this.gridRecieveEmail.RowHeadersWidth = 51;
            this.gridRecieveEmail.RowTemplate.Height = 24;
            this.gridRecieveEmail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridRecieveEmail.Size = new System.Drawing.Size(742, 188);
            this.gridRecieveEmail.TabIndex = 0;
            this.gridRecieveEmail.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridRecieveEmail_CellClick_1);
            // 
            // EmailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1490, 667);
            this.Controls.Add(this.btbBackToAdminForm);
            this.Controls.Add(this.btnHomeInbox);
            this.Controls.Add(this.btnFormHomeSendEmail);
            this.Controls.Add(this.panelSendEmail);
            this.Controls.Add(this.panelInbox);
            this.Name = "EmailForm";
            this.Text = "AdminEmail";
            this.Load += new System.EventHandler(this.AdminEmail_Load);
            this.panelSendEmail.ResumeLayout(false);
            this.panelSendEmail.PerformLayout();
            this.panelInbox.ResumeLayout(false);
            this.panelInbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridRecieveEmail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnFormHomeSendEmail;
        private System.Windows.Forms.Button btnHomeInbox;
        private System.Windows.Forms.Button btbBackToAdminForm;
        private System.Windows.Forms.Panel panelSendEmail;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Button btnSendEmail;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSubject;
        private System.Windows.Forms.Label Subject;
        private System.Windows.Forms.TextBox txtSendTo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelInbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtShowRecieved;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView gridRecieveEmail;
    }
}