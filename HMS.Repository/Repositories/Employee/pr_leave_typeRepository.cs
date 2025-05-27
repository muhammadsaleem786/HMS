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
    public static class pr_leave_typeRepository
    {
        public static PaginationResult Pagination(this IRepository<pr_leave_type> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_leave_type, bool>> predicate = (e => e.CompanyID == CompanyID && (e.Category == "S" || e.Category == "V"));

                bool DisplayTypeName, DisplayCategory, DisplayAccuruelFrequencyID, DisplayEarndValue;
                bool OrderByTypeName, OrderByCategory, OrderByAccuruelFrequencyID, OrderByEarndValue;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayTypeName = PFilter.VisibleColumnInfoList.IndexOf("TypeName") > -1;
                    DisplayCategory = PFilter.VisibleColumnInfoList.IndexOf("Category") > -1;
                    DisplayAccuruelFrequencyID = PFilter.VisibleColumnInfoList.IndexOf("AccuruelFrequencyID") > -1;
                    DisplayEarndValue = PFilter.VisibleColumnInfoList.IndexOf("EarndValue") > -1;
                    predicate = (c =>
                    c.CompanyID == CompanyID &&
                    (DisplayTypeName && c.TypeName.ToLower().Contains(PFilter.SearchText.ToLower())
                    || (DisplayCategory && (c.Category == "V" ? "Vacation" : "Sick Leave").ToLower().Contains(PFilter.SearchText.ToLower())
                    || (DisplayAccuruelFrequencyID && c.sys_drop_down_value.Value.ToLower().ToString().Contains(PFilter.SearchText.ToLower())
                    || (DisplayEarndValue && c.EarnedValue.ToString().Contains(PFilter.SearchText)
                    )))));
                }

                IQueryable<pr_leave_type> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "TypeName";

                OrderByTypeName = PFilter.OrderBy.IndexOf("TypeName") > -1;
                OrderByCategory = PFilter.OrderBy.IndexOf("Category") > -1;
                OrderByAccuruelFrequencyID = PFilter.OrderBy.IndexOf("AccuruelFrequencyID") > -1;
                OrderByEarndValue = PFilter.OrderBy.IndexOf("EarndValue") > -1;

                Expression<Func<pr_leave_type, string>> orderingFunction = (c =>
                                                              OrderByTypeName ? c.TypeName :
                                                              OrderByCategory ? c.Category :
                                                              OrderByEarndValue ? c.EarnedValue.ToString() :
                                                              OrderByAccuruelFrequencyID ? c.AccrualFrequencyID.ToString() : "0"
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    objResult.DataList = filteredData.Include(x => x.sys_drop_down_value).Select(s => new
                    {
                        s.ID,
                        s.TypeName,
                        Category = s.Category == "V" ? "Vacation" : "Sick Leave",
                        AccuruelFrequencyID = s.sys_drop_down_value.Value,
                        EarndValue = s.EarnedValue,
                    }).ToList<object>();

                    objResult.DataList = (from c in filteredData select c).ToList<object>();
                }
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    //objResult.DataList = (from c in filteredData select c).ToList<object>();
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                        .Include(x => x.sys_drop_down_value).Select(s => new
                        {
                            s.ID,
                            s.TypeName,
                            Category = s.Category == "V" ? "Vacation" : "Sick Leave",
                            AccuruelFrequencyID = s.sys_drop_down_value.Value,
                            EarndValue = s.EarnedValue,
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
