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

namespace HMS.Repository.Repositories.Appointment
{
    public static class emr_documentRepository
    {
        public static PaginationResult Pagination(this IRepository<emr_document> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_document, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayDate, DisplayRemarks;
                bool OrderByDate, OrderByRemarks;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayDate = PFilter.VisibleColumnInfoList.IndexOf("Date") > -1;
                    DisplayRemarks = PFilter.VisibleColumnInfoList.IndexOf("Remarks") > -1;
                    predicate = (c => c.CompanyID == CompanyID &&
                    (DisplayDate && c.Date.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayRemarks && c.Remarks.ToString().Contains(PFilter.SearchText.ToLower())

                    )));
                }

                IQueryable<emr_document> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByDate = PFilter.OrderBy.IndexOf("Date") > -1;
                OrderByRemarks = PFilter.OrderBy.IndexOf("Remarks") > -1;


                Expression<Func<emr_document, string>> orderingFunction = (c =>
                                                              OrderByDate ? c.Date.ToString() :
                                                              OrderByRemarks ? c.Remarks.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    objResult.DataList = filteredData.Include(a=>a.sys_drop_down_value)
                        .Select(s => new
                        {
                            s.ID,
                            Date = s.Date,
                            Remarks = s.Remarks,
                            DocumentUpload = s.DocumentUpload,
                            Tag = s.sys_drop_down_value.Value,
                            Uploaddate=s.CreatedDate,
                           
                        }).OrderByDescending(A => A.ID).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Include(a => a.sys_drop_down_value).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           Date = s.Date,
                           Remarks = s.Remarks,
                           DocumentUpload = s.DocumentUpload,
                           Tag = s.sys_drop_down_value.Value,
                           Uploaddate = s.CreatedDate,
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
