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

namespace HMS.Repository.Repositories.Appointment
{
    public static class emr_expenseRepository
    {
        public static PaginationResult Pagination(this IRepository<emr_expense> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_expense, bool>> predicate = (e => e.CompanyId == ID);

                bool DisplayDate, DisplayCategory, DisplayAmount;
                bool OrderByDate, OrderByCategory, OrderByAmount;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayDate = PFilter.VisibleColumnInfoList.IndexOf("Date") > -1;
                    DisplayCategory = PFilter.VisibleColumnInfoList.IndexOf("CategoryId") > -1;
                    DisplayAmount = PFilter.VisibleColumnInfoList.IndexOf("Amount") > -1;
                    predicate = (c =>
                    (DisplayDate && c.Date.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayAmount && c.Amount.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                    )));
                }

                IQueryable<emr_expense> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByDate = PFilter.OrderBy.IndexOf("Date") > -1;
                OrderByCategory = PFilter.OrderBy.IndexOf("CategoryId") > -1;
                OrderByAmount = PFilter.OrderBy.IndexOf("Amount") > -1;

                Expression<Func<emr_expense, string>> orderingFunction = (c =>
                                                              OrderByDate ? c.Date.ToString() : 
                                                              OrderByCategory ? c.CategoryId.ToString() :
                                                              OrderByAmount ? c.Amount.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {

                    objResult.DataList = filteredData
                        .Select(s => new
                        {
                            s.ID,
                            Date = s.Date,
                            Category = s.sys_drop_down_value.Value,
                            Amount = s.Amount,
                            CreatedDate = s.CreatedDate
                        }).OrderByDescending(A => A.ID).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           Date = s.Date,
                           Category = s.sys_drop_down_value.Value,
                           Amount = s.Amount,
                           CreatedDate=s.CreatedDate
                       }).OrderByDescending(A => A.ID).ToList<object>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
        public static PaginationResult PaginationWithParm(this IRepository<emr_expense> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_expense, bool>> predicate = (e => e.CompanyId == ID && (FilterID == "0" || e.CategoryId.ToString() == FilterID));

                bool DisplayDate, DisplayCategory, DisplayAmount;
                bool OrderByDate, OrderByCategory, OrderByAmount;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayDate = PFilter.VisibleColumnInfoList.IndexOf("Date") > -1;
                    DisplayCategory = PFilter.VisibleColumnInfoList.IndexOf("CategoryId") > -1;
                    DisplayAmount = PFilter.VisibleColumnInfoList.IndexOf("Amount") > -1;
                    predicate = (c => c.CompanyId == ID && (FilterID == "0" || c.CategoryId.ToString() == FilterID) &&
                    (DisplayDate && c.Date.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayAmount && c.Amount.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                    )));
                }
                if (IgnorePaging)
                {
                    predicate = (c => c.CompanyId == ID && c.PaymentStatusId != 61);
                }
                IQueryable<emr_expense> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByDate = PFilter.OrderBy.IndexOf("Date") > -1;
                OrderByCategory = PFilter.OrderBy.IndexOf("CategoryId") > -1;
                OrderByAmount = PFilter.OrderBy.IndexOf("Amount") > -1;

                Expression<Func<emr_expense, string>> orderingFunction = (c =>
                                                              OrderByDate ? c.Date.ToString() :
                                                              OrderByCategory ? c.CategoryId.ToString() :
                                                              OrderByAmount ? c.Amount.ToString() : ""
                                                              );

                if (PFilter.OrderBy == "ID")
                {
                    filteredData = filteredData.OrderByDescending(d => (d.ID)).AsQueryable();
                    objResult.TotalRecord = filteredData.Count();
                }
                else
                {
                    if (PFilter.IsOrderAsc)
                        filteredData = filteredData.OrderBy(orderingFunction);
                    else
                        filteredData = filteredData.OrderByDescending(orderingFunction);
                    objResult.TotalRecord = filteredData.Count();
                }

                if (IgnorePaging)
                {

                    objResult.DataList = filteredData
                        .Select(s => new
                        {
                            s.ID,
                            Date = s.Date,
                            Category = s.sys_drop_down_value.Value,
                            Amount = s.Amount,
                            CreatedDate = s.CreatedDate
                        }).OrderByDescending(A => A.ID).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           Date = s.Date,
                           Category = s.sys_drop_down_value.Value,
                           Amount = s.Amount,
                           CreatedDate = s.CreatedDate
                       }).OrderByDescending(A => A.ID).ToList<object>();
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
