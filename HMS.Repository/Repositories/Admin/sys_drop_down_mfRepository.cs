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
    public static class sys_drop_down_mfRepository
    {
        public static PaginationResult Pagination(this IRepository<sys_drop_down_mf> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                //Expression<Func<sys_drop_down_mf, bool>> predicate = (e => e.CompanyID == ID);

                bool DisplayName, DisplaySystemGenerated;
                bool OrderByName, OrderBySystemGenerated;


             
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
    }
}
