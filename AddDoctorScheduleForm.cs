using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Lab1_WinForms
{
    public partial class AddDoctorScheduleForm : Form
    {
        public int SelectedEmpId => Convert.ToInt32(cmbEmp.SelectedValue);
        public int SelectedStatusId => Convert.ToInt32(cmbStatus.SelectedValue);
        public TimeOnly WorkFrom => TimeOnly.FromDateTime(dtpFrom.Value);
        public TimeOnly WorkTo => TimeOnly.FromDateTime(dtpTo.Value);
        public List<int> SelectedDayIds { get; private set; } = new List<int>();

        private class DayItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        public AddDoctorScheduleForm(DataTable dtEmps, DataTable dtStatuses, DataTable dtDays, string vrach)
        {
            InitializeComponent();
            this.Text = "Добавление расписания врача";

            cmbEmp.DataSource = dtEmps;
            cmbEmp.DisplayMember = "fio";
            cmbEmp.ValueMember = "id";

            cmbStatus.DataSource = dtStatuses;
            cmbStatus.DisplayMember = "name";
            cmbStatus.ValueMember = "id";

            if (!string.IsNullOrEmpty(vrach))
            {
                var match = dtEmps.AsEnumerable()
                                   .FirstOrDefault(r => r.Field<string>("fio") == vrach);
                if (match != null)
                    cmbEmp.SelectedValue = match["id"];
            }
            else
            {
                cmbEmp.SelectedValue = dtEmps.Rows[0]["fio"];
            }
            foreach (DataRow row in dtDays.Rows)
            {
                clbDays.Items.Add(new DayItem
                {
                    Id = Convert.ToInt32(row["id"]),
                    Name = row["name"].ToString()
                });
            }

            dtpFrom.Format = DateTimePickerFormat.Custom;
            dtpFrom.CustomFormat = "HH:mm";
            dtpFrom.ShowUpDown = true;
            dtpFrom.Value = DateTime.Today.AddHours(8);

            dtpTo.Format = DateTimePickerFormat.Custom;
            dtpTo.CustomFormat = "HH:mm";
            dtpTo.ShowUpDown = true;
            dtpTo.Value = DateTime.Today.AddHours(14);

            btnSave.Click += (s, e) =>
            {
                if (cmbEmp.SelectedValue == null || cmbStatus.SelectedValue == null)
                {
                    MessageBox.Show("Выберите врача и статус.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (clbDays.CheckedItems.Count == 0)
                {
                    MessageBox.Show("Отметьте хотя бы один день недели.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (WorkFrom >= WorkTo)
                {
                    MessageBox.Show("Время начала должно быть меньше времени окончания.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SelectedDayIds.Clear();
                foreach (DayItem item in clbDays.CheckedItems)
                {
                    SelectedDayIds.Add(item.Id);
                }

                // Чтобы логика Form1 не ломалась (смотри метод btnAddSchedule_Click)
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
        }
    }
}
