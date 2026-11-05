using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Repository.Repositories.Items
{
    public static class quote_Invoice_mfRepository
    {
        public static PaginationResult Pagination(this IRepository<quote_invoice_mf> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<quote_invoice_mf, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayNo, DisplayReference, DisplayCompanyName;
                bool OrderByNo, OrderByReference, OrderByCompanyName;
                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayNo = PFilter.VisibleColumnInfoList.IndexOf("OrderNo") > -1;
                    DisplayReference = PFilter.VisibleColumnInfoList.IndexOf("BillNo") > -1;
                    DisplayCompanyName = PFilter.VisibleColumnInfoList.IndexOf("CompanyName") > -1;
                    predicate = (c => c.CompanyID == CompanyID &&
                    (DisplayNo && (c.OrderNo).Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayReference && (c.InvoiceNo).ToString().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayCompanyName && c.emr_patient_mf.PatientName.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                  ))));
                }
                IQueryable<quote_invoice_mf> filteredData = repository.Queryable().Where(predicate);
                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByNo = PFilter.OrderBy.IndexOf("OrderNo") > -1;
                OrderByReference = PFilter.OrderBy.IndexOf("InvoiceNo") > -1;
                OrderByCompanyName = PFilter.OrderBy.IndexOf("CompanyName") > -1;
                Expression<Func<quote_invoice_mf, string>> orderingFunction = (c =>
                                                             OrderByNo ? c.OrderNo :
                                                              OrderByReference ? c.InvoiceNo.ToString() :
                                                             OrderByCompanyName ? c.emr_patient_mf.PatientName.ToString() : ""
                                                               );

                if (PFilter.OrderBy == "ID")
                {
                    filteredData = filteredData.OrderByDescending(d => (d.ID)).AsQueryable();
                    objResult.TotalRecord = filteredData.Count();
                }
                else
                {
                    if (PFilter.IsOrderAsc)
                        filteredData = filteredData.OrderBy(orderingFunction);
                    else
                        filteredData = filteredData.OrderByDescending(orderingFunction);
                    objResult.TotalRecord = filteredData.Count();
                }
                if (IgnorePaging)
                {
                    var result = filteredData.Include(x => x.quote_invoice_dt).Include(x => x.emr_patient_mf)

                        .Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord).Select(s => new
                        {
                            s.ID,
                            Invoiceno = s.InvoiceNo,
                            status = s.SaveStatus == 1 ? "Draft" : "Publish",
                            date = s.InvoiceDate.Month + "/" + s.InvoiceDate.Day + "/" + s.InvoiceDate.Year,
                            duedate = s.DueDate.Month + "/" + s.DueDate.Day + "/" + s.DueDate.Year,
                            name = s.emr_patient_mf.PatientName,
                            amount = s.Total,
                            DisplayName = s.emr_patient_mf.PatientName,
                            isSelected = false,
                            SaveStatus = s.SaveStatus
                        }).OrderByDescending(a => a.ID).ToList();
                    objResult.DataList = result.AsEnumerable().Select(z => new InvoiceResponse
                    {
                        ID = z.ID,
                        Invoiceno = z.Invoiceno.ToString(),
                        date = z.date,
                        duedate = z.duedate,
                        name = z.name,
                        amount = z.amount,
                        status = z.status,
                        CompanyName = z.DisplayName,
                        isSelected = z.isSelected,
                        SaveStatus = z.SaveStatus == 1 ? "Draft" : "Publish"
                    }).ToList<object>();
                }
                else
                {
                    var result = filteredData.Include(x => x.quote_invoice_dt).Include(a => a.emr_patient_mf)
                        .Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                   .Select(s => new
                   {
                       s.ID,
                       status = s.SaveStatus == 1 ? "Draft" : "Publish",
                       Invoiceno = s.InvoiceNo,
                      date = s.InvoiceDate.Month + "/" + s.InvoiceDate.Day + "/" + s.InvoiceDate.Year,
                       duedate = s.DueDate.Month + "/" + s.DueDate.Day + "/" + s.DueDate.Year,
                       name = s.emr_patient_mf.PatientName,
                       amount = s.Total,
                       isSelected = false,
                       SaveStatus = s.SaveStatus,
                       DisplayName = s.emr_patient_mf.PatientName,
                   }).OrderByDescending(a => a.ID).ToList();
                    objResult.DataList = result.AsEnumerable().Select(z => new InvoiceResponse
                    {
                        ID = z.ID,
                        Invoiceno = z.Invoiceno.ToString(),
                        date = z.date,
                        status = z.status,
                        duedate = z.duedate,
                        name = z.name,
                        amount = z.amount,
                        CompanyName = z.DisplayName,
                        isSelected = z.isSelected,
                        SaveStatus = z.SaveStatus == 1 ? "Draft" : "Publish"
                    }).ToList<object>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
    }
}
