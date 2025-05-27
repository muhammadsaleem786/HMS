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

    public static class adm_companyRepository
    {
        public static PaginationResult Pagination(this IRepository<adm_company> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_company, bool>> predicate = (e => e.ID == ID);

                //bool DisplayLocationName, DisplayUserName, DisplayPhoneNo, DisplayEmail;
                //bool OrderByName, OrderByUserName, OrderByPhoneNo, OrderByEmail;


                //if (!string.IsNullOrEmpty(PFilter.SearchText))
                //{
                //    DisplayLocationName = PFilter.VisibleColumnInfoList.IndexOf("LocationName") > -1;
                //    //DisplayUserName = PFilter.VisibleColumnInfoList.IndexOf("UserName") > -1;
                //    predicate = (c =>
                //    (DisplayLocationName && c.Name.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                //    //(DisplayEmail && c.Email.ToLower().Contains(PFilter.SearchText.ToLower())

                //    //
                //    )));
                //}

                //IQueryable<adm_user> filteredData = repository.Queryable().Where(predicate);

                ////if (string.IsNullOrEmpty(PFilter.OrderBy))
                ////    PFilter.OrderBy = "TaxCode";

                //OrderByName = PFilter.OrderBy.IndexOf("Name") > -1;
                ////OrderByUserName = PFilter.OrderBy.IndexOf("UserName") > -1;
                //OrderByPhoneNo = PFilter.OrderBy.IndexOf("PhoneNo") > -1;
                //OrderByEmail = PFilter.OrderBy.IndexOf("Email") > -1;

                //Expression<Func<adm_user, string>> orderingFunction = (c =>
                //                                              OrderByName ? c.Name :
                //                                              OrderByEmail ? c.Email : ""
                //                                              );

                //if (PFilter.IsOrderAsc)
                //    filteredData = filteredData.OrderBy(orderingFunction);
                //else
                //    filteredData = filteredData.OrderByDescending(orderingFunction);

                //objResult.TotalRecord = filteredData.Count();

                //if (IgnorePaging)
                //    objResult.DataList = (from c in filteredData select c).ToList<object>();
                //else
                //{
                //    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                //    objResult.DataList = (from c in PageResult select c).ToList<object>();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
    }
}
