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

namespace HMS.Repository.Repositories.Admin
{
    public static class sys_notification_alertRepository
    {
        public static PaginationResult Pagination(this IRepository<sys_notification_alert> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<sys_notification_alert, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayEmailTo;
                bool OrderByEmailTo;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayEmailTo = PFilter.VisibleColumnInfoList.IndexOf("EmailTo") > -1;
                    //DisplayUserName = PFilter.VisibleColumnInfoList.IndexOf("UserName") > -1;
                    //DisplayPhoneNo = PFilter.VisibleColumnInfoList.IndexOf("PhoneNo") > -1;
                    //DisplayEmail = PFilter.VisibleColumnInfoList.IndexOf("Email") > -1;
                    predicate = (c =>
                    (DisplayEmailTo && c.EmailTo.ToLower().Contains(PFilter.SearchText.ToLower())

                    //
                    ));
                }

                IQueryable<sys_notification_alert> filteredData = repository.Queryable().Where(predicate);

                //if (string.IsNullOrEmpty(PFilter.OrderBy))
                //    PFilter.OrderBy = "TaxCode";

                OrderByEmailTo = PFilter.OrderBy.IndexOf("EmailTo") > -1;
                OrderByEmailTo = PFilter.OrderBy.IndexOf("EmailTo") > -1;

                Expression<Func<sys_notification_alert, string>> orderingFunction = (c =>
                                                              OrderByEmailTo ? c.EmailTo :""
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
