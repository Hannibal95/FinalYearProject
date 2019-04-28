using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FinalProjectApp.Classes
{
    public class ProjectReport
    {
        FinalProjectDbEntities context = new FinalProjectDbEntities();
        List<Tuple<string, double>> projectStats = new List<Tuple<string, double>>();
        public void CreateReport(List<Tuple<string, double>> data)
        {
            projectStats = data;

            Microsoft.Office.Interop.Excel.Application excel;
            Workbook workbook;

            excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Visible = false;
            excel.DisplayAlerts = false;

            workbook = excel.Workbooks.Add(Type.Missing);

            try
            {

                Worksheet worksheet1;
                Range cellRange;
                worksheet1 = (Worksheet)workbook.Worksheets[1];
                worksheet1.Activate();
                worksheet1.Name = SysVars.CurrentProjectName + " Stats";
                worksheet1.Range[worksheet1.Cells[1, 1], worksheet1.Cells[1, 2]].Merge();
                worksheet1.Cells[1, 1] = SysVars.CurrentProjectName + " Stats Report";
                worksheet1.Cells.Font.Size = 14;

                int rowcount = 3;
                foreach (DataRow datarow in ExportToExcel().Rows)
                {
                    rowcount += 1;
                    for (int i = 1; i <= ExportToExcel().Columns.Count; i++)
                    {
                        if (rowcount == 4)
                        {
                            worksheet1.Cells[3, i] = ExportToExcel().Columns[i - 1].ColumnName;
                            worksheet1.Cells.Font.Color = System.Drawing.Color.Black;
                        }

                        worksheet1.Cells[rowcount, i] = datarow[i - 1].ToString();

                        if (rowcount > 4)
                        {
                            if (i == ExportToExcel().Columns.Count)
                            {
                                if (rowcount % 2 == 0)
                                {
                                    cellRange = worksheet1.Range[worksheet1.Cells[rowcount, 1], worksheet1.Cells[rowcount, ExportToExcel().Columns.Count]];
                                }
                            }
                        }
                    }
                }

                cellRange = worksheet1.Range[worksheet1.Cells[1, 1], worksheet1.Cells[rowcount, ExportToExcel().Columns.Count]];
                cellRange.EntireColumn.AutoFit();
                Borders border = cellRange.Borders;
                border.LineStyle = XlLineStyle.xlContinuous;
                border.Weight = 2d;

                cellRange = worksheet1.Range[worksheet1.Cells[1, 1], worksheet1.Cells[2, ExportToExcel().Columns.Count]];

                string filename = SysVars.CurrentProjectName;
                if (filename.Contains(".") || filename.Contains("/") || filename.Contains(@"\"))
                {
                    filename = filename.Replace(".", "-");
                    filename = filename.Replace("/", "-");
                    filename = filename.Replace(@"\", "-");
                }
                filename += " Report-" + DateTime.Today.ToString("yyyy-MM-dd");

                var folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string folderName = folderBrowserDialog1.SelectedPath;
                    workbook.SaveAs(folderName + "\\" + filename);
                }
                workbook.Close();
                excel.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                workbook = null;
            }
        }

        public System.Data.DataTable ExportToExcel()
        {
            System.Data.DataTable table = new System.Data.DataTable();

            table.Columns.Add("Name:", typeof(string));
            table.Columns.Add("Value:", typeof(double));

            foreach (var item in projectStats)
            {
                table.Rows.Add(item.Item1, item.Item2);
            }

            return table;

        }
    }
}