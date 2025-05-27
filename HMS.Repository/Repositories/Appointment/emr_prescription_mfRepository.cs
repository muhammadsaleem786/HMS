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

namespace HMS.Repository.Repositories.Appointment
{
    public static class emr_prescription_mfRepository
    {
        public static PaginationResult Pagination(this IRepository<emr_prescription_mf> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_prescription_mf, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayPatientName, DisplayAmount;
                bool OrderByPatientName, OrderByAmount;

                IQueryable<emr_prescription_mf> filteredData = repository.Queryable().Where(predicate);
                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                var emr_medicine = repository.GetRepository<emr_medicine>().Queryable().Where(a => a.CompanyID == CompanyID).Select(c => new
                {
                    c.ID,
                    c.Medicine
                });
                var dataList = filteredData.Include(I => I.emr_patient_mf).Include(I => I.emr_prescription_treatment).Select(x => new
                {
                    ID = x.ID,
                    AppointmentDate = x.AppointmentDate,
                    PatientName = x.emr_patient_mf.PatientName,
                    PaidAmount = x.emr_patient_mf.emr_patient_bill.Where(a => a.PatientId == x.emr_patient_mf.ID && (decimal?)a.DoctorId== (decimal?)x.DoctorId).Sum(c => (decimal?)c.PaidAmount),
                    OutAmount = x.emr_patient_mf.emr_patient_bill.Where(a => a.PatientId == x.emr_patient_mf.ID && (decimal?)a.DoctorId == (decimal?)x.DoctorId).Sum(c => (decimal?)c.OutstandingBalance),
                    MedicineIdList = x.emr_prescription_treatment.Where(a => a.PrescriptionId == x.ID).Select(a => new
                    {
                        mid = a.MedicineId.ToString()
                    }).ToList(),
                }).OrderByDescending(a=>a.ID).ToList();
                List<PrescriptionModel> list = new List<PrescriptionModel>();
                if (IgnorePaging)
                {
                    list = (from s in dataList
                            group s by new { s.ID, s.AppointmentDate, s.PatientName, s.PaidAmount, s.OutAmount } into grp
                            select new PrescriptionModel
                            {
                                ID = grp.Key.ID,
                                AppointmentDate = grp.Key.AppointmentDate,
                                PatientName = grp.Key.PatientName,
                                PaidAmount = Convert.ToDecimal(grp.Key.PaidAmount),
                                OutAmount = Convert.ToDecimal(grp.Key.OutAmount),
                                medicine = grp.Select(a => a.MedicineIdList.Select(x=>new { x.mid})).ToList(),
                            }).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord).ToList();
                }
                else
                {

                    list = (from s in dataList
                            group s by new { s.ID, s.AppointmentDate, s.PatientName, s.PaidAmount, s.OutAmount } into grp
                            select new PrescriptionModel
                            {
                                ID = grp.Key.ID,
                                AppointmentDate = grp.Key.AppointmentDate,
                                PatientName = grp.Key.PatientName,
                                PaidAmount =Convert.ToDecimal(grp.Key.PaidAmount),
                                OutAmount = Convert.ToDecimal(grp.Key.OutAmount),
                                medicine = grp.Select(a => a.MedicineIdList.Select(x => new { x.mid })).ToList(),
                            }).Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord).ToList();

                }

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("PatientName") > -1;
                    list = list.Where(c =>
                   (DisplayPatientName && c.PatientName.ToLower().Contains(PFilter.SearchText.ToLower()))).ToList();
                }

                OrderByPatientName = PFilter.OrderBy.IndexOf("PatientName") > -1;

                Expression<Func<PrescriptionModel, string>> orderingFunction = (c =>
                                                             OrderByPatientName ? c.PatientName.ToString() : ""
                                                              );

                IQueryable<PrescriptionModel> prList = list.AsQueryable();
                if (PFilter.IsOrderAsc)
                    prList = prList.OrderBy(orderingFunction);
                else
                    prList = prList.OrderByDescending(orderingFunction);

                objResult.TotalRecord = dataList.Count();
                objResult.DataList = prList.ToList<Object>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }


    }
}
