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
    public static class emr_prescription_diagnosRepository
    {
        public static PaginationResult Pagination(this IRepository<emr_prescription_diagnos> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_prescription_diagnos, bool>> predicate = (e => e.CompanyID == ID);
                bool DisplayName;
                bool OrderByName;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Diagnos") > -1;
                    predicate = (c =>
                    (DisplayName && c.Diagnos.ToLower().Contains(PFilter.SearchText.ToLower()
                    )));
                }

                IQueryable<emr_prescription_diagnos> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "Id";
                OrderByName = PFilter.OrderBy.IndexOf("Diagnos") > -1;

                Expression<Func<emr_prescription_diagnos, string>> orderingFunction = (c =>
                                                              OrderByName ? c.Diagnos : ""
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
                            ID = s.ID,
                            Diagnos = s.Diagnos,
                        }).OrderByDescending(a => a.ID).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           ID = s.ID,
                           Diagnos = s.Diagnos,
                       }).OrderByDescending(a => a.ID).ToList<object>();
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
