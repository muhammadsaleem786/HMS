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

namespace HMS.Repository.Repositories.Admission
{
    public static class ipd_paymentRepository
    {
        public static PaginationResult PaginationWithParm(this IRepository<ipd_payment> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText,string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<ipd_payment, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayAmount, DisplayNote;
                bool OrderByAmount, OrderByNote;

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayAmount = PFilter.VisibleColumnInfoList.IndexOf("Amount") > -1;
                    DisplayNote = PFilter.VisibleColumnInfoList.IndexOf("Notes") > -1;
                    predicate = (c => c.CompanyId == CompanyID
                  );
                }

                IQueryable<ipd_payment> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByAmount = PFilter.OrderBy.IndexOf("Amount") > -1;
                OrderByNote = PFilter.OrderBy.IndexOf("Notes") > -1;
                Expression<Func<ipd_payment, string>> orderingFunction = (c =>
                                                             OrderByAmount ? c.Amount.ToString() :
                                                              OrderByNote ? c.Notes :""
                                                               );


                if (PFilter.OrderBy == "ID")
                {
                    filteredData = filteredData.OrderByDescending(d => (d.ID)).AsQueryable();
                    objResult.TotalRecord = filteredData.Count();
                }
                else
                {
                    if (PFilter.IsOrderAsc)
                        filteredData = filteredData.OrderBy(orderingFunction);
                    else
                        filteredData = filteredData.OrderByDescending(orderingFunction);
                    objResult.TotalRecord = filteredData.Count();
                }
                if (IgnorePaging)
                {
                    var result = filteredData
                   .Select(s => new
                   {
                       s.ID,
                       date = s.PaymentDate.Month + "/" + s.PaymentDate.Day + "/" + s.PaymentDate.Year,
                       amount = s.Amount,
                       Notes = s.Notes,
                   }).OrderByDescending(a => a.ID).ToList();

                    objResult.DataList = result.AsEnumerable().Select(z => new
                    {
                        ID = z.ID,
                        date = z.date,
                        amount = z.amount,
                        Notes = z.Notes,
                    }).ToList<object>();
                }
                else
                {
                    var result = filteredData
                   .Select(s => new
                   {
                       s.ID,
                       date = s.PaymentDate.Month + "/" + s.PaymentDate.Day + "/" + s.PaymentDate.Year,
                       amount = s.Amount,
                       Notes = s.Notes,
                   }).OrderByDescending(a => a.ID).ToList();

                    objResult.DataList = result.AsEnumerable().Select(z => new
                    {
                        ID = z.ID,
                        date = z.date,
                        amount = z.amount,
                        Notes = z.Notes
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
