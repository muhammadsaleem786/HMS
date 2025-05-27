using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Repository.Repositories.Appointment
{
    public static class emr_incomeRepository
    {
        public static PaginationResult Pagination(this IRepository<emr_income> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_income, bool>> predicate = (e => e.CompanyId == ID);

                bool DisplayDate, DisplayCategory, DisplayAmount, DisplayReceivedAmount;
                bool OrderByDate, OrderByCategory, OrderByAmount, OrderByReceivedAmount;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayDate = PFilter.VisibleColumnInfoList.IndexOf("Date") > -1;
                    DisplayCategory = PFilter.VisibleColumnInfoList.IndexOf("Category") > -1;
                    DisplayAmount = PFilter.VisibleColumnInfoList.IndexOf("DueAmount") > -1;
                    DisplayReceivedAmount = PFilter.VisibleColumnInfoList.IndexOf("ReceivedAmount") > -1;
                    predicate = (c =>
                    (DisplayDate && c.Date.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayAmount && c.DueAmount.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayReceivedAmount && c.ReceivedAmount.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayCategory && c.CategoryId.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                    )))));
                }
                if (IgnorePaging)
                {
                    predicate = (c => c.DueAmount != 0);
                }

                IQueryable<emr_income> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByDate = PFilter.OrderBy.IndexOf("Date") > -1;
                OrderByCategory = PFilter.OrderBy.IndexOf("Category") > -1;
                OrderByAmount = PFilter.OrderBy.IndexOf("DueAmount") > -1;
                OrderByReceivedAmount = PFilter.OrderBy.IndexOf("ReceivedAmount") > -1;
                Expression<Func<emr_income, string>> orderingFunction = (c =>
                                                              OrderByDate ? c.Date.ToString() :
                                                              OrderByCategory ? c.CategoryId.ToString() :
                                                              OrderByAmount ? c.DueAmount.ToString() :
                                                              OrderByReceivedAmount ? c.ReceivedAmount.ToString() : ""
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
                            DueAmount = s.DueAmount,
                            ReceivedAmount = s.ReceivedAmount,
                            CreatedDate = s.CreatedDate
                        }).OrderByDescending(A => A.ID).ToList<object>();
                }
                else
                {
                    var data = filteredData
                        .Select(s => new
                        {
                            s.ID,
                            Date = s.Date,
                            Category = s.sys_drop_down_value.Value,
                            DueAmount = s.DueAmount,
                            ReceivedAmount = s.ReceivedAmount,
                            CreatedDate = s.CreatedDate
                        }).OrderByDescending(A => A.CreatedDate).ToList();
                    objResult.DataList = data.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           Date = s.Date,
                           Category = s.Category,
                           DueAmount = s.DueAmount,
                           ReceivedAmount = s.ReceivedAmount,
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
        public static PaginationResult PaginationWithParm(this IRepository<emr_income> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_income, bool>> predicate = (e => e.CompanyId == ID && (FilterID == "0" || e.CategoryId.ToString() == FilterID));

                bool DisplayDate, DisplayCategory, DisplayAmount, DisplayReceivedAmount;
                bool OrderByDate, OrderByCategory, OrderByAmount, OrderByReceivedAmount;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayDate = PFilter.VisibleColumnInfoList.IndexOf("Date") > -1;
                    DisplayCategory = PFilter.VisibleColumnInfoList.IndexOf("Category") > -1;
                    DisplayAmount = PFilter.VisibleColumnInfoList.IndexOf("DueAmount") > -1;
                    DisplayReceivedAmount = PFilter.VisibleColumnInfoList.IndexOf("ReceivedAmount") > -1;
                    predicate = (c => c.CompanyId == ID && (FilterID == "0" || c.CategoryId.ToString() == FilterID) &&
                    (DisplayDate && c.Date.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayAmount && c.DueAmount.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayReceivedAmount && c.ReceivedAmount.ToString().ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayCategory && c.CategoryId.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                    )))));
                }
                if (IgnorePaging)
                {
                    predicate = (c => c.CompanyId==ID && c.DueAmount != 0);
                }
                IQueryable<emr_income> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByDate = PFilter.OrderBy.IndexOf("Date") > -1;
                OrderByCategory = PFilter.OrderBy.IndexOf("Category") > -1;
                OrderByAmount = PFilter.OrderBy.IndexOf("DueAmount") > -1;
                OrderByReceivedAmount = PFilter.OrderBy.IndexOf("ReceivedAmount") > -1;
                Expression<Func<emr_income, string>> orderingFunction = (c =>
                                                              OrderByDate ? c.Date.ToString() :
                                                              OrderByCategory ? c.CategoryId.ToString() :
                                                              OrderByAmount ? c.DueAmount.ToString() :
                                                              OrderByReceivedAmount ? c.ReceivedAmount.ToString() : ""
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
                            DueAmount = s.DueAmount,
                            ReceivedAmount = s.ReceivedAmount,
                            CreatedDate = s.CreatedDate
                        }).OrderByDescending(A => A.CreatedDate).ToList<object>();
                }
                else
                {
                    var data = filteredData
                        .Select(s => new
                        {
                            s.ID,
                            Date = s.Date,
                            Category = s.sys_drop_down_value.Value,
                            DueAmount = s.DueAmount,
                            ReceivedAmount = s.ReceivedAmount,
                            CreatedDate = s.CreatedDate
                        }).OrderByDescending(A => A.CreatedDate).ToList();
                    objResult.DataList = data.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           Date = s.Date,
                           Category = s.Category,
                           DueAmount = s.DueAmount,
                           ReceivedAmount = s.ReceivedAmount,
                           CreatedDate = s.CreatedDate
                       }).OrderByDescending(A => A.CreatedDate).ToList<object>();
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
