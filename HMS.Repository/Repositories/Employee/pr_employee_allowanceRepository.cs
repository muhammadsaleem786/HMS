using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace HMS.Repository.Repositories.Employee
{
    public static class pr_employee_allowanceRepository
    {
        public static PaginationResult Pagination(this IRepository<pr_employee_allowance> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_employee_allowance, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayAllowanceName, DisplayAmntOrPercentage;
                bool OrderByAllowanceName, OrderByAmntOrPercentage;


                //if (!string.IsNullOrEmpty(PFilter.SearchText))
                //{
                //    DisplayAllowanceName = PFilter.VisibleColumnInfoList.IndexOf("AllowanceName") > -1;
                //    DisplayAmntOrPercentage = PFilter.VisibleColumnInfoList.IndexOf("AllowanceValue") > -1;
                //    predicate = (c =>
                //    c.CompanyID == CompanyID &&
                //    (DisplayAllowanceName && c.AllowanceName.ToLower().Contains(PFilter.SearchText.ToLower())
                //    || (DisplayAmntOrPercentage && c.AllowanceValue.ToString().Contains(PFilter.SearchText))
                //    ));
                //}

                IQueryable<pr_employee_allowance> filteredData = repository.Queryable().Where(predicate);

                //if (string.IsNullOrEmpty(PFilter.OrderBy))
                //    PFilter.OrderBy = "AllowanceName";

                //OrderByAllowanceName = PFilter.OrderBy.IndexOf("AllowanceName") > -1;
                //OrderByAmntOrPercentage = PFilter.OrderBy.IndexOf("AllowanceValue") > -1;

                //Expression<Func<pr_allowance, string>> orderingFunction = (c =>
                //                                               OrderByAllowanceName ? c.AllowanceName :
                //                                               OrderByAmntOrPercentage ? c.AllowanceValue.ToString() : "0"
                //                                              );

                //if (PFilter.IsOrderAsc)
                //    filteredData = filteredData.OrderBy(orderingFunction);
                //else
                //    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();
                //string currency = ConfigurationManager.AppSettings["Currency"];
                if (IgnorePaging)
                {
                    objResult.DataList = (from c in filteredData select c).ToList<object>();
                    //objResult.DataList = filteredData.Include(x => x.sys_drop_down_value).Select(s => new
                    //{
                    //    s.ID,
                    //    AllowanceName = s.Default ? s.AllowanceName + " (Default)" : s.AllowanceName + "",
                    //    AllowanceValue = s.AllowanceType == "P" ? currency + ": " + s.AllowanceValue + " %" : currency + ": " + s.AllowanceValue,
                    //}).ToList<object>();
                }
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    objResult.DataList = (from c in filteredData select c).ToList<object>();
                    //objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                    //    .Include(x => x.sys_drop_down_value).Select(s => new
                    //    {
                    //        s.ID,
                    //        AllowanceName = s.Default ? s.AllowanceName + " (Default)" : s.AllowanceName + "",
                    //        AllowanceValue = s.AllowanceType == "P" ? currency + ": " + s.AllowanceValue + " %" : currency + ": " + s.AllowanceValue,
                    //    }).ToList<object>();
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
