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

namespace HMS.Repository.Repositories.Admin
{
    public static class sys_drop_down_valueRepository
    {
        public static PaginationResult Pagination(this IRepository<sys_drop_down_value> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<sys_drop_down_value, bool>> predicate = (e => e.CompanyID == ID || e.CompanyID==null);

                bool DisplayName, DisplaySystemGenerated;
                bool OrderByName, OrderBySystemGenerated;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Value") > -1;
                    DisplaySystemGenerated = PFilter.VisibleColumnInfoList.IndexOf("SystemGenerated") > -1;

                    predicate = (c => c.CompanyID == ID &&
                    (DisplayName && c.Value.ToLower().Contains(PFilter.SearchText.ToLower())
                     || (DisplaySystemGenerated && c.SystemGenerated.ToString().Contains(PFilter.SearchText.ToLower())
                    )));
                }

                IQueryable<sys_drop_down_value> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("Value") > -1;
                OrderBySystemGenerated = PFilter.OrderBy.IndexOf("SystemGenerated") > -1;

                Expression<Func<sys_drop_down_value, string>> orderingFunction = (c =>
                                                              OrderByName ? c.Value :
                                                                OrderBySystemGenerated ? c.SystemGenerated.ToString() : "0"
                                                             );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    //objResult.DataList = (from c in filteredData select c).ToList<object>();
                    objResult.DataList = filteredData
                       .Select(s => new
                       {
                           s.ID,
                           s.Value,
                           SystemGenerated = s.SystemGenerated == true ? "Yes" : "No",

                       }).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           s.Value,
                           SystemGenerated = s.SystemGenerated == true ? "Yes" : "No",

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
