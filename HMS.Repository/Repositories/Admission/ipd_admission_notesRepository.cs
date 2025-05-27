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
    public static class ipd_admission_notesRepository
    {
        public static PaginationResult Pagination(this IRepository<ipd_admission_notes> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText,string AdmitId, string PatientId, string Appointmentid, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<ipd_admission_notes, bool>> predicate = (e => e.CompanyId == CompanyID && e.AdmissionId.ToString()==AdmitId && e.PatientId.ToString() == PatientId);

                bool DisplayPatientName, DisplayDOB, DisplayEmail, DisplayMobile, DisplayMRNO, DisplayCNIC;
                bool OrderByPatientName, OrderByDOB, OrderByEmail, OrderByMobile, OrderByMRNO, OrderByCNIC;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("Note") > -1;
                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayPatientName && c.Note.ToString().Contains(PFilter.SearchText.ToLower())));
                }

                IQueryable<ipd_admission_notes> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByPatientName = PFilter.OrderBy.IndexOf("Note") > -1;


                Expression<Func<ipd_admission_notes, string>> orderingFunction = (c =>
                                                              OrderByPatientName ? c.Note.ToString() : ""
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
                            Note = s.Note,
                            Date=s.CreatedDate,
                            DischargeDate=s.ipd_admission.DischargeDate
                        }).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           AuthoredBy = s.adm_user_mf.Name,
                           Note = s.Note,
                           Date = s.CreatedDate,
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
