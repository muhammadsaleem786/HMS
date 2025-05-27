using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Repository.Repositories.Admin
{
    public static class adm_role_dtRepository
    {
        public static PaginationResult Pagination(this IRepository<adm_role_dt> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_role_dt, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayName, DisplayEmployees;
                bool OrderByName, OrderByEmployees;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("RoleName") > -1;
                    DisplayEmployees = PFilter.VisibleColumnInfoList.IndexOf("Employees") > -1;

                    predicate = (c => CompanyID == CompanyID
                    //(DisplayTaxCode && c.TaxCode.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    //(DisplayTaxName && c.TaxName.ToLower().Contains(PFilter.SearchText.ToLower())

                    //
                    );
                }

                IQueryable<adm_role_dt> filteredData = repository.Queryable().Where(predicate);

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
