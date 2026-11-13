namespace Lab1_WinForms
{
    partial class AddDoctorScheduleForm
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
            cmbEmp = new ComboBox();
            cmbStatus = new ComboBox();
            clbDays = new CheckedListBox();
            dtpFrom = new DateTimePicker();
            dtpTo = new DateTimePicker();
            btnSave = new Button();
            SuspendLayout();
            // 
            // cmbEmp
            // 
            cmbEmp.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEmp.FormattingEnabled = true;
            cmbEmp.Location = new Point(48, 92);
            cmbEmp.Name = "cmbEmp";
            cmbEmp.Size = new Size(151, 28);
            cmbEmp.TabIndex = 0;
            // 
            // cmbStatus
            // 
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.FormattingEnabled = true;
            cmbStatus.Location = new Point(221, 92);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.Size = new Size(151, 28);
            cmbStatus.TabIndex = 1;
            // 
            // clbDays
            // 
            clbDays.CheckOnClick = true;
            clbDays.FormattingEnabled = true;
            clbDays.Location = new Point(48, 157);
            clbDays.Name = "clbDays";
            clbDays.Size = new Size(150, 114);
            clbDays.TabIndex = 2;
            // 
            // dtpFrom
            // 
            dtpFrom.Location = new Point(258, 168);
            dtpFrom.Name = "dtpFrom";
            dtpFrom.Size = new Size(250, 27);
            dtpFrom.TabIndex = 3;
            // 
            // dtpTo
            // 
            dtpTo.Location = new Point(258, 201);
            dtpTo.Name = "dtpTo";
            dtpTo.Size = new Size(250, 27);
            dtpTo.TabIndex = 4;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(258, 242);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(94, 29);
            btnSave.TabIndex = 5;
            btnSave.Text = "Сохранить";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // AddDoctorScheduleForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnSave);
            Controls.Add(dtpTo);
            Controls.Add(dtpFrom);
            Controls.Add(clbDays);
            Controls.Add(cmbStatus);
            Controls.Add(cmbEmp);
            Name = "AddDoctorScheduleForm";
            Text = "AddDoctorScheduleForm";
            ResumeLayout(false);
        }

        #endregion

        private ComboBox cmbEmp;
        private ComboBox cmbStatus;
        private CheckedListBox clbDays;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private Button btnSave;
    }
}