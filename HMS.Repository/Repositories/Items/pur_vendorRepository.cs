using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Repository.Repositories.Items
{
    public static class pur_vendorRepository
    {
        public static PaginationResult Pagination(this IRepository<pur_vendor> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pur_vendor, bool>> predicate = (e => e.CompanyID == CompanyID);
                bool DisplayName, DisplaySKU, DisplayDescription, DisplayRate;
                bool OrderByName, OrderBySKU;
                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("FirstName") > -1;
                    DisplaySKU = PFilter.VisibleColumnInfoList.IndexOf("LastName") > -1;
                    DisplayDescription = PFilter.VisibleColumnInfoList.IndexOf("CompanyName") > -1;
                    DisplayRate = PFilter.VisibleColumnInfoList.IndexOf("VendorPhone") > -1;
                    predicate = (c => c.CompanyID == CompanyID &&
                    (DisplayName && (c.FirstName).Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayName && (c.LastName).Contains(PFilter.SearchText.ToLower()) ||
                    (DisplaySKU && (c.CompanyName).ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplaySKU && (c.VendorPhone).ToLower().Contains(PFilter.SearchText.ToLower())
                  )))));
                }
                IQueryable<pur_vendor> filteredData = repository.Queryable().Where(predicate);
                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByName = PFilter.OrderBy.IndexOf("FirstName") > -1;
                OrderBySKU = PFilter.OrderBy.IndexOf("LastName") > -1;
                Expression<Func<pur_vendor, string>> orderingFunction = (c =>
                                                OrderByName ? c.FirstName :
                                                OrderBySKU ? c.LastName : ""
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
                    objResult.DataList = filteredData.Include(a => a.adm_company).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                      .Select(s => new
                      {
                          s.ID,
                          FirstName = s.FirstName,
                          LastName = s.LastName,
                          CompanyName = s.CompanyName,
                          VendorPhone = s.VendorPhone,
                          VendorEmail = s.VendorEmail,
                          Address = s.Address,
                          Address2 = s.Address2,
                      }).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData.Include(a => a.adm_company).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
              .Select(s => new
              {
                  s.ID,
                  FirstName = s.FirstName,
                  LastName = s.LastName,
                  CompanyName = s.CompanyName,
                  VendorPhone = s.VendorPhone,
                  VendorEmail = s.VendorEmail,
                  Address = s.Address,
                  Address2 = s.Address2,
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
