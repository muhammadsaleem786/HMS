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

namespace HMS.Repository.Repositories.Employee
{
    public static class sys_holidaysRepository
    {
        public static PaginationResult Pagination(this IRepository<sys_holidays> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<sys_holidays, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayHolidayName, DisplayFromDate, DisplayToDate;
                bool OrderByHolidayName, OrderByFromDate, OrderByToDate;

                IQueryable<sys_holidays> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                var list = new List<sys_holidaysModel>();
                if (IgnorePaging)
                {
                    list = filteredData.Select(s => new sys_holidaysModel
                    {
                        ID = s.ID,
                        HolidayName = s.HolidayName,
                        FromDate = s.FromDate,
                        ToDate = s.ToDate
                    }).ToList();
                }
                else
                {
                    list = filteredData.OrderBy(x => x.ID).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new sys_holidaysModel
                       {
                           ID = s.ID,
                           HolidayName = s.HolidayName,
                           FromDate = s.FromDate,
                           ToDate = s.ToDate
                       }).ToList();
                }

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayHolidayName = PFilter.VisibleColumnInfoList.IndexOf("HolidayName") > -1;
                    DisplayFromDate = PFilter.VisibleColumnInfoList.IndexOf("FromDate") > -1;
                    DisplayToDate = PFilter.VisibleColumnInfoList.IndexOf("ToDate") > -1;
                    list = list.Where(c =>
                     (DisplayHolidayName && c.HolidayName.ToLower().Contains(PFilter.SearchText.ToLower())
                     || (DisplayFromDate && c.FromDate.ToString("dd/MM/yyyy").Contains(PFilter.SearchText))
                      || (DisplayToDate && c.ToDate.ToString("dd/MM/yyyy").Contains(PFilter.SearchText))
                     )).ToList();
                }

                OrderByHolidayName = PFilter.OrderBy.IndexOf("HolidayName") > -1;
                OrderByFromDate = PFilter.OrderBy.IndexOf("FromDate") > -1;
                OrderByToDate = PFilter.OrderBy.IndexOf("ToDate") > -1;

                Expression<Func<sys_holidaysModel, string>> orderingFunction = (c =>
                                                              OrderByHolidayName ? c.HolidayName :
                                                              OrderByFromDate ? c.FromDate.ToString("dd/MM/yyyy") :
                                                              OrderByToDate ? c.ToDate.ToString("dd/MM/yyyy") : ""
                                                              );


                IQueryable<sys_holidaysModel> prList = list.AsQueryable();
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
