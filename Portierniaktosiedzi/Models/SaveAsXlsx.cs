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

        public SaveAsXlsx(string path, /*NegativeArray<Day> list, */string name, int month, int year)
        {
            try
            {
                if (System.IO.File.Exists(path + "\\" + name + ".xlsx"))
                {
                    System.IO.File.Delete(path + "\\" + name + ".xlsx");
                }
            }
            catch (System.IO.IOException)
            {
                System.Windows.MessageBox.Show("Plik jest aktualnie w użyciu, proszę go wyłączyć");
                return;
            }

            timetable = new Application();
            workbook = timetable.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            worksheet = (Worksheet)workbook.Worksheets[1];
            worksheet.Name = "Harmonogram";
            GenerateTemplateSheet();
            FillTemplateSheet(month, year);
            object missing = System.Reflection.Missing.Value;
            workbook.SaveAs(
                        Filename: path + "\\" + name,
                        FileFormat: XlFileFormat.xlOpenXMLWorkbook,
                        Password: missing,
                        WriteResPassword: missing,
                        ReadOnlyRecommended: false,
                        CreateBackup: false,
                        AccessMode: XlSaveAsAccessMode.xlNoChange,
                        ConflictResolution: XlSaveConflictResolution.xlUserResolution,
                        AddToMru: missing,
                        TextCodepage: missing,
                        TextVisualLayout: missing,
                        Local: missing);
        }

        private void GenerateTemplateSheet()
        {
            AdjustWidth();
            MergeRows();
            SetDefaultText();
        }

        private void AdjustWidth()
        {
            int[] tab = new int[] { 16, 13, 17, 17, 17, 17, 17, 17, 17, 17, 17 };
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
            for (int i = 2; i <= 11; i++)
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

        private void FillTemplateSheet(/*NegativeArray<Day> list, */int month, int year)
        {
            SetMonthAndYear(month, year);
        }

        private void SetMonthAndYear(int month, int year)
        {
            worksheet.Cells[2, 1] = "Miesiąc :\n" + CultureInfo.CreateSpecificCulture("pl").DateTimeFormat.GetMonthName(month).ToString();
            worksheet.Cells[11, 1] = "Harmonogram\ndyżurów\nportierni:\nrok\n" + year.ToString();
            AlignCenter("A2");
            AlignCenter("A11");
            SetDaysInMonth(month, year);
            DaysInWeek(month, year);
        }

        private void SetDaysInMonth(int month, int year)
        {
            for (int i = 1; i <= System.DateTime.DaysInMonth(year, month); i++)
            {
                    worksheet.Cells[3 + i, 2] = i.ToString() + " " + month.ToString().PadLeft(2, '0');
            }
        }

        private void DaysInWeek(int month, int year)
        {
            for (int i = 1; i <= System.DateTime.DaysInMonth(year, month); i++)
            {
                worksheet.Cells[i + 3, 3] = CultureInfo.CreateSpecificCulture("pl").DateTimeFormat.GetDayName(new System.DateTime(year, month, i).DayOfWeek);
            }
        }
    }
}
