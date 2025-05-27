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
    public static class adm_reminder_mfRepository
    {
        public static PaginationResult Pagination(this IRepository<adm_reminder_mf> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_reminder_mf, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayName, DisplayEmployees;
                bool OrderByName, OrderByEmployees;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Name") > -1;
                    predicate = (c => c.CompanyId == CompanyID &&
 (DisplayName && c.Name.ToLower().Contains(PFilter.SearchText.ToLower())
 ));
                }

                IQueryable<adm_reminder_mf> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("Name") > -1;


                Expression<Func<adm_reminder_mf, string>> orderingFunction = (c =>
                                                              OrderByName ? c.Name : ""
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
                           s.Name,
                       }).ToList<object>();
                }
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           s.Name,
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
