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
    public static class pr_pay_scheduleRepository
    {
        public static PaginationResult Pagination(this IRepository<pr_pay_schedule> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_pay_schedule, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayScheduleName, DisplayPayPeriod;
                bool OrderByScheduleName, OrderByPayPeriod;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayScheduleName = PFilter.VisibleColumnInfoList.IndexOf("ScheduleName") > -1;
                    DisplayPayPeriod = PFilter.VisibleColumnInfoList.IndexOf("PayTypeID") > -1;
                    predicate = (c =>
                    c.CompanyID == CompanyID &&
                    (DisplayScheduleName && c.ScheduleName.ToLower().Contains(PFilter.SearchText.ToLower())
                    || (DisplayPayPeriod && c.sys_drop_down_value.Value.ToLower().Contains(PFilter.SearchText.ToLower())
                    )));
                }

                IQueryable<pr_pay_schedule> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ScheduleName";

                OrderByScheduleName = PFilter.OrderBy.IndexOf("ScheduleName") > -1;
                OrderByPayPeriod = PFilter.OrderBy.IndexOf("PayTypeID") > -1;

                Expression<Func<pr_pay_schedule, string>> orderingFunction = (c =>
                                                              OrderByScheduleName ? c.ScheduleName :
                                                              OrderByPayPeriod ? c.sys_drop_down_value.Value : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    objResult.DataList = filteredData
                        .Include(i => i.sys_drop_down_value)
                        .Select(s => new
                        {
                            s.ID,
                            ScheduleName = s.Active ? s.ScheduleName + " (Default)" : s.ScheduleName,
                            PayTypeID = s.sys_drop_down_value.Value
                        }).ToList<object>();
                    //objResult.DataList = (from c in filteredData select c).ToList<object>();
                }
                else
                {
                    //var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    //objResult.DataList = (from c in PageResult select c).ToList<object>();
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                        .Include(i => i.sys_drop_down_value)
                        .Select(s => new
                        {
                            s.ID,
                            ScheduleName = s.Active ? s.ScheduleName + " (Default)" : s.ScheduleName,
                            PayTypeID = s.sys_drop_down_value.Value
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
