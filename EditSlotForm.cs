using System;
using System.Windows.Forms;

namespace Lab1_WinForms
{
    public partial class EditSlotForm : Form
    {
        public TimeOnly WorkFrom => TimeOnly.FromDateTime(dtpFrom.Value);
        public TimeOnly WorkTo => TimeOnly.FromDateTime(dtpTo.Value);
        public bool IsDayOff => chkDayOff.Checked;

        public EditSlotForm(string empFio, string dayName, TimeOnly? currentFrom, TimeOnly? currentTo)
        {
            InitializeComponent();
            this.Text = $"{empFio} — {dayName}";

            // Настраиваем поля только для ввода часов и минут (без календаря)
            dtpFrom.Format = DateTimePickerFormat.Custom;
            dtpFrom.CustomFormat = "HH:mm";
            dtpFrom.ShowUpDown = true;

            dtpTo.Format = DateTimePickerFormat.Custom;
            dtpTo.CustomFormat = "HH:mm";
            dtpTo.ShowUpDown = true;

            // Если время уже было задано в БД — подставляем его, иначе ставим 08:00 - 14:00
            TimeOnly defaultFrom = currentFrom ?? new TimeOnly(8, 0);
            TimeOnly defaultTo = currentTo ?? new TimeOnly(14, 0);

            dtpFrom.Value = DateTime.Today.Add(defaultFrom.ToTimeSpan());
            dtpTo.Value = DateTime.Today.Add(defaultTo.ToTimeSpan());

            // При галочке "Выходной" блокируем выбор времени
            chkDayOff.CheckedChanged += (s, e) =>
            {
                dtpFrom.Enabled = !chkDayOff.Checked;
                dtpTo.Enabled = !chkDayOff.Checked;
            };

            // Проверка и сохранение
            btnSave.Click += (s, e) =>
            {
                if (!chkDayOff.Checked && WorkFrom >= WorkTo)
                {
                    MessageBox.Show("Время начала должно быть меньше времени окончания.",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
        }
    }
}
