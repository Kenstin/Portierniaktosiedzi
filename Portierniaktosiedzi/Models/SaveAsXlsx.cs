using Microsoft.Office.Interop.Excel;
using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.Models
{
    class SaveAsXlsx
    {
        private static Application timetable;
        private Workbook workbook;
        private Worksheet worksheet;

        public SaveAsXlsx(string path, /*NegativeArray<Day> list,*/ string name/* int month, int year*/)
        {
            timetable = new Application();
            workbook = timetable.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            worksheet = (Worksheet)workbook.Worksheets[0];
            worksheet.Name = "Harmonogram";
            GenerateTemplateSheet();
            object missing = System.Reflection.Missing.Value;
            workbook.SaveAs(
                        Filename: path + "\\" + name,
                        FileFormat: XlFileFormat.xlOpenXMLWorkbook,
                        Password: missing,
                        WriteResPassword: missing,
                        ReadOnlyRecommended: false,
                        CreateBackup: false,
                        AccessMode: XlSaveAsAccessMode.xlNoChange,
                        ConflictResolution: missing,
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
            int[] tab = new int[] { 14, 13, 17, 17, 17, 17, 17, 17, 17, 17, 17 };
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
            Range(11, 1, 41, 1);
            for (int i = 2; i <= 11; i++)
            {
                Range(2, i, 3, i);
                Range(33, i, 41, i);
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
            worksheet.Cells[41, 3] = "Ilość dni roboczych";
            AlignCenter("C41");
            worksheet.Range["C41"].WrapText = true;
            worksheet.Cells[2, 9] = "6.00 - 14.00";
            AlignCenter("I2");
            worksheet.Cells[2, 10] = "14.00 - 22.00";
            AlignCenter("J2");
            worksheet.Cells[2, 11] = "22.00 - 6.00";
            AlignCenter("K2");
        }

        private void AlignCenter(string cell)
        {
            worksheet.Range[cell].HorizontalAlignment = XlHAlign.xlHAlignCenter;
            worksheet.Range[cell].VerticalAlignment = XlHAlign.xlHAlignCenter;
        }
    }
}
