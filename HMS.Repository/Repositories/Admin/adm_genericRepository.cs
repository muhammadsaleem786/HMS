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
    public static class adm_genericRepository
    {
        public static PaginationResult Pagination(this IRepository<adm_generic> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_generic, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayName, DisplayGenericName;
                bool OrderByName, OrderByGenericName;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("CommonUse") > -1;
                    DisplayGenericName = PFilter.VisibleColumnInfoList.IndexOf("GenericName") > -1;

                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayName && c.GenericName.ToLower().Contains(PFilter.SearchText.ToLower())
                     || (DisplayGenericName && c.GenericName.ToLower().Contains(PFilter.SearchText.ToLower()))));
                }

                IQueryable<adm_generic> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("CommonUse") > -1;
                OrderByGenericName = PFilter.OrderBy.IndexOf("GenericName") > -1;

                Expression<Func<adm_generic, string>> orderingFunction = (c =>
                                                              OrderByName ? c.CommonUse :
                                                                OrderByGenericName ? c.GenericName : "0"
                                                             );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    objResult.DataList = filteredData
                      .Include(i => i.sys_drop_down_value).Include(i => i.sys_drop_down_value1)
                      .Select(s => new
                      {
                          s.ID,
                          s.GenericName,
                          s.CommonUse,
                          Specialty = s.sys_drop_down_value.Value,
                          Category = s.sys_drop_down_value1.Value,
                          IsActive=s.IsActive==true?"Yes":"No",
                      }).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData
                    .Include(i => i.sys_drop_down_value).Include(i => i.sys_drop_down_value1)
                    .Select(s => new
                    {
                        s.ID,
                        s.GenericName,
                        s.CommonUse,
                        Specialty = s.sys_drop_down_value.Value,
                        Category = s.sys_drop_down_value1.Value,
                        IsActive = s.IsActive == true ? "Yes" : "No",
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
