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
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Repository.Repositories.Items
{
    public static class pur_Sale_mfRepository
    {
        public static PaginationResult Pagination(this IRepository<pur_sale_mf> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pur_sale_mf, bool>> predicate = (e => e.CompanyID == CompanyID);
                bool DisplayNo, DisplayReference, DisplayItemName, DisplayDate, DisplayCompanyName;
                bool OrderByNo, OrderByReference, OrderByDate, OrderByCompanyName;
                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayNo = PFilter.VisibleColumnInfoList.IndexOf("Invoiceno") > -1;
                    DisplayReference = PFilter.VisibleColumnInfoList.IndexOf("amount") > -1;
                    DisplayDate = PFilter.VisibleColumnInfoList.IndexOf("date") > -1;
                    DisplayCompanyName = PFilter.VisibleColumnInfoList.IndexOf("CompanyName") > -1;
                    DisplayItemName = true;
                    predicate = (c => c.CompanyID == CompanyID &&
                     (DisplayNo && (c.ID.ToString()).Contains(PFilter.SearchText.ToLower()) ||
                        (DisplayReference && (c.Total.ToString()).ToLower().Contains(PFilter.SearchText.ToLower()) ||
                        (DisplayDate && (c.Date.ToString()).Contains(PFilter.SearchText.ToLower()) ||
                       (DisplayCompanyName && (c.adm_company.CompanyName.ToString()).Contains(PFilter.SearchText.ToLower()) ||
                       (DisplayItemName && c.pur_sale_dt.AsEnumerable().Any(a => a.adm_item.Name.ToLower().Contains(PFilter.SearchText.ToLower()
                    ))))))));
                }
                IQueryable<pur_sale_mf> filteredData = repository.Queryable().Where(predicate);
                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByNo = PFilter.OrderBy.IndexOf("Invoiceno") > -1;
                OrderByReference = PFilter.OrderBy.IndexOf("amount") > -1;
                OrderByDate = PFilter.OrderBy.IndexOf("date") > -1;
                OrderByCompanyName = PFilter.OrderBy.IndexOf("CompanyName") > -1;
                Expression<Func<pur_sale_mf, string>> orderingFunction = (c =>
                                                             OrderByNo ? c.ID.ToString() :
                                                             OrderByReference ? c.Total.ToString() :
                                                             OrderByDate ? c.Date.ToString() :
                                                             OrderByCompanyName ? c.adm_company.CompanyName.ToString() : ""
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
                    var result = filteredData.Include(x => x.pur_sale_dt).Include(x => x.adm_company)
                        .Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                   .Select(s => new
                   {
                       s.ID,
                       Invoiceno = s.ID,
                       date = s.Date.Month + "/" + s.Date.Day + "/" + s.Date.Year,
                       amount = s.Total,
                       CompanyName = s.adm_company.CompanyName,
                       SaleType = s.sys_drop_down_value.Value,
                       CustomerName = s.emr_patient_mf.PatientName,
                       ReturnInvoiceId = s.ReturnInvoiceId,
                   }).ToList();
                    objResult.DataList = result.AsEnumerable().Select(z => new
                    {
                        ID = z.ID,
                        Invoiceno = z.Invoiceno,
                        date = z.date,
                        amount = z.amount,
                        CompanyName = z.CompanyName,
                        SaleType = z.SaleType,
                        CustomerName = z.CustomerName,
                        ReturnInvoiceId = z.ReturnInvoiceId,
                    }).ToList<object>();
                }
                else
                {
                    var result = filteredData.Include(x => x.pur_sale_dt).Include(a => a.adm_company)
                   .Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                        .Select(s => new
                        {
                            s.ID,
                            Invoiceno = s.ID,
                            date = s.Date.Month + "/" + s.Date.Day + "/" + s.Date.Year,
                            amount = s.Total,
                            CompanyName = s.adm_company.CompanyName,
                            SaleType = s.sys_drop_down_value.Value,
                            CustomerName = s.emr_patient_mf.PatientName,
                            ReturnInvoiceId = s.ReturnInvoiceId,
                        }).ToList();

                    objResult.DataList = result.AsEnumerable().Select(z => new
                    {
                        ID = z.ID,
                        Invoiceno = z.Invoiceno,
                        date = z.date,
                        amount = z.amount,
                        CompanyName = z.CompanyName,
                        SaleType = z.SaleType,
                        CustomerName = z.CustomerName,
                        ReturnInvoiceId = z.ReturnInvoiceId
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
