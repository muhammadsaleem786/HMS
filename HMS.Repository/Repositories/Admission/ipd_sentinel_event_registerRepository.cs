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

namespace HMS.Repository.Repositories.Admission
{
    public static class ipd_sentinel_event_registerRepository
    {
        public static PaginationResult Pagination(this IRepository<ipd_sentinel_event_register> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<ipd_sentinel_event_register, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayPatientName, DisplayDOB, DisplayEmail, DisplayMobile, DisplayMRNO, DisplayCNIC;
                bool OrderByPatientName, OrderByDOB, OrderByEmail, OrderByMobile, OrderByMRNO, OrderByCNIC;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("EventDescription") > -1;
                    DisplayDOB = PFilter.VisibleColumnInfoList.IndexOf("EventDate") > -1;
                    predicate = (c => c.CompanyID == CompanyID &&
                    (DisplayPatientName && c.EventDescription.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayDOB && c.EventDate.ToString().Contains(PFilter.SearchText.ToLower()))));
                }

                IQueryable<ipd_sentinel_event_register> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByPatientName = PFilter.OrderBy.IndexOf("EventDescription") > -1;
                OrderByDOB = PFilter.OrderBy.IndexOf("EventDate") > -1;


                Expression<Func<ipd_sentinel_event_register, string>> orderingFunction = (c =>
                                                              OrderByPatientName ? c.EventDescription.ToString() :
                                                                OrderByDOB ? c.EventDate.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                filteredData = filteredData.OrderByDescending(d => (d.ID)).AsQueryable();
                objResult.TotalRecord = filteredData.Count();
                if (IgnorePaging)
                {

                    objResult.DataList = filteredData
                        .Select(s => new
                        {
                            s.ID,
                            s.EventDescription,
                            s.EventDate,
                            s.EventType,
                            s.ImmediateActionTaken,
                            s.OutcomeDetails,
                            s.ReportedBy,
                            s.ReviewedByClinicalManager
                        }).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           s.EventDescription,
                           s.EventDate,
                           s.EventType,
                           s.ImmediateActionTaken,
                           s.OutcomeDetails,
                           s.ReportedBy,
                           s.ReviewedByClinicalManager
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
