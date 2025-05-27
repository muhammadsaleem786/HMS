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

namespace HMS.Repository.Repositories.Admission
{
    public static class ipd_procedure_mfRepository
    {
        public static PaginationResult Pagination(this IRepository<ipd_procedure_mf> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string AdmitId, string PatientId, string Appointmentid, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<ipd_procedure_mf, bool>> predicate = (e => e.CompanyId == CompanyID && e.AdmissionId.ToString() == AdmitId && e.PatientId.ToString() == PatientId);

                bool DisplayPatientName, DisplayDOB, DisplayEmail, DisplayMobile, DisplayMRNO, DisplayCNIC;
                bool OrderByPatientName, OrderByDOB, OrderByEmail, OrderByMobile, OrderByMRNO, OrderByCNIC;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("Date") > -1;
                    DisplayDOB = PFilter.VisibleColumnInfoList.IndexOf("PatientProcedure") > -1;
                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayPatientName && c.Date.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayDOB && c.PatientProcedure.ToString().Contains(PFilter.SearchText.ToLower())
                    )));
                }

                IQueryable<ipd_procedure_mf> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByPatientName = PFilter.OrderBy.IndexOf("Date") > -1;
                OrderByDOB = PFilter.OrderBy.IndexOf("PatientProcedure") > -1;


                Expression<Func<ipd_procedure_mf, string>> orderingFunction = (c =>
                                                              OrderByPatientName ? c.Date.ToString() :
                                                                OrderByDOB ? c.PatientProcedure.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                filteredData = filteredData.OrderByDescending(d => (d.ID)).AsQueryable();
                objResult.TotalRecord = filteredData.Count();
                if (IgnorePaging)
                {

                    objResult.DataList = filteredData
                        .Select(s => new
                        {
                            s.ID,
                            Date = s.Date,
                            PatientProcedure = s.PatientProcedure,
                            Price = s.Price,
                            PaidAmount = s.PaidAmount,
                            Balance = s.Price - s.PaidAmount,
                            DischargeDate = s.ipd_admission.DischargeDate
                        }).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           Date = s.Date,
                           PatientProcedure = s.PatientProcedure,
                           Price = s.Price,
                           PaidAmount = s.PaidAmount,
                           Balance = s.Price - s.PaidAmount,
                           DischargeDate = s.ipd_admission.DischargeDate

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
