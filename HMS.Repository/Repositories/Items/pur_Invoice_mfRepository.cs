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
    public static class pur_Invoice_mfRepository
    {
        public static PaginationResult Pagination(this IRepository<pur_invoice_mf> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pur_invoice_mf, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayNo, DisplayReference, DisplayCompanyName;
                bool OrderByNo, OrderByReference, OrderByCompanyName;
                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayNo = PFilter.VisibleColumnInfoList.IndexOf("OrderNo") > -1;
                    DisplayReference = PFilter.VisibleColumnInfoList.IndexOf("BillNo") > -1;
                    DisplayCompanyName = PFilter.VisibleColumnInfoList.IndexOf("CompanyName") > -1;
                    predicate = (c => c.CompanyID == CompanyID &&
                    (DisplayNo && (c.OrderNo).Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayReference && (c.BillNo).ToString().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayCompanyName && c.pur_vendor.CompanyName.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                  ))));
                }
                if (IgnorePaging)
                {
                    predicate = c => c.CompanyID == CompanyID && c.Total > (c.pur_payment.Sum(a => (decimal?)a.Amount) ?? 0);
                }
                IQueryable<pur_invoice_mf> filteredData = repository.Queryable().Where(predicate);
                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByNo = PFilter.OrderBy.IndexOf("OrderNo") > -1;
                OrderByReference = PFilter.OrderBy.IndexOf("BillNo") > -1;
                OrderByCompanyName = PFilter.OrderBy.IndexOf("CompanyName") > -1;
                Expression<Func<pur_invoice_mf, string>> orderingFunction = (c =>
                                                             OrderByNo ? c.OrderNo :
                                                              OrderByReference ? c.BillNo.ToString() :
                                                             OrderByCompanyName ? c.pur_vendor.CompanyName.ToString() : ""
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
                    var result = filteredData.Include(x => x.pur_invoice_dt).Include(x => x.pur_vendor).Include(z => z.pur_payment)

                        .Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord).Select(s => new
                        {
                            s.ID,
                            Invoiceno = s.BillNo,
                            status = s.SaveStatus == 1 ? "Draft" : "Publish",
                            date = s.BillDate.Month + "/" + s.BillDate.Day + "/" + s.BillDate.Year,
                            duedate = s.DueDate.Month + "/" + s.DueDate.Day + "/" + s.DueDate.Year,
                            name = s.pur_vendor.FirstName + " " + s.pur_vendor.LastName,
                            amount = s.Total,
                            DueBalance = s.Total - (s.pur_payment.Count() > 0 ? s.pur_payment.Sum(a => a.Amount) : 0),
                            CompanyName = s.pur_vendor.CompanyName,
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
                        DueBalance = z.DueBalance,
                        status = z.status,
                        CompanyName = z.CompanyName,
                        isSelected = z.isSelected,
                        SaveStatus = z.SaveStatus == 1 ? "Draft" : "Publish"
                    }).ToList<object>();
                }
                else
                {
                    var result = filteredData.Include(x => x.pur_invoice_dt).Include(a => a.pur_vendor).Include(z => z.pur_payment)
                        .Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                   .Select(s => new
                   {
                       s.ID,
                       Invoiceno = s.BillNo,
                       status = s.SaveStatus == 1 ? "Draft" : "Publish",
                       date = s.BillDate.Month + "/" + s.BillDate.Day + "/" + s.BillDate.Year,
                       duedate = s.DueDate.Month + "/" + s.DueDate.Day + "/" + s.DueDate.Year,
                       name = s.pur_vendor.FirstName + " " + s.pur_vendor.LastName,
                       amount = s.Total,
                       DueBalance = s.Total - (s.pur_payment.Count() > 0 ? s.pur_payment.Sum(a => a.Amount) : 0),
                       isSelected = false,
                       CompanyName = s.pur_vendor.CompanyName,
                       SaveStatus = s.SaveStatus,
                   }).OrderByDescending(a => a.ID).ToList();
                    objResult.DataList = result.AsEnumerable().Select(z => new InvoiceResponse
                    {
                        ID = z.ID,
                        Invoiceno = z.Invoiceno.ToString(),
                        date = z.date,
                        duedate = z.duedate,
                        name = z.name,
                        amount = z.amount,
                        DueBalance = z.DueBalance,
                        status = z.status,
                        CompanyName = z.CompanyName,
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
