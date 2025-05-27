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
    public static class adm_user_mfRepository
    {
        public static PaginationResult Pagination(this IRepository<adm_user_mf> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_user_mf, bool>> predicate = (e => e.adm_user_company.Any(x => x.CompanyID == CompanyID));

                bool DisplayUserID, DisplayRoleID, DisplayEmail, DisplayLastLogin;
                bool OrderByUserID, OrderByRoleID, OrderByEmail, OrderByLastLogin;
                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayUserID = PFilter.VisibleColumnInfoList.IndexOf("UserName") > -1;
                    DisplayRoleID = PFilter.VisibleColumnInfoList.IndexOf("RoleName") > -1;
                    DisplayEmail = PFilter.VisibleColumnInfoList.IndexOf("Email") > -1;
                    DisplayLastLogin = PFilter.VisibleColumnInfoList.IndexOf("LastLogin") > -1;
                    predicate = (c => c.adm_user_company.Any(x => x.CompanyID == CompanyID) &&
                    (DisplayUserID && c.Name.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayRoleID && c.adm_user_company.Any(x => x.adm_role_mf.RoleName.ToLower().Contains(PFilter.SearchText.ToLower())) ||
                    (DisplayEmail && c.Email.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayLastLogin && c.LastSignIn.ToString().Contains(PFilter.SearchText)

                    )))));
                }

                IQueryable<adm_user_mf> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByUserID = PFilter.OrderBy.IndexOf("UserName") > -1;
                OrderByRoleID = PFilter.OrderBy.IndexOf("RoleName") > -1;
                OrderByEmail = PFilter.OrderBy.IndexOf("Email") > -1;
                OrderByLastLogin = PFilter.OrderBy.IndexOf("LastLogin") > -1;

                Expression<Func<adm_user_mf, string>> orderingFunction = (c =>
                                                              OrderByUserID ? c.Name.ToString() :
                                                                OrderByRoleID ? c.adm_user_company.Select(x => x.adm_role_mf).FirstOrDefault().RoleName :
                                                                  OrderByEmail ? c.Email :
                                                              OrderByLastLogin ? c.LastSignIn.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderByDescending(orderingFunction);
                else
                    filteredData = filteredData.OrderBy(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    objResult.DataList = filteredData
                        .Include(i => i.adm_user_company)
                        .Select(s => new
                        {
                            s.ID,
                            UserName = s.Name,
                            RoleName = s.adm_user_company.Select(x => x.adm_role_mf).FirstOrDefault().RoleName,
                            Email = s.Email,
                            PhoneNo = s.PhoneNo,
                            LastLogin = s.LastSignIn,
                            Date = s.ActivationTokenDate,
                            ClinicName = s.adm_user_company.Select(x => x.adm_company).FirstOrDefault().CompanyName,
                        }).ToList<object>();
                }
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Include(i => i.adm_user_company)
                       .Select(s => new
                       {
                           s.ID,
                           UserName = s.Name,
                           RoleName = s.adm_user_company.Select(x => x.adm_role_mf.RoleName).FirstOrDefault(),
                           Email = s.Email,
                           PhoneNo = s.PhoneNo,
                           LastLogin = s.LastSignIn,
                           Date = s.ActivationTokenDate,
                           ClinicName = s.adm_user_company.Select(x => x.adm_company).FirstOrDefault().CompanyName,
                       }).ToList<object>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
        public static PaginationResult PaymentPagination(this IRepository<adm_user_mf> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_user_mf, bool>> predicate = (e => e.adm_user_company.Any(x => x.AdminID == x.UserID));

                bool DisplayUserID, DisplayRoleID, DisplayEmail, DisplayLastLogin;
                bool OrderByUserID, OrderByRoleID, OrderByEmail, OrderByLastLogin;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayUserID = PFilter.VisibleColumnInfoList.IndexOf("UserName") > -1;
                    DisplayRoleID = PFilter.VisibleColumnInfoList.IndexOf("RoleName") > -1;
                    DisplayEmail = PFilter.VisibleColumnInfoList.IndexOf("Email") > -1;
                    DisplayLastLogin = PFilter.VisibleColumnInfoList.IndexOf("LastLogin") > -1;
                    predicate = (c => c.adm_user_company.Any(x => x.CompanyID == CompanyID) &&
                    (DisplayUserID && c.Name.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayRoleID && c.adm_user_company.Any(x => x.adm_role_mf.RoleName.ToLower().Contains(PFilter.SearchText.ToLower())) ||
                    (DisplayEmail && c.Email.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayLastLogin && c.LastSignIn.ToString().Contains(PFilter.SearchText)

                    )))));
                }

                IQueryable<adm_user_mf> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByUserID = PFilter.OrderBy.IndexOf("UserName") > -1;
                OrderByRoleID = PFilter.OrderBy.IndexOf("RoleName") > -1;
                OrderByEmail = PFilter.OrderBy.IndexOf("Email") > -1;
                OrderByLastLogin = PFilter.OrderBy.IndexOf("LastLogin") > -1;

                Expression<Func<adm_user_mf, string>> orderingFunction = (c =>
                                                              OrderByUserID ? c.Name.ToString() :
                                                                OrderByRoleID ? c.adm_user_company.Select(x => x.adm_role_mf).FirstOrDefault().RoleName :
                                                                  OrderByEmail ? c.Email :
                                                              OrderByLastLogin ? c.LastSignIn.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderByDescending(orderingFunction);
                else
                    filteredData = filteredData.OrderBy(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    objResult.DataList = filteredData
                        .Include(i => i.adm_user_company)
                        .Select(s => new
                        {
                            s.ID,
                            UserName = s.Name,
                            RoleName = s.adm_user_company.Select(x => x.adm_role_mf).FirstOrDefault().RoleName,
                            Email = s.Email,
                            PhoneNo = s.PhoneNo,
                            LastLogin = s.LastSignIn,
                            Date = s.ActivationTokenDate,
                            ClinicName = s.adm_user_company.Select(x => x.adm_company).FirstOrDefault().CompanyName,
                        }).OrderByDescending(a => a.ID).ToList<object>();
                }
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Include(i => i.adm_user_company)
                       .Select(s => new
                       {
                           s.ID,
                           UserName = s.Name,
                           RoleName = s.adm_user_company.Select(x => x.adm_role_mf.RoleName).FirstOrDefault(),
                           Email = s.Email,
                           PhoneNo = s.PhoneNo,
                           LastLogin = s.LastSignIn,
                           Date = s.ActivationTokenDate,
                           ClinicName = s.adm_user_company.Select(x => x.adm_company).FirstOrDefault().CompanyName,
                       }).OrderByDescending(a => a.ID).ToList<object>();
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
