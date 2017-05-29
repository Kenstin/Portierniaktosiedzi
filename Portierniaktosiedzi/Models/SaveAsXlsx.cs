using System.Collections.Generic;
using System.Globalization;
using Microsoft.Office.Interop.Excel;
using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.Models
{
    class SaveAsXlsx
    {
        private static Application timetable;
        private Workbook workbook;
        private Worksheet worksheet;

        public SaveAsXlsx(string path, NegativeArray<Day> list, Dictionary<Employee, decimal> leftworkinghours, string name, int month, int year)
        {
            try
            {
                if (System.IO.File.Exists(path + "\\" + name + ".xlsx"))
                {
                    System.IO.File.Delete(path + "\\" + name + ".xlsx");
                }
            }
            catch (System.IO.IOException e)
            {
                throw new System.IO.IOException("Access to the file denied.", e);
            }

            timetable = new Application();
            workbook = timetable.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            worksheet = (Worksheet)workbook.Worksheets[1];
            worksheet.Name = "Harmonogram";
            GenerateTemplateSheet();
            FillTemplateSheet(list, leftworkinghours, month, year);
            workbook.SaveAs(
                        Filename: path + "\\" + name,
                        FileFormat: XlFileFormat.xlOpenXMLWorkbook,
                        ReadOnlyRecommended: false,
                        CreateBackup: false,
                        AccessMode: XlSaveAsAccessMode.xlNoChange,
                        ConflictResolution: XlSaveConflictResolution.xlUserResolution);
            workbook.Close();
            timetable.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(timetable);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
        }

        private void GenerateTemplateSheet()
        {
            AdjustWidth();
            MergeRows();
            SetDefaultText();
        }

        private void AdjustWidth()
        {
            int[] tab = new int[] { 16, 13, 17, 17, 17, 17 };
            for (int i = 0; i < tab.Length; i++)
            {
                Range columnrange = (Range)worksheet.Cells[1, i + 1];
                columnrange.Columns.ColumnWidth = tab[i];
            }

            Range rowrange = (Range)worksheet.Cells[1, 1];
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

        private void FillTemplateSheet(NegativeArray<Day> list, Dictionary<Employee, decimal> workinghours, int month, int year)
        {
            SetMonthAndYear(month, year);
            FillEmployees(list, month, year);
            FillWorkingHours(workinghours);
        }

        private void SetMonthAndYear(int month, int year)
        {
            worksheet.Cells[2, 1] = "Miesiąc :\n" + CultureInfo.CreateSpecificCulture("pl").DateTimeFormat.GetMonthName(month);
            worksheet.Cells[11, 1] = "Harmonogram\ndyżurów\nportierni:\nrok\n" + year;
            AlignCenter("A2");
            AlignCenter("A11");
            SetDaysInMonth(month, year);
            DaysInWeek(month, year);
        }

        private void SetDaysInMonth(int month, int year)
        {
            for (int i = 1; i <= System.DateTime.DaysInMonth(year, month); i++)
            {
                    worksheet.Cells[3 + i, 2] = "'" + i + "." + month.ToString().PadLeft(2, '0');
            }
        }

        private void DaysInWeek(int month, int year)
        {
            for (int i = 1; i <= System.DateTime.DaysInMonth(year, month); i++)
            {
                worksheet.Cells[i + 3, 3] = CultureInfo.CreateSpecificCulture("pl").DateTimeFormat.GetDayName(new System.DateTime(year, month, i).DayOfWeek);
            }
        }

        private void FillEmployees(NegativeArray<Day> list, int month, int year)
        {
            for (int shift = 0; shift <= 2; shift++)
            {
                worksheet.Cells[1, 3 + shift] = list[0].Shifts[shift].Name;
            }

            for (int day = 1; day <= System.DateTime.DaysInMonth(year, month); day++)
            {
                for (int shift = 0; shift <= 2; shift++)
                {
                    worksheet.Cells[day + 3, shift + 3] = list[day].Shifts[shift].Name;
                }
            }
        }

        private void FillWorkingHours(Dictionary<Employee, decimal> workinghours)
        {
            int x = 8, y = 4;
            foreach (var i in workinghours)
            {
                worksheet.Cells[y, x + 1] = i.Key.Name;
                worksheet.Cells[y++, x++] = i.Value;
            }
        }
    }
}
