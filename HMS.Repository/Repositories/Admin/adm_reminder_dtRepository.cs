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

namespace HMS.Repository.Repositories.Admin
{
    public static class adm_reminder_dtRepository
    {
        public static PaginationResult Pagination(this IRepository<adm_reminder_dt> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_reminder_dt, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayName, DisplayEmployees;
                bool OrderByName, OrderByEmployees;



                IQueryable<adm_reminder_dt> filteredData = repository.Queryable().Where(predicate);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
    }
}
