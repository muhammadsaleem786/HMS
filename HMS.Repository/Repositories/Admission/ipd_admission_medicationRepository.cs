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
    public static class ipd_admission_medicationRepository
    {
        public static PaginationResult Pagination(this IRepository<ipd_admission_medication> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string AdmitId, string PatientId, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<ipd_admission_medication, bool>> predicate = (e => e.CompanyId == CompanyID && e.AdmissionId.ToString() == AdmitId && e.PatientId.ToString() == PatientId);

                bool DisplayPatientName, DisplayDOB, DisplayEmail, DisplayMobile, DisplayMRNO, DisplayCNIC;
                bool OrderByPatientName, OrderByDOB, OrderByEmail, OrderByMobile, OrderByMRNO, OrderByCNIC;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("Note") > -1;
                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayPatientName && c.Prescription.ToString().Contains(PFilter.SearchText.ToLower())));
                }

                IQueryable<ipd_admission_medication> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByPatientName = PFilter.OrderBy.IndexOf("Prescription") > -1;


                Expression<Func<ipd_admission_medication, string>> orderingFunction = (c =>
                                                              OrderByPatientName ? c.Prescription.ToString() : ""
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
                            AuthoredBy = s.adm_user_mf.Name,
                            Prescription = s.Prescription,
                            PrescriptionDate = s.PrescriptionDate,
                            QuantityRequested = s.QuantityRequested,
                            Refills = s.Refills,
                            BillTo = s.BillTo,
                            Medicine = s.adm_item.Name,
                            Status = s.IsRequestNow == true ? "Fulfilled" : "",
                            IsActive=s.IsActive,
                        }).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           AuthoredBy = s.adm_user_mf.Name,
                           Prescription = s.Prescription,
                           PrescriptionDate = s.PrescriptionDate,
                           QuantityRequested = s.QuantityRequested,
                           Refills = s.Refills,
                           BillTo = s.BillTo,
                           Medicine = s.adm_item.Name,
                           Status = s.IsRequestNow == true ? "Fulfilled" : "",
                           IsActive = s.IsActive,
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
