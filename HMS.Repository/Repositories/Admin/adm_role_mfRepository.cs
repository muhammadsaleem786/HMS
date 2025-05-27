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

namespace HMS.Repository.Repositories.Admin
{
    public static class adm_role_mfRepository
    {
        public static PaginationResult Pagination(this IRepository<adm_role_mf> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_role_mf, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayName, DisplayEmployees;
                bool OrderByName, OrderByEmployees;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("RoleName") > -1;
                    DisplayEmployees = PFilter.VisibleColumnInfoList.IndexOf("Employees") > -1;

                    predicate = (c => c.CompanyID == CompanyID &&
                    (DisplayName && c.RoleName.ToLower().Contains(PFilter.SearchText.ToLower())
                     || (DisplayEmployees && c.adm_user_company.Where(e => e.RoleID == c.ID).Select(x => x.EmployeeID).Count().ToString().Contains(PFilter.SearchText.ToLower()) 
                    )));
                }

                IQueryable<adm_role_mf> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("RoleName") > -1;
                OrderByEmployees = PFilter.OrderBy.IndexOf("Employees") > -1;

                Expression<Func<adm_role_mf, string>> orderingFunction = (c =>
                                                              OrderByName ? c.RoleName :
                                                                OrderByEmployees ? c.adm_user_company.Where(e => e.RoleID == c.ID).Select(x => x.EmployeeID).Count().ToString() : "0"
                                                             );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    //objResult.DataList = (from c in filteredData select c).ToList<object>();
                    objResult.DataList = filteredData
                       .Include(i => i.adm_user_company)
                       .Select(s => new
                       {
                           s.ID,
                           s.RoleName,
                           Employees = s.adm_user_company.Where(e => e.RoleID == s.ID).Select(x => x.EmployeeID).Count()

                       }).ToList<object>();
                }
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Include(i => i.adm_user_company)
                       .Select(s => new
                       {
                           s.ID,
                           s.RoleName,
                           Employees = s.adm_user_company.Where (e=> e.RoleID == s.ID).Select(x => x.EmployeeID).Count()

                       }).ToList<object>();
                    //objResult.DataList = (from c in PageResult select c).ToList<object>();
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
