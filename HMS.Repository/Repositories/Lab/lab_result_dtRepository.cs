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

namespace HMS.Repository.Repositories.Lab
{
    public static class lab_result_dtRepository
    {
        public static PaginationResult Pagination(this IRepository<lab_result_dt> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<lab_result_dt, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayName, DisplayGenericName;
                bool OrderByName, OrderByGenericName;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Result") > -1;
                    DisplayGenericName = PFilter.VisibleColumnInfoList.IndexOf("Status") > -1;

                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayName && c.Result.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                     || (DisplayGenericName && c.Status.ToLower().Contains(PFilter.SearchText.ToLower()))));
                }

                IQueryable<lab_result_dt> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("Result") > -1;
                OrderByGenericName = PFilter.OrderBy.IndexOf("Status") > -1;

                Expression<Func<lab_result_dt, string>> orderingFunction = (c =>
                                                              OrderByName ? c.Result.ToString() :
                                                                OrderByGenericName ? c.Status: "0"
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
                          s.Result,
                          s.Status,
                          s.Comment,
                      }).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData
                    .Select(s => new
                    {
                        s.ID,
                        s.Result,
                        s.Status,
                        s.Comment,
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
