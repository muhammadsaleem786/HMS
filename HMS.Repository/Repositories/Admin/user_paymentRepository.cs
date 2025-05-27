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
    public static class user_paymentRepository
    {
        public static PaginationResult Pagination(this IRepository<user_payment> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<user_payment, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayUserID, DisplayRoleID, DisplayEmail, DisplayLastLogin;
                bool OrderByUserID, OrderByRoleID, OrderByEmail, OrderByLastLogin;


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
    }

}
