using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Repository.Repositories.Admission
{
    public static class ipd_medication_logRepository
    {
        public static PaginationResult Pagination(this IRepository<ipd_medication_log> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<ipd_medication_log, bool>> predicate = (e => e.CompanyId == ID);
                bool DisplayPatientName, DisplayDOB, DisplayEmail, DisplayMobile, DisplayMRNO, DisplayCNIC;
                bool OrderByPatientName, OrderByDOB, OrderByEmail, OrderByMobile, OrderByMRNO, OrderByCNIC;
                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("Value") > -1;
                    predicate = (c => c.CompanyId == ID &&
                    (DisplayPatientName && c.Date.ToString().Contains(PFilter.SearchText.ToLower())));
                }
                IQueryable<ipd_medication_log> filteredData = repository.Queryable().Where(predicate);
                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByPatientName = PFilter.OrderBy.IndexOf("Value") > -1;
                Expression<Func<ipd_medication_log, string>> orderingFunction = (c =>
                                                              OrderByPatientName ? c.Date.ToString() : ""
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
                            MedicineName = s.adm_item.Name,
                            Time = s.Time,
                            Date = s.Date
                        }).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           MedicineName = s.adm_item.Name,
                           Time = s.Time,
                           Date = s.Date
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
