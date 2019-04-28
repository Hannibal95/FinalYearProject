using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Office.Interop.Excel;

namespace FinalProjectApp.Classes
{
    public class BuildReport
    {
        FinalProjectDbEntities context = new FinalProjectDbEntities();
        List<Option> options = new List<Option>();
        public void CreateReport()
        {
            SetOptions();

            Microsoft.Office.Interop.Excel.Application excel;
            Workbook workbook;

            excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Visible = false;
            excel.DisplayAlerts = false;

            workbook = excel.Workbooks.Add(Type.Missing);

            List<Tuple<string, string, int>> sheetList = new List<Tuple<string, string, int>>()
            {
                new Tuple<string, string, int>("All Options", SysVars.CurrentBuildName + " Report: All Options", 1),
                new Tuple<string, string, int>("Not Completed Options", SysVars.CurrentBuildName + " Report: Not Completed Options", 2),
                new Tuple<string, string, int>("Completed Options", SysVars.CurrentBuildName + " Report: Completed Options", 3),
            };

            try
            {
                foreach (var sheet in sheetList)
                {
                    GenWorksheet(workbook, sheet);
                }

                string filename = SysVars.CurrentBuildName; 
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

        private void SetOptions()
        {
            var query = (from x in context.Options
                         where x.BuildId == SysVars.CurrentBuildId
                         select x);
            foreach (var option in query)
            {
                options.Add(option);
            }
        }

        public System.Data.DataTable ExportToExcel(int flag)
        {
            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Call Value", typeof(double));
            table.Columns.Add("Value-to-Cost Ratio", typeof(double));
            table.Columns.Add("Volatility", typeof(double));
            table.Columns.Add("Duration (Days)", typeof(int));
            table.Columns.Add("Completed", typeof(bool));

            foreach (var option in options)
            {
                if (flag == 1)
                {
                    table.Rows.Add(option.Id, option.Name, option.CallValue, option.ValueToCostRatio,
                    option.Volatility, option.DurationDays, option.Complete);
                }
                else if (flag == 2 && option.Complete == false)
                {
                    table.Rows.Add(option.Id, option.Name, option.CallValue, option.ValueToCostRatio,
                    option.Volatility, option.DurationDays, option.Complete);
                }
                else if (flag == 3 && option.Complete == true)
                {
                    table.Rows.Add(option.Id, option.Name, option.CallValue, option.ValueToCostRatio,
                    option.Volatility, option.DurationDays, option.Complete);
                }
            }

            return table;

        }

        private void GenWorksheet(Workbook workbook, Tuple<string, string, int> sheet)
        {
            Worksheet worksheet1;
            Range cellRange;
            worksheet1 = (Worksheet)workbook.Worksheets[1];
            worksheet1.Activate();
            worksheet1.Name = sheet.Item1;

            worksheet1.Range[worksheet1.Cells[1, 1], worksheet1.Cells[1, 7]].Merge();
            worksheet1.Cells[1, 1] = sheet.Item2;
            worksheet1.Cells.Font.Size = 14;

            int rowcount = 3;
            foreach (DataRow datarow in ExportToExcel(sheet.Item3).Rows)
            {
                rowcount += 1;
                for (int i = 1; i <= ExportToExcel(sheet.Item3).Columns.Count; i++)
                {
                    if (rowcount == 4)
                    {
                        worksheet1.Cells[3, i] = ExportToExcel(sheet.Item3).Columns[i - 1].ColumnName;
                        worksheet1.Cells.Font.Color = System.Drawing.Color.Black;
                    }

                    worksheet1.Cells[rowcount, i] = datarow[i - 1].ToString();

                    if (rowcount > 4)
                    {
                        if (i == ExportToExcel(sheet.Item3).Columns.Count)
                        {
                            if (rowcount % 2 == 0)
                            {
                                cellRange = worksheet1.Range[worksheet1.Cells[rowcount, 1], worksheet1.Cells[rowcount, ExportToExcel(sheet.Item3).Columns.Count]];
                            }
                        }
                    }
                }
            }

            cellRange = worksheet1.Range[worksheet1.Cells[1, 1], worksheet1.Cells[rowcount, ExportToExcel(sheet.Item3).Columns.Count]];
            cellRange.EntireColumn.AutoFit();
            Borders border = cellRange.Borders;
            border.LineStyle = XlLineStyle.xlContinuous;
            border.Weight = 2d;

            cellRange = worksheet1.Range[worksheet1.Cells[1, 1], worksheet1.Cells[2, ExportToExcel(sheet.Item3).Columns.Count]];

            if (sheet.Item3 < 3) { workbook.Worksheets.Add(); }
        }
    }
}
