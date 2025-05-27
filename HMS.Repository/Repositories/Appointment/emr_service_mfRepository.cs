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
    public static class emr_service_mfRepository
    {
        public static PaginationResult Pagination(this IRepository<emr_service_mf> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_service_mf, bool>> predicate = (e => e.CompanyId == ID);

                bool DisplayName;
                bool OrderByName;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("ServiceName") > -1;
                    predicate = (c =>
                    (DisplayName && c.ServiceName.ToLower().Contains(PFilter.SearchText.ToLower()
                    )));
                }

                IQueryable<emr_service_mf> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("ServiceName") > -1;

                Expression<Func<emr_service_mf, string>> orderingFunction = (c =>
                                                              OrderByName ? c.ServiceName : ""
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
                            ID=s.ID,
                            ServiceName = s.ServiceName,
                            Price = s.Price,
                            RefCode = s.RefCode
                        }).OrderByDescending(A => A.ID).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           ID = s.ID,
                           ServiceName = s.ServiceName,
                           Price = s.Price,
                           RefCode = s.RefCode
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
