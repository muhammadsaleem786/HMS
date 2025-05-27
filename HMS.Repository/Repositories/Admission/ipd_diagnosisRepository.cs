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
    public static class ipd_diagnosisRepository
    {
        public static PaginationResult Pagination(this IRepository<ipd_diagnosis> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<ipd_diagnosis, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayPatientName, DisplayDOB, DisplayEmail, DisplayMobile, DisplayMRNO, DisplayCNIC;
                bool OrderByPatientName, OrderByDOB, OrderByEmail, OrderByMobile, OrderByMRNO, OrderByCNIC;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("Description") > -1;
                    DisplayDOB = PFilter.VisibleColumnInfoList.IndexOf("Date") > -1;
                    predicate = (c => c.CompanyID == CompanyID &&
                    (DisplayPatientName && c.Description.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayDOB && c.Date.ToString().Contains(PFilter.SearchText.ToLower()))));
                }

                IQueryable<ipd_diagnosis> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByPatientName = PFilter.OrderBy.IndexOf("Description") > -1;
                OrderByDOB = PFilter.OrderBy.IndexOf("Date") > -1;


                Expression<Func<ipd_diagnosis, string>> orderingFunction = (c =>
                                                              OrderByPatientName ? c.Description.ToString() :
                                                                OrderByDOB ? c.Date.ToString()  : ""
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
                            Description = s.Description,
                            Date = s.Date,                           
                        }).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           Description = s.Description,
                           Date = s.Date,
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
