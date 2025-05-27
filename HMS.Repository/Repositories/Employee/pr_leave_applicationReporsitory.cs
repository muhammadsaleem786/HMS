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
    public static class pr_leave_applicationReporsitory
    {
        public static PaginationResult Pagination(this IRepository<pr_leave_application> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_leave_application, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayName, DisplayCategory, DisplayLeaveType, DisplayHours, DisplayDuration;
                bool OrderByName, OrderByCategory, OrderByLeaveType, OrderByHours, OrderByDuration;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("EmployeeName") > -1;
                    DisplayCategory = PFilter.VisibleColumnInfoList.IndexOf("Catgory") > -1;
                    DisplayLeaveType = PFilter.VisibleColumnInfoList.IndexOf("LeaveTypeID") > -1;
                    DisplayHours = PFilter.VisibleColumnInfoList.IndexOf("Hours") > -1;
                    DisplayDuration = PFilter.VisibleColumnInfoList.IndexOf("LeaveTypeID") > -1;
                    predicate = (c =>
                    c.CompanyID == CompanyID && (c.LeaveTypeID == 62 || c.LeaveTypeID == 63) &&
                    (DisplayName && (c.pr_employee_mf.FirstName + " " + c.pr_employee_mf.LastName).ToLower().Replace("  ", " ").Contains(PFilter.SearchText.ToLower())
                    || (DisplayCategory && (c.pr_leave_type.Category == "V" ? "Vacation" : c.pr_leave_type.Category == "L" ? "LWP" : "Sick Leave").ToLower().Contains(PFilter.SearchText.ToLower()))
                      || (DisplayLeaveType && c.pr_leave_type.TypeName.ToString().ToLower().Contains(PFilter.SearchText.ToLower()))
                       || (DisplayHours && c.Hours.ToString().Contains(PFilter.SearchText.ToLower()))
                       || (DisplayDuration && (c.FromDate).ToString().Contains(PFilter.SearchText.ToLower()))
                       || (DisplayDuration && (c.ToDate).ToString().Contains(PFilter.SearchText.ToLower()))
                    ));
                }

                IQueryable<pr_leave_application> filteredData = repository.Queryable().Where(predicate);//.Include(x => x.pr_employee_mf).Include(x => x.pr_leave_type);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("EmployeeName") > -1;
                OrderByCategory = PFilter.OrderBy.IndexOf("Catgory") > -1;
                OrderByLeaveType = PFilter.OrderBy.IndexOf("LeaveTypeID") > -1;
                OrderByHours = PFilter.OrderBy.IndexOf("Hours") > -1;
                Expression<Func<pr_leave_application, string>> orderingFunction = (c =>
                                                              OrderByName ? (c.pr_employee_mf.FirstName + " " + c.pr_employee_mf.LastName) :
                                                              OrderByCategory ? c.pr_leave_type.Category.ToString() :
                                                              OrderByLeaveType ? c.pr_leave_type.TypeName.ToString() :
                                                             OrderByHours ? c.Hours.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();
                //string currency = ConfigurationManager.AppSettings["Currency"];

                //var dataList = filteredData.Select(s => new
                //{
                //    s.ID,
                //    EmployeeID = s.pr_employee_mf.FirstName + " " + s.pr_employee_mf.LastName,
                //    LeaveTypeID = s.pr_leave_type.TypeName,
                //    Category = s.pr_leave_type.Category == "V" ? "Vacation" : "sick Leave",
                //    s.Hours,
                //    Duration = s.FromDate + " - " + s.ToDate,
                //}).ToList<object>();

                if (IgnorePaging)
                {
                    //objResult.DataList = (from c in filteredData select c).ToList<object>();
                    objResult.DataList = filteredData.Select(s => new
                    {
                        s.ID,
                        EmployeeName = s.pr_employee_mf.FirstName + " " + s.pr_employee_mf.LastName,
                        LeaveTypeID = s.pr_leave_type.TypeName,
                        Category = s.pr_leave_type.Category == "V" ? "Vacation" : s.pr_leave_type.Category == "L" ? "LWP" : "sick Leave",
                        s.Hours,
                        s.FromDate,
                        s.ToDate
                    }).ToList().Select(z => new {
                        z.ID,
                        z.EmployeeName,
                        z.LeaveTypeID,
                        z.Category,
                        z.Hours,
                        Duration = z.FromDate.ToString("dd/MM/yyyy") + " - " + z.ToDate.ToString("dd/MM/yyyy"),
                    }).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                        .Select(s => new
                        {
                            s.ID,
                            EmployeeName = s.pr_employee_mf.FirstName + " " + s.pr_employee_mf.LastName,
                            LeaveTypeID = s.pr_leave_type.TypeName,
                            Category = s.pr_leave_type.Category == "V" ? "Vacation" : s.pr_leave_type.Category == "L" ? "LWP" : "sick Leave",
                            s.Hours,
                            s.FromDate,
                            s.ToDate
                        }).ToList().Select(z => new {
                            z.ID,
                            z.EmployeeName,
                            z.LeaveTypeID,
                            z.Category,
                            z.Hours,
                            Duration = z.FromDate.ToString("dd/MM/yyyy") + " - " + z.ToDate.ToString("dd/MM/yyyy"),
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
