using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace HMS.Web.API.Common
{
    public static class Documents
    {
        public static string ExportWithType(int ExportType, string VisibleColumnInfo, List<object> objList)
        {
            string FilePath = "";
            var table = GenerateDatatable(VisibleColumnInfo, objList);

            switch (ExportType)
            {
                // Write PDF File

                case (int)UtilEnum.ExportTypes.PDF:
                    FilePath = DocumentInfo.getTempDocumentPathInfo(true) + ".pdf";
                    SavePDF(table, FilePath);
                    break;
                // Write XLS File
                case (int)UtilEnum.ExportTypes.XLSX:
                    FilePath = DocumentInfo.getTempDocumentPathInfo(true) + ".xlsx";
                    SaveExcel(table, FilePath);
                    break;
                case (int)UtilEnum.ExportTypes.CSV:
                    FilePath = DocumentInfo.getTempDocumentPathInfo(true) + ".csv";
                    SaveExcel(table, FilePath);
                    break;
                case (int)UtilEnum.ExportTypes.XLS:
                    FilePath = DocumentInfo.getTempDocumentPathInfo(true) + ".xls";
                    SaveExcel(table, FilePath);
                    break;
            }

            return Utility.UrlEncode(FilePath);
        }
        private static DataTable GenerateDatatable(string VisibleColumnInfo, List<object> objList)
        {
            DataTable table = new DataTable();
            int Index = 0, CIndex;
            List<string> CustomColumnName = new List<string>();

            try
            {
                var ColumnNames = VisibleColumnInfo.Split(',').Select(s => s.Split('#')[0].Trim()).ToList();
                var ColumnTitle = VisibleColumnInfo.Split(',').Select(s => s.Split('#')[1].Trim()).ToList();

                if (objList.Count() > 0)
                {
                    var firstRecord = objList.FirstOrDefault();

                    PropertyInfo[] infos = firstRecord.GetType().GetProperties();

                    foreach (var infoi in infos)
                    {
                        CIndex = ColumnNames.IndexOf(infoi.Name);
                        if (CIndex == -1) continue;
                        table.Columns.Add(new DataColumn(ColumnTitle[CIndex], infoi.PropertyType));
                    }

                    foreach (var record in objList)
                    {
                        Index = 0;
                        DataRow row = table.NewRow();
                        PropertyInfo[] info = record.GetType().GetProperties();

                        foreach (var infoi in info)
                        {
                            if (ColumnNames.IndexOf(infoi.Name) == -1) continue;
                            row[Index] = infoi.GetValue(record); Index += 1;
                        }

                        table.Rows.Add(row);
                    }

                    return table;
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return null;
        }
        public static string ExportDocument()
        {
            string FilePath = "";
            FilePath = DocumentInfo.getTempDocumentPathInfo(true);
            return Utility.UrlEncode(FilePath);
        }
        private static void SavePDF(DataTable dt, string FilePath)
        {
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            try
            {
                StringBuilder strBody = new StringBuilder();
                using (StringWriter sw = new StringWriter())
                {
                    strBody.Append("<table border = '1'>");
                    strBody.Append("<tr>");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        strBody.Append("<th><b>" + dt.Columns[j].ToString() + "</b></th>");
                    }
                    strBody.Append("</tr>");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strBody.Append("<tr>");
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            strBody.Append("<td>" + dt.Rows[i][dt.Columns[j].ToString()].ToString() + "</td>");
                        }
                        strBody.Append("</tr>");
                    }
                    strBody.Append("</table>");
                    //SavePDF(strBody.ToString(), FilePath);



                    PdfWriter.GetInstance(document, new FileStream(FilePath, FileMode.Create));
                    document.Open();
                    List<iTextSharp.text.IElement> htmlarraylist = HTMLWorker.ParseToList(new StringReader(strBody.ToString()), null);
                    for (int k = 0; k < htmlarraylist.Count; k++)
                    {
                        document.Add((iTextSharp.text.IElement)htmlarraylist[k]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                document.Close();
            }
        }
        private static void SaveExcel(DataTable dt, string FilePath)
        {
            try
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet1 = workbook.CreateSheet("Sheet 1");
                //make a header row
                IRow row1 = sheet1.CreateRow(0);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Columns[j].ToString());
                }
                //loops through data
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet1.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.SetCellValue(dt.Rows[i][dt.Columns[j].ToString()].ToString());
                    }
                }
                using (var fileData = new FileStream(FilePath, FileMode.Create))
                {
                    workbook.Write(fileData);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ReadFile(string path)
        {
            string template = File.ReadAllText(HttpContext.Current.Server.MapPath(path));
            return template;
        }
    }
}