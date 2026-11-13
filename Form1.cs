using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Drawing.Printing;
using Npgsql;

namespace Lab1_WinForms
{
    public partial class Form1 : Form
    {
        private readonly string _connString = "Host=localhost;Port=5432;Database=kozyrev;Username=postgres;Password=admin";
        private DataTable _dtDays;
        private int _printRowIndex = 0;
        private DataTable _dtStatuses;
        private ContextMenuStrip _statusMenu;

        public Form1()
        {
            InitializeComponent();
            dgvSchedule.CellDoubleClick += dgvSchedule_CellDoubleClick;
            dgvSchedule.CellFormatting += DgvSchedule_CellFormatting;
            dgvSchedule.CellMouseDown += DgvSchedule_CellMouseDown;
            btnAddSchedule.Click += btnAddSchedule_Click;
            btnPrint.Click += btnPrint_Click;
            //btnChangeStatus.Click += btnChangeStatus_Click;
            btnDeleteDoctorSchedule.Click += btnDeleteDoctorSchedule_Click;
            //dgvSchedule.CellMouseDown += DgvSchedule_CellMouseDown;

            LoadScheduleGrid();
            //BuildStatusContextMenu();
        }

        private void LoadScheduleGrid()
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();

                // 1. Справочник дней недели
                _dtDays = new DataTable();
                new NpgsqlDataAdapter(@"SELECT id, name FROM dayweek ORDER BY id", conn).Fill(_dtDays);

                // 2. Справочник статусов
                _dtStatuses = new DataTable();
                new NpgsqlDataAdapter(@"SELECT id, name FROM status ORDER BY id", conn).Fill(_dtStatuses);

                // 3. Врачи и их специализации
                var dtEmps = new DataTable();
                string sqlEmps = @"
                    SELECT e.id, e.fio, s.name AS spec_name
                    FROM emp e
                    LEFT JOIN spec s ON e.specid = s.id
                    ORDER BY e.fio";
                new NpgsqlDataAdapter(sqlEmps, conn).Fill(dtEmps);

                // 4. Расписание со статусами
                var dtSchedule = new DataTable();
                string sqlSchedule = @"
                    SELECT sch.id, sch.empid, sch.dayweekid, sch.workfrom, sch.workto, 
                           sch.statusid, st.name AS status_name
                    FROM schedule sch
                    LEFT JOIN status st ON sch.statusid = st.id";
                new NpgsqlDataAdapter(sqlSchedule, conn).Fill(dtSchedule);

                // 5. Формируем таблицу для DataGridView
                DataTable gridTable = new DataTable();
                gridTable.Columns.Add("empid", typeof(int));
                gridTable.Columns.Add("Врач", typeof(string));
                gridTable.Columns.Add("Специализация", typeof(string));
                gridTable.Columns.Add("Статус", typeof(string));

                foreach (DataRow day in _dtDays.Rows)
                {
                    gridTable.Columns.Add(day["name"].ToString(), typeof(string));
                }

                // Заполняем строки по врачам
                foreach (DataRow emp in dtEmps.Rows)
                {
                    DataRow row = gridTable.NewRow();
                    int empId = Convert.ToInt32(emp["id"]);

                    row["empid"] = empId;
                    row["Врач"] = emp["fio"];
                    row["Специализация"] = emp["spec_name"] == DBNull.Value ? "Не указана" : emp["spec_name"];

                    // Берем единый статус врача из первой записи его расписания
                    DataRow[] empSchedules = dtSchedule.Select($"empid = {empId}");
                    row["Статус"] = empSchedules.Length > 0 && empSchedules[0]["status_name"] != DBNull.Value
                        ? empSchedules[0]["status_name"].ToString()
                        : "Не задан";

                    // Заполняем ячейки дней недели только временем приема
                    foreach (DataRow day in _dtDays.Rows)
                    {
                        int dayId = Convert.ToInt32(day["id"]);
                        string dayName = day["name"].ToString();

                        DataRow[] found = dtSchedule.Select($"empid = {empId} AND dayweekid = {dayId}");

                        if (found.Length > 0 && found[0]["workfrom"] != DBNull.Value && found[0]["workto"] != DBNull.Value)
                        {
                            TimeOnly from = (TimeOnly)found[0]["workfrom"];
                            TimeOnly to = (TimeOnly)found[0]["workto"];
                            row[dayName] = $"{from:HH:mm} - {to:HH:mm}";
                        }
                        else
                        {
                            row[dayName] = "—";
                        }
                    }

                    gridTable.Rows.Add(row);
                }

