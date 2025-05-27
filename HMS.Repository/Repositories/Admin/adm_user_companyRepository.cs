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

    public static class adm_user_companyRepository
    {
        public static PaginationResult Pagination(this IRepository<adm_user_company> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_user_company, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayUserID, DisplayRoleID, DisplayEmail, DisplayLastLogin;
                bool OrderByUserID, OrderByRoleID, OrderByEmail, OrderByLastLogin;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayUserID = PFilter.VisibleColumnInfoList.IndexOf("UserName") > -1;
                    DisplayRoleID = PFilter.VisibleColumnInfoList.IndexOf("RoleName") > -1;
                    DisplayEmail = PFilter.VisibleColumnInfoList.IndexOf("Email") > -1;
                    DisplayLastLogin = PFilter.VisibleColumnInfoList.IndexOf("LastLogin") > -1;
                    predicate = (c => c.CompanyID == CompanyID &&
                    (DisplayUserID && c.UserID.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayRoleID && c.RoleID.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    
                    (DisplayLastLogin && c.adm_user_mf.LastSignIn.ToString().Contains(PFilter.SearchText)

                    ))));
                }

                IQueryable<adm_user_company> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByUserID = PFilter.OrderBy.IndexOf("UserName") > -1;
                OrderByRoleID = PFilter.OrderBy.IndexOf("RoleName") > -1;
                OrderByEmail = PFilter.OrderBy.IndexOf("Email") > -1;
                OrderByLastLogin = PFilter.OrderBy.IndexOf("LastLogin") > -1;

                Expression<Func<adm_user_company, string>> orderingFunction = (c =>
                                                              OrderByUserID ? c.UserID.ToString() :
                                                                OrderByRoleID ? c.RoleID.ToString() :
                                                                  
                                                              OrderByLastLogin ? c.adm_user_mf.LastSignIn.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    //objResult.DataList = (from c in filteredData select c).ToList<object>();

                    objResult.DataList = filteredData
                       
                        .Select(s => new
                        {
                            s.ID,
                            RoleName = s.adm_role_mf.RoleName,
                            LastLogin = s.adm_user_mf.LastSignIn

                        }).ToList<object>();
                }
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    //objResult.DataList = (from c in PageResult select c).ToList<object>();
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           RoleName = s.adm_role_mf.RoleName,
                           LastLogin = s.adm_user_mf.LastSignIn

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
