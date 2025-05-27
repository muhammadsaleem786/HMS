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
    public static class pr_employee_leaveRepository
    {
        public static PaginationResult Pagination(this IRepository<pr_employee_leave> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_employee_leave, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayName, DisplayEmployees;
                bool OrderByName, OrderByEmployees;


                //if (!string.IsNullOrEmpty(PFilter.SearchText))
                //{
                //    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("RoleName") > -1;
                //    DisplayEmployees = PFilter.VisibleColumnInfoList.IndexOf("Employees") > -1;

                //    predicate = (c => CompanyID == CompanyID
                //    //(DisplayTaxCode && c.TaxCode.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                //    //(DisplayTaxName && c.TaxName.ToLower().Contains(PFilter.SearchText.ToLower())

                //    //
                //    );
                //}

                IQueryable<pr_employee_leave> filteredData = repository.Queryable().Where(predicate);

                //if (string.IsNullOrEmpty(PFilter.OrderBy))
                //    PFilter.OrderBy = "TaxCode";

                //OrderByTaxCode = PFilter.OrderBy.IndexOf("TaxCode") > -1;
                //OrderByTaxName = PFilter.OrderBy.IndexOf("TaxName") > -1;
                //OrderByTaxRate = PFilter.OrderBy.IndexOf("TaxRate") > -1;

                //Expression<Func<adm_tax, string>> orderingFunction = (c =>
                //                                              OrderByTaxCode ? c.TaxCode :
                //                                              OrderByTaxName ? c.TaxName : ""
                //                                              );

                //if (PFilter.IsOrderAsc)
                //    filteredData = filteredData.OrderBy(orderingFunction);
                //else
                //    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                    objResult.DataList = (from c in filteredData select c).ToList<object>();
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    objResult.DataList = (from c in PageResult select c).ToList<object>();
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
