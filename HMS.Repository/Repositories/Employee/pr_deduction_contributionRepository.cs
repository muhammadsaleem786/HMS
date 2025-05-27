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

namespace HMS.Repository.Repositories.Employee
{
    public static class pr_deduction_contributionRepository
    {
        public static PaginationResult Pagination(this IRepository<pr_deduction_contribution> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<pr_deduction_contribution, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayName, DisplayCategory, DisplayAmntOrPercentage;
                bool OrderByName, OrderByCategory, OrderByAmntOrPercentage;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("DeductionContributionName") > -1;
                    DisplayCategory = PFilter.VisibleColumnInfoList.IndexOf("Category") > -1;
                    DisplayAmntOrPercentage = PFilter.VisibleColumnInfoList.IndexOf("DeductionContributionValue") > -1;
                    predicate = (c =>
                    c.CompanyID == CompanyID &&
                    (DisplayName && c.DeductionContributionName.ToLower().Contains(PFilter.SearchText.ToLower())
                    || (DisplayCategory && (c.Category == "C" ? "Contribution" : "Deduction").ToLower().ToString().Contains(PFilter.SearchText.ToLower()))
                    || (DisplayAmntOrPercentage && c.DeductionContributionValue.ToString().Contains(PFilter.SearchText))
                    ));
                }

                IQueryable<pr_deduction_contribution> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "DeductionContributionName";

                OrderByName = PFilter.OrderBy.IndexOf("DeductionContributionName") > -1;
                OrderByCategory = PFilter.OrderBy.IndexOf("Category") > -1;
                OrderByAmntOrPercentage = PFilter.OrderBy.IndexOf("DeductionContributionValue") > -1;

                Expression<Func<pr_deduction_contribution, string>> orderingFunction = (c =>
                                                              OrderByName ? c.DeductionContributionName :
                                                              OrderByCategory ? c.Category :
                                                              OrderByAmntOrPercentage ? c.DeductionContributionValue.ToString() : "0"
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();
                string currency = ConfigurationManager.AppSettings["Currency"];
                if (IgnorePaging)
                {
                    //objResult.DataList = (from c in filteredData select c).ToList<object>();
                    objResult.DataList = filteredData.Select(s => new
                    {
                        s.ID,
                        DeductionContributionName = s.Default ? s.DeductionContributionName + " (Default)" : s.DeductionContributionName,
                        Category = s.Category == "C" ? "Contribution" : "Deduction",
                        DeductionContributionType = s.DeductionContributionType,
                        DeductionContributionValue = s.DeductionContributionValue
                    }).ToList().Select(z => new
                    {
                        z.ID,
                        z.DeductionContributionName,
                        z.Category,
                        DeductionContributionValue = z.DeductionContributionType == "P" ? (currency + ": " + String.Format("{0:n0}", z.DeductionContributionValue) + "%") : (currency + ": " + String.Format("{0:n0}", z.DeductionContributionValue)),
                    }).ToList<object>();
                }
                else
                {
                    var PageResult = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    //objResult.DataList = (from c in filteredData select c).ToList<object>();
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           DeductionContributionName = s.Default ? s.DeductionContributionName + " (Default)" : s.DeductionContributionName,
                           Category = s.Category == "C" ? "Contribution" : "Deduction",
                           DeductionContributionType = s.DeductionContributionType,
                           DeductionContributionValue = s.DeductionContributionValue,
                       }).ToList().Select(z => new
                       {
                           z.ID,
                           z.DeductionContributionName,
                           z.Category,
                           DeductionContributionValue = z.DeductionContributionType == "P" ? (currency + ": " + String.Format("{0:n0}", z.DeductionContributionValue) + "%") : (currency + ": " + String.Format("{0:n0}", z.DeductionContributionValue)),
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
