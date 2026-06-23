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
    public static class lab_parameterRepository
    {
        public static PaginationResult Pagination(this IRepository<lab_parameter> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<lab_parameter, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayName, DisplayGenericName;
                bool OrderByName, OrderByGenericName;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("ParameterName") > -1;
                    DisplayGenericName = PFilter.VisibleColumnInfoList.IndexOf("Description") > -1;

                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayName && c.ParameterName.ToLower().Contains(PFilter.SearchText.ToLower())
                     || (DisplayGenericName && c.Description.ToLower().Contains(PFilter.SearchText.ToLower()))));
                }

                IQueryable<lab_parameter> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("ParameterName") > -1;
                OrderByGenericName = PFilter.OrderBy.IndexOf("Description") > -1;

                Expression<Func<lab_parameter, string>> orderingFunction = (c =>
                                                              OrderByName ? c.ParameterName :
                                                                OrderByGenericName ? c.Description: "0"
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
                          s.ParameterName,
                          s.Description ,
                          s.NormalMinimum,
                          s.NormalMaximum,
                          s.CriticalLow,
                          s.CriticalHigh,
                          s.ReferenceRange,
                          s.CatalogId,
                          Unit = s.sys_drop_down_value.Value,
                      }).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData
                    .Include(i => i.sys_drop_down_value)
                    .Select(s => new
                    {
                        s.ID,
                        s.ParameterName,
                        s.Description,
                        s.NormalMinimum,
                        s.NormalMaximum,
                        s.CriticalLow,
                        s.CriticalHigh,
                        s.ReferenceRange,
                        s.CatalogId,
                        Unit = s.sys_drop_down_value.Value,
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
