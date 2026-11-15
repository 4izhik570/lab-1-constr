namespace Lab1_WinForms
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dgvSchedule = new DataGridView();
            btnAddSchedule = new Button();
            btnDeleteDoctorSchedule = new Button();
            btnPrint = new Button();
            btnFind = new Button();
            dgvFunc = new DataGridView();
            nudFind = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)dgvSchedule).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvFunc).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudFind).BeginInit();
            SuspendLayout();
            // 
            // dgvSchedule
            // 
            dgvSchedule.AllowUserToAddRows = false;
            dgvSchedule.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSchedule.Location = new Point(12, 142);
            dgvSchedule.Name = "dgvSchedule";
            dgvSchedule.ReadOnly = true;
            dgvSchedule.RowHeadersWidth = 51;
            dgvSchedule.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvSchedule.Size = new Size(673, 153);
            dgvSchedule.TabIndex = 0;
            // 
            // btnAddSchedule
            // 
            btnAddSchedule.Location = new Point(37, 50);
            btnAddSchedule.Name = "btnAddSchedule";
            btnAddSchedule.Size = new Size(252, 29);
            btnAddSchedule.TabIndex = 1;
            btnAddSchedule.Text = "Добавить расписание врача";
            btnAddSchedule.UseVisualStyleBackColor = true;
            btnAddSchedule.Click += btnAddSchedule_Click_1;
            // 
            // btnDeleteDoctorSchedule
            // 
            btnDeleteDoctorSchedule.Location = new Point(37, 85);
            btnDeleteDoctorSchedule.Name = "btnDeleteDoctorSchedule";
            btnDeleteDoctorSchedule.Size = new Size(252, 29);
            btnDeleteDoctorSchedule.TabIndex = 3;
            btnDeleteDoctorSchedule.Text = "Удалить расписание врача";
            btnDeleteDoctorSchedule.UseVisualStyleBackColor = true;
            // 
            // btnPrint
            // 
            btnPrint.Location = new Point(309, 50);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(159, 29);
            btnPrint.TabIndex = 4;
            btnPrint.Text = "Печать расписания";
            btnPrint.UseVisualStyleBackColor = true;
            // 
            // btnFind
            // 
            btnFind.Location = new Point(12, 390);
            btnFind.Name = "btnFind";
            btnFind.Size = new Size(161, 29);
            btnFind.TabIndex = 5;
            btnFind.Text = "Найти врача по ID";
            btnFind.UseVisualStyleBackColor = true;
            btnFind.Click += btnFind_Click;
            // 
            // dgvFunc
            // 
            dgvFunc.AllowUserToAddRows = false;
            dgvFunc.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvFunc.Location = new Point(252, 322);
            dgvFunc.Name = "dgvFunc";
            dgvFunc.ReadOnly = true;
            dgvFunc.RowHeadersWidth = 51;
            dgvFunc.Size = new Size(354, 97);
            dgvFunc.TabIndex = 6;
            // 
            // nudFind
            // 
            nudFind.Location = new Point(12, 334);
            nudFind.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudFind.Name = "nudFind";
            nudFind.Size = new Size(161, 27);
            nudFind.TabIndex = 7;
            nudFind.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(nudFind);
            Controls.Add(dgvFunc);
            Controls.Add(btnFind);
            Controls.Add(btnPrint);
            Controls.Add(btnDeleteDoctorSchedule);
            Controls.Add(btnAddSchedule);
            Controls.Add(dgvSchedule);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dgvSchedule).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvFunc).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudFind).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvSchedule;
        private Button btnAddSchedule;
        private Button btnDeleteDoctorSchedule;
        private Button btnPrint;
        private Button btnFind;
        private DataGridView dgvFunc;
        private NumericUpDown nudFind;
    }
}
