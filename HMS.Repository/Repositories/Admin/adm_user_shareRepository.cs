using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Repositories;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace HMS.Repository.Repositories.Admin
{
    public static class adm_user_shareRepository
    {
        public static PaginationResult PaginationWithParm(this IRepository<adm_user_share> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<adm_user_share, bool>> predicate = (e => e.CompanyId == ID);

                bool DisplayDate, DisplayCategory, DisplayAmount, DisplayReceivedAmount;
                bool OrderByDate, OrderByCategory, OrderByAmount, OrderByReceivedAmount;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayDate = PFilter.VisibleColumnInfoList.IndexOf("Date") > -1;
                    DisplayCategory = PFilter.VisibleColumnInfoList.IndexOf("Category") > -1;
                    DisplayAmount = PFilter.VisibleColumnInfoList.IndexOf("DueAmount") > -1;
                    DisplayReceivedAmount = PFilter.VisibleColumnInfoList.IndexOf("ReceivedAmount") > -1;
                    predicate = (c => c.CompanyId == ID &&
                    (DisplayDate && c.GeneralDoctorShare.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayAmount && c.GeneralOwnerShare.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                     )));
                }
                if (IgnorePaging)
                {
                    predicate = (c => c.CompanyId == ID);
                }
                IQueryable<adm_user_share> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";
                OrderByDate = PFilter.OrderBy.IndexOf("Date") > -1;
                OrderByCategory = PFilter.OrderBy.IndexOf("Category") > -1;
                OrderByAmount = PFilter.OrderBy.IndexOf("DueAmount") > -1;
                OrderByReceivedAmount = PFilter.OrderBy.IndexOf("ReceivedAmount") > -1;
                Expression<Func<adm_user_share, string>> orderingFunction = (c =>
                                                              OrderByCategory ? c.GeneralDoctorShare.ToString() :
                                                              OrderByAmount ? c.GeneralOwnerShare.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {

                    objResult.DataList = filteredData
                        .Select(s => new
                        {
                            s.ID,
                            DoctorName=s.adm_user_mf2.Name,
                            GeneralDoctorShare = s.GeneralDoctorShare,
                            GeneralOwnerShare = s.GeneralOwnerShare,
                            ProcedureDoctorShare = s.ProcedureDoctorShare,
                            ProcedureOwnerShare = s.ProcedureOwnerShare,
                            DiscountDoctorShare = s.DiscountDoctorShare,
                            DiscountOwnerShare = s.DiscountOwnerShare,
                            CreatedDate = s.CreatedDate
                        }).OrderByDescending(A => A.CreatedDate).ToList<object>();
                }
                else
                {
                    var data = filteredData
                        .Select(s => new
                        {
                            s.ID,
                            DoctorName = s.adm_user_mf2.Name,
                            GeneralDoctorShare = s.GeneralDoctorShare,
                            GeneralOwnerShare = s.GeneralOwnerShare,
                            ProcedureDoctorShare = s.ProcedureDoctorShare,
                            ProcedureOwnerShare = s.ProcedureOwnerShare,
                            DiscountDoctorShare = s.DiscountDoctorShare,
                            DiscountOwnerShare = s.DiscountOwnerShare,
                            CreatedDate = s.CreatedDate
                        }).OrderByDescending(A => A.CreatedDate).ToList();
                    objResult.DataList = data.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           DoctorName=s.DoctorName,
                           GeneralDoctorShare = s.GeneralDoctorShare,
                           GeneralOwnerShare = s.GeneralOwnerShare,
                           ProcedureDoctorShare = s.ProcedureDoctorShare,
                           ProcedureOwnerShare = s.ProcedureOwnerShare,
                           DiscountDoctorShare = s.DiscountDoctorShare,
                           DiscountOwnerShare = s.DiscountOwnerShare,
                           CreatedDate = s.CreatedDate
                       }).OrderByDescending(A => A.CreatedDate).ToList<object>();
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
