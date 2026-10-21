using HMS.Entities.Models;
using HMS.Repository.Common;
using HMS.Entities.CustomModel;
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
    public static class adm_user_sessionRepository
    {
        public static PaginationResult Pagination(this IRepository<adm_user_session> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_user_session, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayShiftStartTime, DisplayShiftEndTime;
                bool OrderByShiftStartTime, OrderByShiftEndTime;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayShiftStartTime = PFilter.VisibleColumnInfoList.IndexOf("ShiftStartTime") > -1;
                    DisplayShiftEndTime = PFilter.VisibleColumnInfoList.IndexOf("ShiftEndTime") > -1;
                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayShiftStartTime && c.ShiftStartTime.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayShiftEndTime && c.ShiftEndTime.ToString().Contains(PFilter.SearchText.ToLower())
                    )));
                }

                IQueryable<adm_user_session> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByShiftStartTime = PFilter.OrderBy.IndexOf("ShiftStartTime") > -1;
                OrderByShiftEndTime = PFilter.OrderBy.IndexOf("ShiftEndTime") > -1;


                Expression<Func<adm_user_session, string>> orderingFunction = (c =>
                                                              OrderByShiftStartTime ? c.ShiftStartTime.ToString() :
                                                                OrderByShiftEndTime ? c.ShiftEndTime.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                filteredData = filteredData.OrderByDescending(d => (d.ID)).AsQueryable();
                objResult.TotalRecord = filteredData.Count();

                var query = IgnorePaging
    ? filteredData
    : filteredData
        .Skip(PFilter.SkipRecord)
        .Take(PFilter.TakeRecord);
                var sessions = query
   .Select(s => new
   {
       s.ID,
       s.ShiftStartTime,
       s.ShiftEndTime,
       s.StartingCash,
       s.TransactionsCount,
       s.TotalCollection,
       s.ClosingComments,
       s.OpeningComments,
       s.ClosingBalance,
       Name = s.adm_user_mf.Name,
       s.UserId
   })
   .ToList();
                objResult.DataList = sessions
    .Select(s => new
    {
        s.ID,
        ShiftStart = s.ShiftStartTime.AddHours(5),
        ShiftEnd = s.ShiftEndTime.HasValue
            ? s.ShiftEndTime.Value.AddHours(5).ToString()
            : null,
        s.StartingCash,
        s.TransactionsCount,
        s.TotalCollection,
        s.ClosingComments,
        s.OpeningComments,
        s.ClosingBalance,
        s.Name,
        s.UserId
    })
    .ToList<object>();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }

        public static PaginationResult PaginationWithParm(this IRepository<adm_user_session> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_user_session, bool>> predicate;
                if (FilterID == "O")
                {
                    // Open Sessions
                    predicate = e => e.CompanyId == ID && e.ShiftEndTime == null;
                }
                else if (FilterID == "C")
                {
                    // Closed Sessions
                    predicate = e => e.CompanyId == ID && e.ShiftEndTime != null;
                }
                else
                {
                    // All Sessions
                    predicate = e => e.CompanyId == ID;
                }

                bool DisplayShiftStartTime, DisplayShiftEndTime;
                bool OrderByShiftStartTime, OrderByShiftEndTime;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayShiftStartTime = PFilter.VisibleColumnInfoList.IndexOf("ShiftStartTime") > -1;
                    DisplayShiftEndTime = PFilter.VisibleColumnInfoList.IndexOf("ShiftEndTime") > -1;
                    if (FilterID == "O")
                    {
                        predicate = c =>
                            c.CompanyId == ID &&
                            c.ShiftEndTime == null &&
                            (
                                (DisplayShiftStartTime &&
                                 c.ShiftStartTime.ToString().Contains(PFilter.SearchText.ToLower()))
                                ||
                                (DisplayShiftEndTime &&
                                 c.ShiftEndTime.ToString().Contains(PFilter.SearchText.ToLower()))
                            );
                    }
                    else if (FilterID == "C")
                    {
                        predicate = c =>
                            c.CompanyId == ID &&
                            c.ShiftEndTime != null &&
                            (
                                (DisplayShiftStartTime &&
                                 c.ShiftStartTime.ToString().Contains(PFilter.SearchText.ToLower()))
                                ||
                                (DisplayShiftEndTime &&
                                 c.ShiftEndTime.ToString().Contains(PFilter.SearchText.ToLower()))
                            );
                    }
                    else
                    {
                        predicate = (c => c.CompanyId == ID &&
                    (DisplayShiftStartTime && c.ShiftStartTime.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayShiftEndTime && c.ShiftEndTime.ToString().Contains(PFilter.SearchText.ToLower())
                    )));
                    }
                }
                IQueryable<adm_user_session> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByShiftStartTime = PFilter.OrderBy.IndexOf("ShiftStartTime") > -1;
                OrderByShiftEndTime = PFilter.OrderBy.IndexOf("ShiftEndTime") > -1;


                Expression<Func<adm_user_session, string>> orderingFunction = (c =>
                                                              OrderByShiftStartTime ? c.ShiftStartTime.ToString() :
                                                                OrderByShiftEndTime ? c.ShiftEndTime.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                filteredData = filteredData.OrderByDescending(d => (d.ID)).AsQueryable();
                objResult.TotalRecord = filteredData.Count();

                var query = IgnorePaging
    ? filteredData
    : filteredData
        .Skip(PFilter.SkipRecord)
        .Take(PFilter.TakeRecord);
                var sessions = query
    .Select(s => new
    {
        s.ID,
        s.ShiftStartTime,
        s.ShiftEndTime,
        s.StartingCash,
        s.TransactionsCount,
        s.TotalCollection,
        s.ClosingComments,
        s.OpeningComments,
        s.ClosingBalance,
        Name = s.adm_user_mf.Name,
        s.UserId
    })
    .ToList();


                objResult.DataList = sessions
      .Select(s => new
      {
          s.ID,
          ShiftStart = s.ShiftStartTime.AddHours(5),
          ShiftEnd = s.ShiftEndTime.HasValue
              ? s.ShiftEndTime.Value.AddHours(5).ToString()
              : null,
          s.StartingCash,
          s.TransactionsCount,
          s.TotalCollection,
          s.ClosingComments,
          s.OpeningComments,
          s.ClosingBalance,
          s.Name,
          s.UserId
      })
      .ToList<object>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }

    }

}
