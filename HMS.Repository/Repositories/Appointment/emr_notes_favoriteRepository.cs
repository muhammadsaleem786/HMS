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
    public static class emr_notes_favoriteRepository
    {
        public static PaginationResult Pagination(this IRepository<emr_notes_favorite> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_notes_favorite, bool>> predicate = (e => e.CompanyId == ID);

                bool DisplayName;
                bool OrderByName;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Complaint") > -1;
                    predicate = (c =>
                    (DisplayName && c.ReferenceType.ToLower().Contains(PFilter.SearchText.ToLower()
                    )));
                }

                IQueryable<emr_notes_favorite> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "Id";

                OrderByName = PFilter.OrderBy.IndexOf("Complaint") > -1;

                Expression<Func<emr_notes_favorite, string>> orderingFunction = (c =>
                                                              OrderByName ? c.ReferenceType : ""
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
                            Complaint = s.ReferenceType,
                        }).OrderByDescending(A => A.ID).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                          ID= s.ID,
                           Complaint = s.ReferenceType,
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
