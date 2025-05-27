using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Repository.Repositories.Items
{
    public static class pur_Invoice_dtRepository
    {
        public static PaginationResult Pagination(this IRepository<pur_invoice_dt> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pur_invoice_dt, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayName, DisplayDescription, DisplayRate;
                bool OrderByName, OrderByDescription, OrderByRate;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }

    }
}
