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
    public static class emr_prescription_treatment_templateRepository
    {
        public static PaginationResult Pagination(this IRepository<emr_prescription_treatment_template> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_prescription_treatment_template, bool>> predicate = (e => e.CompanyID == ID);

                bool DisplayName, DisplayPhoneNo;
                bool OrderByName, OrderByPhoneNo;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("TemplateName") > -1;
                    DisplayPhoneNo = PFilter.VisibleColumnInfoList.IndexOf("CreatedDate") > -1;
                    predicate = (c =>
                    (DisplayName && c.TemplateName.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayPhoneNo && c.CreatedDate.ToString().Contains(PFilter.SearchText.ToLower())
                    )));
                }

                IQueryable<emr_prescription_treatment_template> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("TemplateName") > -1;
                OrderByPhoneNo = PFilter.OrderBy.IndexOf("CreatedDate") > -1;

                Expression<Func<emr_prescription_treatment_template, string>> orderingFunction = (c =>
                                                              OrderByName ? c.TemplateName :
                                                              OrderByPhoneNo ? c.CreatedDate.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                    objResult.DataList = (from c in filteredData select c).ToList<object>();
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    objResult.DataList = (from c in PageResult select c).ToList<object>();
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
