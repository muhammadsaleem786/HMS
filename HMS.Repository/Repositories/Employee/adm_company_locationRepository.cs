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

namespace HMS.Repository.Repositories.Employee
{
    public static class adm_company_locationRepository
    {
        public static PaginationResult Pagination(this IRepository<adm_company_location> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_company_location, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayLocationName, DisplayEmployees;
                bool OrderByLocationName, OrderByEmployees;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayLocationName = PFilter.VisibleColumnInfoList.IndexOf("LocationName") > -1;
                    DisplayEmployees = PFilter.VisibleColumnInfoList.IndexOf("Employees") > -1;
                    predicate = (c =>
                    c.CompanyID == CompanyID &&
                    (DisplayLocationName && c.LocationName.ToLower().Contains(PFilter.SearchText.ToLower())
                    || (DisplayEmployees && c.pr_employee_mf.Count().ToString().Contains(PFilter.SearchText))

                    ));
                }

                IQueryable<adm_company_location> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "LocationName";

                OrderByLocationName = PFilter.OrderBy.IndexOf("LocationName") > -1;
                OrderByEmployees = PFilter.OrderBy.IndexOf("Employees") > -1;

                Expression<Func<adm_company_location, string>> orderingFunction = (c =>
                                                              OrderByLocationName ? c.LocationName :
                                                              OrderByEmployees ? c.pr_employee_mf.Count().ToString() : "0"
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    objResult.DataList = filteredData.Include(x => x.pr_employee_mf).Select(s => new
                    {
                        s.ID,
                        s.LocationName,
                        Employees = s.pr_employee_mf.Count
                    }).ToList<object>();

                    //objResult.DataList = (from c in filteredData select c).ToList<object>();
                }
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                        .Include(x => x.pr_employee_mf).Select(s => new
                        {
                            s.ID,
                            s.LocationName,
                            Employees = s.pr_employee_mf.Count
                        }).ToList<object>();
                    //objResult.DataList = (from c in filteredData select c).ToList<object>();
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
