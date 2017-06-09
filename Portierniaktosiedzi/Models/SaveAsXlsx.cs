using System;
using System.Globalization;
using Microsoft.Office.Interop.Excel;

namespace Portierniaktosiedzi.Models
{
    public class SaveAsXlsx : IDisposable
    {
        private readonly Application excelApplication;
        private readonly Timetable timetable;
        private Worksheet worksheet;
        private bool disposedValue; // To detect redundant calls

        public SaveAsXlsx(Timetable timetable)
        {
            excelApplication = new Application();
            this.timetable = timetable;
        }

        ~SaveAsXlsx()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SaveAs(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (System.IO.IOException e)
            {
                throw new System.IO.IOException("Access to the file denied.", e);
            }

            var workbooks = excelApplication.Workbooks;
            var workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);

            try
            {
                worksheet = (Worksheet)workbook.Worksheets[1];
                worksheet.Name = "Harmonogram";
                GenerateTemplateSheet();
                FillTemplateSheet();
                workbook.SaveAs(
                    Filename: path,
                    FileFormat: XlFileFormat.xlOpenXMLWorkbook,
                    ReadOnlyRecommended: false,
                    CreateBackup: false,
                    AccessMode: XlSaveAsAccessMode.xlNoChange,
                    ConflictResolution: XlSaveConflictResolution.xlUserResolution);
            }
            finally
            {
                workbook.Close();
                excelApplication.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbooks);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    worksheet = null;
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);

                disposedValue = true;
            }
        }

        private void GenerateTemplateSheet()
        {
            AdjustWidth();
            MergeRows();
            SetDefaultText();
        }

        private void AdjustWidth()
        {
            int[] tab = { 16, 13, 17, 17, 17, 17 };
            for (int i = 0; i < tab.Length; i++)
            {
                var columnrange = (Range)worksheet.Cells[1, i + 1];
                columnrange.Columns.ColumnWidth = tab[i];
            }

            var rowrange = (Range)worksheet.Cells[1, 1];
            rowrange.Rows.RowHeight = 26;
        }

        private void MergeRows()
        {
            Range(2, 1, 10, 1);
            Range(11, 1, 34, 1);
            for (int i = 2; i <= 6; i++)
            {
                Range(2, i, 3, i);
            }
        }

        private void Range(int rowA, int columnA, int rowB, int columnB)
        {
            worksheet.Range[worksheet.Cells[rowA, columnA], worksheet.Cells[rowB, columnB]].Merge();
        }

        private void SetDefaultText()
        {
            //TODO zamień wszystkie worksheet.Cells na worksheet range np. worksheet.Range["B2"].HorizontalAlignment ...
            worksheet.Cells[2, 2] = "Data";
            AlignCenter("B2");
            worksheet.Cells[2, 3] = "Dzień";
            AlignCenter("C2");
            worksheet.Cells[2, 4] = "1 zmiana";
            AlignCenter("D2");
            worksheet.Cells[2, 5] = "2 zmiana";
            AlignCenter("E2");
            worksheet.Cells[2, 6] = "3 zmiana";
            AlignCenter("F2");
        }

        private void AlignCenter(string cell)
        {
            worksheet.Range[cell].HorizontalAlignment = XlHAlign.xlHAlignCenter;
            worksheet.Range[cell].VerticalAlignment = XlHAlign.xlHAlignCenter;
        }

        private void FillTemplateSheet()
        {
            SetMonthAndYear();
            FillEmployees();
            FillWorkingHours();
            SetPreviousDay();
        }

        private void SetMonthAndYear()
        {
            worksheet.Cells[2, 1] = "Miesiąc:\n" + CultureInfo.CreateSpecificCulture("pl").DateTimeFormat.GetMonthName(timetable.Month.Month);
            worksheet.Cells[11, 1] = "Harmonogram\ndyżurów\nportierni:\nrok\n" + timetable.Month.Year;
            AlignCenter("A2");
            AlignCenter("A11");
            SetDaysInMonth();
            DaysInWeek();
        }

        private void SetDaysInMonth()
        {
            for (int i = 1; i <= DateTime.DaysInMonth(timetable.Month.Year, timetable.Month.Month); i++)
            {
                    worksheet.Cells[3 + i, 2] = "'" + i + "." + timetable.Month.Month.ToString().PadLeft(2, '0');
            }
        }

        private void DaysInWeek()
        {
            for (int i = 1; i <= DateTime.DaysInMonth(timetable.Month.Year, timetable.Month.Month); i++)
            {
                worksheet.Cells[i + 3, 3] = CultureInfo.CreateSpecificCulture("pl").DateTimeFormat.GetDayName(new DateTime(timetable.Month.Year, timetable.Month.Month, i).DayOfWeek);
            }
        }

        private void FillEmployees()
        {
            for (int shift = 0; shift <= 2; shift++)
            {
                worksheet.Cells[1, 4 + shift] = timetable.Days[0].Shifts[shift].Name;
            }

            for (int day = 1; day <= DateTime.DaysInMonth(timetable.Month.Year, timetable.Month.Month); day++)
            {
                for (int shift = 0; shift <= 2; shift++)
                {
                    worksheet.Cells[day + 3, shift + 4] = timetable.Days[day].Shifts[shift].Name;
                }
            }
        }

        private void FillWorkingHours()
        {
            int x = 8, y = 4;
            Range(y - 1, x, y - 1, x + 1);
            worksheet.Cells[y - 1, x] = "Pozostałe godziny";
            AlignCenter("H3");

            foreach (var i in timetable.WorkingHoursLeft)
            {
                worksheet.Cells[y, x + 1] = i.Key.Name;
                worksheet.Cells[y++, x] = i.Value;
            }
        }

        private void SetPreviousDay()
        {
            worksheet.Cells[1, 2] = "'" + DateTime.DaysInMonth(timetable.Month.Year, timetable.Month.Month - 1) + "." + (timetable.Month.Month - 1).ToString().PadLeft(2, '0');
            worksheet.Cells[1, 3] = CultureInfo.CreateSpecificCulture("pl").DateTimeFormat.GetDayName(new DateTime(timetable.Month.Year, timetable.Month.Month - 1, DateTime.DaysInMonth(timetable.Month.Year, timetable.Month.Month - 1)).DayOfWeek);
        }
    }
}
