using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HMS.Repository.Repositories.Items
{
    public static class adm_item_logRepository
    {
        public static PaginationResult Pagination(this IRepository<adm_item_log> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
    }
}
