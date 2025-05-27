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
    public static class pr_departmentRepository
    {
        public static PaginationResult Pagination(this IRepository<pr_department> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_department, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayDepartmentName, DisplayEmployees;
                bool OrderByDepartmentName, OrderByEmployees;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayDepartmentName = PFilter.VisibleColumnInfoList.IndexOf("DepartmentName") > -1;
                    DisplayEmployees = PFilter.VisibleColumnInfoList.IndexOf("Employees") > -1;
                    predicate = (c =>
                    c.CompanyID == CompanyID &&
                    (DisplayDepartmentName && c.DepartmentName.ToLower().Contains(PFilter.SearchText.ToLower())
                    || (DisplayEmployees && c.pr_employee_mf.Count().ToString().Contains(PFilter.SearchText))
                    ));
                }

                IQueryable<pr_department> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "DepartmentName";

                OrderByDepartmentName = PFilter.OrderBy.IndexOf("DepartmentName") > -1;
                OrderByEmployees = PFilter.OrderBy.IndexOf("Employees") > -1;

                Expression<Func<pr_department, string>> orderingFunction = (c =>
                                                              OrderByDepartmentName ? c.DepartmentName : ""
                                                              //OrderByEmployees ? c.pr_employee_mf.Count() : 0
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                //int record = objResult.TotalRecord;
                //double NoOfPagesWithfloat = (double)record / RecordPerPage;
                //NoOfPagesWithfloat += (NoOfPagesWithfloat > (record / RecordPerPage)) ? 1 : 0;
                //if (NoOfPagesWithfloat < CurrentPageNo)
                //{
                //    CurrentPageNo = CurrentPageNo - 1;
                //    PFilter.SkipRecord = (CurrentPageNo - 1) * RecordPerPage;
                //}

                if (IgnorePaging)
                {
                    objResult.DataList = filteredData.Include(x => x.pr_employee_mf).Select(s => new
                    {
                        s.ID,
                        s.DepartmentName,
                        Employees = s.pr_employee_mf.Count
                    }).ToList<object>();

                    // objResult.DataList = (from c in filteredData select c).ToList<object>();
                }
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                        .Include(x => x.pr_employee_mf).Select(s => new
                        {
                            s.ID,
                            s.DepartmentName,
                            Employees = s.pr_employee_mf.Count
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
