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
    public static class pr_time_entryRepository
    {
        public static PaginationResult Pagination(this IRepository<pr_time_entry> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_time_entry, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayEmployee, DisplayDate, DisplayTimeIn, DisplayTimeOut, DisplayStatus;
                bool OrderByEmployee, OrderByDate, OrderByTimeIn, OrderByTimeOut, OrderByStatus;

                IQueryable<pr_time_entry> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                objResult.TotalRecord = filteredData.Count();
                List<pr_time_entryModel> list = new List<pr_time_entryModel>();
                if (IgnorePaging)
                {
                    list = filteredData.Include(x => x.pr_employee_mf).Include(x => x.sys_drop_down_value)
                         .Select(s => new
                         {
                             s.ID,
                             Employee = s.pr_employee_mf.FirstName + " " + s.pr_employee_mf.LastName,
                             Date = s.TimeIn,
                             s.TimeOut,
                             s.TimeIn,
                             Status = s.sys_drop_down_value.Value,
                         }).ToList().Select(z => new pr_time_entryModel
                         {
                             ID = z.ID,
                             Employee = z.Employee,
                             Date = z.Date.ToString("dd/MM/yyyy"),
                             TimeOut = z.TimeOut.ToString("hh:mm tt"),
                             TimeIn = z.TimeIn.ToString("hh:mm tt"),
                             Status = z.Status
                         }).OrderBy(x => x.Date).ToList();
                }
                else
                {

                    list = filteredData.OrderBy(x => x.ID).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                        .Include(x => x.pr_employee_mf).Include(x => x.sys_drop_down_value)
                        .Select(s => new
                        {
                            s.ID,
                            Employee = s.pr_employee_mf.FirstName + " " + s.pr_employee_mf.LastName,
                            Date = s.TimeIn,
                            s.TimeOut,
                            s.TimeIn,
                            Status = s.sys_drop_down_value.Value,
                        }).ToList().Select(z => new pr_time_entryModel
                        {
                            ID = z.ID,
                            Employee = z.Employee,
                            Date = z.Date.ToString("dd/MM/yyyy"),
                            TimeOut = z.TimeOut.ToString("hh:mm tt"),
                            TimeIn = z.TimeIn.ToString("hh:mm tt"),
                            Status = z.Status
                        }).OrderBy(x => x.Date).ToList();
                }


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayEmployee = PFilter.VisibleColumnInfoList.IndexOf("Employee") > -1;
                    DisplayDate = PFilter.VisibleColumnInfoList.IndexOf("Date") > -1;
                    DisplayTimeIn = PFilter.VisibleColumnInfoList.IndexOf("TimeIn") > -1;
                    DisplayTimeOut = PFilter.VisibleColumnInfoList.IndexOf("TimeOut") > -1;
                    DisplayStatus = PFilter.VisibleColumnInfoList.IndexOf("Status") > -1;

                    list = list.Where(c =>
                     (DisplayEmployee && c.Employee.ToLower().Replace("  ", " ").Contains(PFilter.SearchText.ToLower())
                     || (DisplayDate && c.Date.ToLower().Contains(PFilter.SearchText.ToLower())
                      || (DisplayTimeIn && c.TimeIn.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                       || (DisplayTimeOut && c.TimeOut.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                        || (DisplayStatus && c.Status.ToLower().Contains(PFilter.SearchText.ToLower())
                     )))))).ToList();
                }

                OrderByEmployee = PFilter.OrderBy.IndexOf("Employee") > -1;
                OrderByDate = PFilter.OrderBy.IndexOf("Date") > -1;
                OrderByTimeIn = PFilter.OrderBy.IndexOf("TimeIn") > -1;
                OrderByTimeOut = PFilter.OrderBy.IndexOf("TimeOut") > -1;
                OrderByStatus = PFilter.OrderBy.IndexOf("Status") > -1;

                Expression<Func<pr_time_entryModel, string>> orderingFunction = (c =>
                                                              OrderByEmployee ? c.Employee :
                                                              OrderByDate ? c.Date :
                                                              OrderByTimeIn ? c.TimeIn.ToString() :
                                                              OrderByTimeOut ? c.TimeOut.ToString() :
                                                              OrderByStatus ? c.Status : ""
                                                              );


                IQueryable<pr_time_entryModel> prList = list.AsQueryable();
                if (PFilter.IsOrderAsc)
                    prList = prList.OrderBy(orderingFunction);
                else
                    prList = prList.OrderByDescending(orderingFunction);

                objResult.TotalRecord = prList.Count();
                objResult.DataList = prList.ToList<Object>();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }

        public static PaginationResult GetTimePaginationList(this IRepository<pr_time_entry> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_time_entry, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayEmployee, DisplayDate, DisplayTimeIn, DisplayTimeOut, DisplayStatus;
                bool OrderByEmployee, OrderByDate, OrderByTimeIn, OrderByTimeOut, OrderByStatus;

                IQueryable<pr_time_entry> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                objResult.TotalRecord = filteredData.Count();
                List<pr_time_entryModel> list = new List<pr_time_entryModel>();
                if (IgnorePaging)
                {
                    list = filteredData.Include(x => x.pr_employee_mf).Include(x => x.sys_drop_down_value)
                         .Select(s => new
                         {
                             s.ID,
                             Employee = s.pr_employee_mf.FirstName + " " + s.pr_employee_mf.LastName,
                             Date = s.TimeIn,
                             s.TimeOut,
                             s.TimeIn,
                             Status = s.sys_drop_down_value.Value,
                         }).ToList().Select(z => new pr_time_entryModel
                         {
                             ID = z.ID,
                             Employee = z.Employee,
                             Date = z.Date.ToString("dd/MM/yyyy"),
                             TimeOut = z.TimeOut.ToString("hh:mm tt"),
                             TimeIn = z.TimeIn.ToString("hh:mm tt"),
                             Status = z.Status
                         }).OrderBy(x => x.Date).ToList();
                }
                else
                {

                    var datalist = filteredData
                   .Include(x => x.pr_employee_mf).Include(x => x.sys_drop_down_value)
                   .Select(z => new
                   {
                       z.EmployeeID,
                       Employee = z.pr_employee_mf.FirstName + " " + z.pr_employee_mf.LastName,
                       z.ID,
                       z.TimeIn,
                       z.TimeOut,
                       Status = z.sys_drop_down_value.Value,
                       z.StatusID
                   }).OrderBy(d => d.Employee).ToList();

                    objResult.DataList = datalist.Select(z => new
                    {
                        z.EmployeeID,
                        z.Employee,
                    }).Distinct().Select(s => new
                    {
                        s.EmployeeID,
                        s.Employee,
                        TimeAttendanceListdt = datalist.Where(x => x.EmployeeID == s.EmployeeID)
                        .Select(a => new
                        {
                            a.ID,
                            a.EmployeeID,
                            a.TimeIn,
                            a.TimeOut,
                            a.StatusID,
                            a.Status
                        })
                        .ToList()
                    }).ToList<object>();

                }


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayEmployee = PFilter.VisibleColumnInfoList.IndexOf("Employee") > -1;
                    DisplayDate = PFilter.VisibleColumnInfoList.IndexOf("Date") > -1;
                    DisplayTimeIn = PFilter.VisibleColumnInfoList.IndexOf("TimeIn") > -1;
                    DisplayTimeOut = PFilter.VisibleColumnInfoList.IndexOf("TimeOut") > -1;
                    DisplayStatus = PFilter.VisibleColumnInfoList.IndexOf("Status") > -1;

                    list = list.Where(c =>
                     (DisplayEmployee && c.Employee.ToLower().Replace("  ", " ").Contains(PFilter.SearchText.ToLower())
                     || (DisplayDate && c.Date.ToLower().Contains(PFilter.SearchText.ToLower())
                      || (DisplayTimeIn && c.TimeIn.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                       || (DisplayTimeOut && c.TimeOut.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                        || (DisplayStatus && c.Status.ToLower().Contains(PFilter.SearchText.ToLower())
                     )))))).ToList();
                }

                OrderByEmployee = PFilter.OrderBy.IndexOf("Employee") > -1;
                OrderByDate = PFilter.OrderBy.IndexOf("Date") > -1;
                OrderByTimeIn = PFilter.OrderBy.IndexOf("TimeIn") > -1;
                OrderByTimeOut = PFilter.OrderBy.IndexOf("TimeOut") > -1;
                OrderByStatus = PFilter.OrderBy.IndexOf("Status") > -1;

                Expression<Func<pr_time_entryModel, string>> orderingFunction = (c =>
                                                              OrderByEmployee ? c.Employee :
                                                              OrderByDate ? c.Date :
                                                              OrderByTimeIn ? c.TimeIn.ToString() :
                                                              OrderByTimeOut ? c.TimeOut.ToString() :
                                                              OrderByStatus ? c.Status : ""
                                                              );


                IQueryable<pr_time_entryModel> prList = list.AsQueryable();
                if (PFilter.IsOrderAsc)
                    prList = prList.OrderBy(orderingFunction);
                else
                    prList = prList.OrderByDescending(orderingFunction);

                objResult.TotalRecord = prList.Count();
                objResult.DataList = prList.ToList<Object>();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
    }
}