                dgvSchedule.DataSource = gridTable;
                dgvSchedule.Columns["empid"].Visible = false;
                dgvSchedule.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }
        private void DgvSchedule_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string status = dgvSchedule.Rows[e.RowIndex].Cells["Статус"].Value?.ToString().ToLower() ?? "";
            if (status.Contains("болеет"))
                e.CellStyle.BackColor = Color.MistyRose;
            else if (status.Contains("в отпуске"))
                e.CellStyle.BackColor = Color.LemonChiffon;
        }

        private void dgvSchedule_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Игнорируем клики по заголовкам и первым 4 колонкам (empID, Врач, Специализация, Статус)
            if (e.RowIndex < 0 || e.ColumnIndex < 4) return;

            int empId = Convert.ToInt32(dgvSchedule.Rows[e.RowIndex].Cells["empid"].Value);
            string empFio = dgvSchedule.Rows[e.RowIndex].Cells["Врач"].Value.ToString();
            string dayName = dgvSchedule.Columns[e.ColumnIndex].Name;

            // Получаем ID дня недели по названию колонки (обратите внимание на индекс нулевого элемента)
            DataRow[] dayRows = _dtDays.Select($"name = '{dayName}'");
            if (dayRows.Length == 0) return;
            int dayId = Convert.ToInt32(dayRows.First()["id"]);

            // Читаем текущее время из базы (если запись на этот день уже существует)
            TimeOnly? currentFrom = null;
            TimeOnly? currentTo = null;

            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                string sql = @"SELECT workfrom, workto FROM schedule WHERE empid = @empId AND dayweekid = @dayId";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("empId", empId);
                    cmd.Parameters.AddWithValue("dayId", dayId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            currentFrom = reader.GetFieldValue<TimeOnly>(0);
                            currentTo = reader.GetFieldValue<TimeOnly>(1);
                        }
                    }
                }
            }

            // Открываем диалоговое окно и сохраняем результат
            using (var form = new EditSlotForm(empFio, dayName, currentFrom, currentTo))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    SaveOrDeleteSlot(empId, dayId, form.WorkFrom, form.WorkTo, form.IsDayOff);
                    LoadScheduleGrid(); // Перезагружаем таблицу на экране
                }
            }
        }

        private void SaveOrDeleteSlot(int empId, int dayId, TimeOnly workFrom, TimeOnly workTo, bool isDayOff)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();

                // 1. Если отмечен выходной — удаляем запись из расписания
                if (isDayOff)
                {
                    string deleteSql = @"DELETE FROM schedule WHERE empid = @empId AND dayweekid = @dayId";
                    using (var cmd = new NpgsqlCommand(deleteSql, conn))
                    {
                        cmd.Parameters.AddWithValue("empId", empId);
                        cmd.Parameters.AddWithValue("dayId", dayId);
                        cmd.ExecuteNonQuery();
                    }
                    return;
                }

                // 2. Берем единый статус врача из его существующих записей (или первый из справочника)
                int statusId = Convert.ToInt32(_dtStatuses.Rows.Cast<DataRow>().First()["id"]);
                using (var stCmd = new NpgsqlCommand(@"SELECT statusid FROM schedule WHERE empid = @empId AND statusid IS NOT NULL LIMIT 1", conn))
                {
                    stCmd.Parameters.AddWithValue("empId", empId);
                    object existingStatus = stCmd.ExecuteScalar();
                    if (existingStatus != null && existingStatus != DBNull.Value)
                        statusId = Convert.ToInt32(existingStatus);
                }

                // 3. Проверяем, есть ли уже запись на этот день
                object existingId;
                using (var checkCmd = new NpgsqlCommand(@"SELECT id FROM schedule WHERE empid = @empId AND dayweekid = @dayId LIMIT 1", conn))
                {
                    checkCmd.Parameters.AddWithValue("empId", empId);
                    checkCmd.Parameters.AddWithValue("dayId", dayId);
                    existingId = checkCmd.ExecuteScalar();
                }

                if (existingId != null && existingId != DBNull.Value)
                {
                    // Обновляем существующую смену
                    string updateSql = @"UPDATE schedule SET workfrom = @workFrom, workto = @workTo WHERE id = @id";
                    using (var cmd = new NpgsqlCommand(updateSql, conn))
                    {
                        cmd.Parameters.AddWithValue("workFrom", workFrom);
                        cmd.Parameters.AddWithValue("workTo", workTo);
                        cmd.Parameters.AddWithValue("id", Convert.ToInt32(existingId));
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Добавляем новую смену
                    string insertSql = @"
                INSERT INTO schedule (workfrom, workto, statusid, empid, dayweekid) 
                VALUES (@workFrom, @workTo, @statusId, @empId, @dayId)";
                    using (var cmd = new NpgsqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("workFrom", workFrom);
                        cmd.Parameters.AddWithValue("workTo", workTo);
                        cmd.Parameters.AddWithValue("statusId", statusId);
                        cmd.Parameters.AddWithValue("empId", empId);
                        cmd.Parameters.AddWithValue("dayId", dayId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        // 1. Вставка новой записи в расписание (выбор врача, статуса и рабочих дней)
        private void btnAddSchedule_Click(object sender, EventArgs e)
        {
            if (dgvSchedule.CurrentRow == null)
            {
                MessageBox.Show("Сначала выберите строку врача.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string vrachFio = Convert.ToString(dgvSchedule.CurrentRow.Cells["Врач"].Value);

            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                var dtEmps = new DataTable();
                new NpgsqlDataAdapter(@"SELECT id, fio FROM emp ORDER BY fio", conn).Fill(dtEmps);

                using (var form = new AddDoctorScheduleForm(dtEmps, _dtStatuses, _dtDays, vrachFio))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        SaveDoctorWeekSchedule(
                            form.SelectedEmpId,
                            form.SelectedStatusId,
                            form.SelectedDayIds,
                            form.WorkFrom,
                            form.WorkTo
                        );
                        LoadScheduleGrid();
                    }
                }
            }
        }

        private void SaveDoctorWeekSchedule(int empId, int statusId, System.Collections.Generic.List<int> dayIds, TimeOnly workFrom, TimeOnly workTo)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    // Обновляем единый статус для всех уже существующих записей врача
                    using (var updStatusCmd = new NpgsqlCommand(@"UPDATE schedule SET statusid = @statusId WHERE empid = @empId", conn, tran))
                    {
                        updStatusCmd.Parameters.AddWithValue("statusId", statusId);
                        updStatusCmd.Parameters.AddWithValue("empId", empId);
                        updStatusCmd.ExecuteNonQuery();
                    }

                    // Для каждого выбранного дня делаем UPDATE или INSERT
                    foreach (int dayId in dayIds)
                    {
                        object existingId;
                        using (var checkCmd = new NpgsqlCommand(@"SELECT id FROM schedule WHERE empid = @empId AND dayweekid = @dayId LIMIT 1", conn, tran))
                        {
                            checkCmd.Parameters.AddWithValue("empId", empId);
                            checkCmd.Parameters.AddWithValue("dayId", dayId);
                            existingId = checkCmd.ExecuteScalar();
                        }

                        if (existingId != null && existingId != DBNull.Value)
                        {
                            string updateSql = @"UPDATE schedule SET workfrom = @workFrom, workto = @workTo, statusid = @statusId WHERE id = @id";
                            using (var cmd = new NpgsqlCommand(updateSql, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("workFrom", workFrom);
                                cmd.Parameters.AddWithValue("workTo", workTo);
                                cmd.Parameters.AddWithValue("statusId", statusId);
                                cmd.Parameters.AddWithValue("id", Convert.ToInt32(existingId));
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string insertSql = @"INSERT INTO schedule (workfrom, workto, statusid, empid, dayweekid) VALUES (@workFrom, @workTo, @statusId, @empId, @dayId)";
                            using (var cmd = new NpgsqlCommand(insertSql, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("workFrom", workFrom);
                                cmd.Parameters.AddWithValue("workTo", workTo);
                                cmd.Parameters.AddWithValue("statusId", statusId);
                                cmd.Parameters.AddWithValue("empId", empId);
                                cmd.Parameters.AddWithValue("dayId", dayId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    tran.Commit();
                }
            }
        }


        private void DgvSchedule_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
        }


        //private void UpdateDoctorWeekStatus(int newStatusId)
        //{
        //    if (dgvSchedule.SelectedRows.Count == 0) return;
        //    int empId = Convert.ToInt32(dgvSchedule.SelectedRows[0].Cells["empid"].Value);

        //    using (var conn = new NpgsqlConnection(_connString))
        //    {
        //        conn.Open();
        //        string sql = @"UPDATE schedule SET statusid = @statusId WHERE empid = @empId";
        //        using (var cmd = new NpgsqlCommand(sql, conn))
        //        {
        //            cmd.Parameters.AddWithValue("statusId", newStatusId);
        //            cmd.Parameters.AddWithValue("empId", empId);
        //            int affected = cmd.ExecuteNonQuery();

        //            if (affected == 0)
        //            {
        //                MessageBox.Show("У врача еще нет записей в расписании. Сначала добавьте рабочие дни.",
        //                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            }
        //        }
        //    }
        //    LoadScheduleGrid();
        //}

        // 3. Удаление всего расписания выбранного врача
        private void btnDeleteDoctorSchedule_Click(object sender, EventArgs e)
        {
            if (dgvSchedule.CurrentRow == null) return;

            int empId = Convert.ToInt32(dgvSchedule.CurrentRow.Cells["empid"].Value);
            string empFio = dgvSchedule.CurrentRow.Cells["Врач"].Value.ToString();

            var confirm = MessageBox.Show($"Удалить все записи расписания для врача: {empFio}?",
                "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                string sql = @"DELETE FROM schedule WHERE empid = @empId";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("empId", empId);
                    cmd.ExecuteNonQuery();
                }
            }
            LoadScheduleGrid();
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintScheduleReport();
        }

        public void PrintScheduleReport()
        {
            _printRowIndex = 0;
            PrintDocument printDoc = new PrintDocument();
            printDoc.DocumentName = "Расписание работы врачей поликлиники";
            printDoc.DefaultPageSettings.Landscape = true; // Альбомная ориентация для широкой таблицы
            printDoc.PrintPage += PrintDoc_PrintPage;

            using (PrintPreviewDialog preview = new PrintPreviewDialog())
            {
                preview.Document = printDoc;
                preview.WindowState = FormWindowState.Maximized;
                preview.ShowDialog();
            }
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            int leftMargin = e.MarginBounds.Left;
            int topMargin = e.MarginBounds.Top;
            int y = topMargin;

            using Font titleFont = new Font("Segoe UI", 14, FontStyle.Bold);
            using Font subTitleFont = new Font("Segoe UI", 9, FontStyle.Italic);
            using Font headerFont = new Font("Segoe UI", 9, FontStyle.Bold);
            using Font cellFont = new Font("Segoe UI", 9, FontStyle.Regular);
            using Pen borderPen = new Pen(Color.Black, 1);

            // Заголовок отчета на первой странице
            if (_printRowIndex == 0)
            {
                g.DrawString("РАСПИСАНИЕ ПРИЕМА ВРАЧЕЙ ГОРОДСКОЙ ПОЛИКЛИНИКИ", titleFont, Brushes.Black, leftMargin, y);
                y += 28;
                g.DrawString($"Сформировано: {DateTime.Now:dd.MM.yyyy HH:mm}", subTitleFont, Brushes.DimGray, leftMargin, y);
                y += 25;
            }

            // Собираем видимые колонки (пропускаем скрытую колонку empid)
            var visibleCols = dgvSchedule.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            int totalGridWidth = visibleCols.Sum(c => c.Width);
            float scale = (float)e.MarginBounds.Width / totalGridWidth;
            int rowHeight = 28;

            // Отрисовка шапки таблицы
            int x = leftMargin;
            foreach (var col in visibleCols)
            {
                int colWidth = (int)(col.Width * scale);
                Rectangle rect = new Rectangle(x, y, colWidth, rowHeight);
                g.FillRectangle(Brushes.LightGray, rect);
                g.DrawRectangle(borderPen, rect);

                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(col.HeaderText, headerFont, Brushes.Black, rect, sf);
                x += colWidth;
            }
            y += rowHeight;

            // Отрисовка строк данных с поддержкой многостраничности
            while (_printRowIndex < dgvSchedule.Rows.Count)
            {
                if (y + rowHeight > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }

                DataGridViewRow gridRow = dgvSchedule.Rows[_printRowIndex];
                x = leftMargin;

                foreach (var col in visibleCols)
                {
                    int colWidth = (int)(col.Width * scale);
                    Rectangle rect = new Rectangle(x, y, colWidth, rowHeight);
                    g.DrawRectangle(borderPen, rect);

                    string text = gridRow.Cells[col.Index].Value?.ToString() ?? "";
                    StringFormat sf = new StringFormat
                    {
                        Alignment = col.Index <= 2 ? StringAlignment.Near : StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };

                    Rectangle textRect = new Rectangle(x + 4, y, colWidth - 8, rowHeight);
                    g.DrawString(text, cellFont, Brushes.Black, textRect, sf);
                    x += colWidth;
                }

                y += rowHeight;
                _printRowIndex++;
            }

            e.HasMorePages = false;
        }
        private void btnAddSchedule_Click_1(object sender, EventArgs e)
        {

        }
    }
}
