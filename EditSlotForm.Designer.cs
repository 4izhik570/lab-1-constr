namespace Lab1_WinForms
{
    partial class EditSlotForm
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
            chkDayOff = new CheckBox();
            btnSave = new Button();
            dtpFrom = new DateTimePicker();
            dtpTo = new DateTimePicker();
            SuspendLayout();
            // 
            // chkDayOff
            // 
            chkDayOff.AutoSize = true;
            chkDayOff.ForeColor = Color.Black;
            chkDayOff.Location = new Point(29, 29);
            chkDayOff.Name = "chkDayOff";
            chkDayOff.Size = new Size(261, 24);
            chkDayOff.TabIndex = 0;
            chkDayOff.Text = "Выходной день (Удалить запись)";
            chkDayOff.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(29, 119);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(94, 29);
            btnSave.TabIndex = 1;
            btnSave.Text = "Сохранить";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // dtpFrom
            // 
            dtpFrom.Location = new Point(36, 71);
            dtpFrom.Name = "dtpFrom";
            dtpFrom.Size = new Size(250, 27);
            dtpFrom.TabIndex = 2;
            // 
            // dtpTo
            // 
            dtpTo.Location = new Point(311, 71);
            dtpTo.Name = "dtpTo";
            dtpTo.Size = new Size(250, 27);
            dtpTo.TabIndex = 3;
            // 
            // EditSlotForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(588, 160);
            Controls.Add(dtpTo);
            Controls.Add(dtpFrom);
            Controls.Add(btnSave);
            Controls.Add(chkDayOff);
            Name = "EditSlotForm";
            Text = "EditSlotForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox chkDayOff;
        private Button btnSave;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
    }
}