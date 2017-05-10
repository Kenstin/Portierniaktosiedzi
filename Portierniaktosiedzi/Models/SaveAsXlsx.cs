using Microsoft.Office.Interop.Excel;
using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.Models
{
    class SaveAsXlsx
    {
        private static Application timetable;
        private Workbook workbook;
        private Worksheet worksheet;

        public SaveAsXlsx(string path, /*NegativeArray<Day> list,*/ string name)
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
            AdjustColumnWidth();
            MergeRows();
        }

        private void AdjustColumnWidth()
        {
            int[] tab = new int[] { 14, 13, 17, 17, 17, 17, 17, 17, 17, 17, 17 };
            for (int i = 0; i < tab.Length; i++)
            {
                Range range = (Range)worksheet.Cells[1, i + 1];
                range.Columns.ColumnWidth = tab[i];
            }
        }

        private void MergeRows()
        {
            Range(3, 1, 11, 1);
            Range(12, 1, 42, 1);
        }

        private void Range(int rowA, int columnA, int rowB, int columnB)
        {
            Range range = worksheet.Range[worksheet.Cells[rowA, columnA], worksheet.Cells[rowB, columnB]];
            range.Merge();
        }
    }
}
