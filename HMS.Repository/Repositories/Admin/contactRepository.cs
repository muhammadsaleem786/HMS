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
    public static class contactRepository
    {
        public static PaginationResult Pagination(this IRepository<contact> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
       {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<contact, bool>> predicate = (e => e.ID == e.ID);
                bool DisplayUserID, DisplayRoleID, DisplayEmail, DisplayLastLogin, DisplayCreatedDate;
                bool OrderByUserID, OrderByRoleID, OrderByEmail, OrderByLastLogin, OrderByCreatedDate;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayUserID = PFilter.VisibleColumnInfoList.IndexOf("Name") > -1;
                    DisplayRoleID = PFilter.VisibleColumnInfoList.IndexOf("Email") > -1;
                    DisplayEmail = PFilter.VisibleColumnInfoList.IndexOf("Phone") > -1;
                    DisplayLastLogin = PFilter.VisibleColumnInfoList.IndexOf("Speciality") > -1;
                    DisplayCreatedDate = PFilter.VisibleColumnInfoList.IndexOf("CreatedDate") > -1;
                    predicate = (c => c.Name.ToString().Contains(PFilter.SearchText.ToLower()) &&
                    (DisplayRoleID && c.Email.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayEmail && c.Phone.Contains(PFilter.SearchText.ToLower())) ||
                    (DisplayLastLogin && c.Speciality.ToLower().Contains(PFilter.SearchText.ToLower())) ||
                    (DisplayCreatedDate && c.CreatedDate.ToString().ToLower().Contains(PFilter.SearchText.ToLower()))));
                }

                IQueryable<contact> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByUserID = PFilter.OrderBy.IndexOf("Name") > -1;
                OrderByRoleID = PFilter.OrderBy.IndexOf("Email") > -1;
                OrderByEmail = PFilter.OrderBy.IndexOf("Phone") > -1;
                OrderByLastLogin = PFilter.OrderBy.IndexOf("Speciality") > -1;
                OrderByCreatedDate = PFilter.OrderBy.IndexOf("CreatedDate") > -1;
                Expression<Func<contact, string>> orderingFunction = (c =>
                                                              OrderByUserID ? c.Name.ToString() :
                                                                OrderByRoleID ? c.Email :
                                                                  OrderByEmail ? c.Phone :
                                                                  OrderByCreatedDate ? c.CreatedDate.ToString() :
                                                              OrderByLastLogin ? c.Speciality.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderByDescending(orderingFunction);
                else
                    filteredData = filteredData.OrderBy(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    objResult.DataList = filteredData
                        .Select(s => new
                        {
                            s.ID,
                            Name = s.Name,
                            Email = s.Email,
                            Phone = s.Phone,
                            Speciality = s.Speciality,
                            CreatedDate = s.CreatedDate,
                        }).OrderByDescending(a => a.ID).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           Name = s.Name,
                           Email = s.Email,
                           Phone = s.Phone,
                           Speciality = s.Speciality,
                           CreatedDate = s.CreatedDate,
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
