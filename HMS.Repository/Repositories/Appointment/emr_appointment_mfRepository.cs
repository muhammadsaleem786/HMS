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
    public static class emr_appointment_mfRepository
    {
        public static PaginationResult Pagination(this IRepository<emr_appointment_mf> repository, string date, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                DateTime todayDate = Convert.ToDateTime(date);
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_appointment_mf, bool>> predicate = (a => a.CompanyId == CompanyID && DbFunctions.TruncateTime(a.AppointmentDate) == DbFunctions.TruncateTime(todayDate) && a.IsAdmission == false);
                bool DisplayName;
                bool OrderByName;

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Notes") > -1;
                    predicate = (c =>
                    (DisplayName && c.Notes.ToLower().Contains(PFilter.SearchText.ToLower()
                    )));
                }
                IQueryable<emr_appointment_mf> filteredData = repository.Queryable().Where(predicate);
                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("Notes") > -1;
                Expression<Func<emr_appointment_mf, string>> orderingFunction = (c =>
                                                              OrderByName ? c.Notes.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);
                objResult.TotalRecord = filteredData.Count();
                if (IgnorePaging)
                {
                    objResult.PatientList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord).Select(z => new PatientList
                    {
                        ID = z.ID,
                        DoctorName = z.adm_user_mf2.Name,
                        PatientName = z.emr_patient_mf.PatientName,
                        AppointmentDate = z.AppointmentDate,
                        AppointmentTime = z.AppointmentTime,
                        DoctorId = z.DoctorId,
                        StatusId = z.StatusId,
                        CNIC = z.emr_patient_mf.CNIC,
                        PatientId = z.PatientId,
                        CreatedBy = z.adm_user_mf.Name,
                        Gender = z.emr_patient_mf.Gender,
                        Image = z.emr_patient_mf.Image,
                        Note = z.Notes,
                    }).OrderByDescending(a => a.StatusId).ThenBy(a => a.AppointmentDate).ToList();
                }
                else
                {
                    objResult.PatientList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord).Select(z => new PatientList
                    {
                        ID = z.ID,
                        DoctorName = z.adm_user_mf2.Name,
                        PatientName = z.emr_patient_mf.PatientName,
                        AppointmentDate = z.AppointmentDate,
                        AppointmentTime = z.AppointmentTime,
                        DoctorId = z.DoctorId,
                        StatusId = z.StatusId,
                        CNIC = z.emr_patient_mf.CNIC,
                        PatientId = z.PatientId,
                        CreatedBy = z.adm_user_mf.Name,
                        Gender = z.emr_patient_mf.Gender,
                        Image = z.emr_patient_mf.Image,
                        Note = z.Notes,
                    }).OrderByDescending(a => a.StatusId).ThenBy(a => a.AppointmentDate).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }

        public static PaginationResult PreviousAppointmentLoad(this IRepository<emr_appointment_mf> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string PatientId, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal patientid = Convert.ToDecimal(PatientId);
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_appointment_mf, bool>> predicate = (e => e.CompanyId == ID && e.PatientId == patientid && e.IsAdmission == false);

                bool DisplayName;
                bool OrderByName;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Notes") > -1;
                    predicate = (c =>
                    (DisplayName && c.Notes.ToLower().Contains(PFilter.SearchText.ToLower()
                    )));
                }

                IQueryable<emr_appointment_mf> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("Notes") > -1;

                Expression<Func<emr_appointment_mf, string>> orderingFunction = (c =>
                                                              OrderByName ? c.Notes.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);


                var emr_appointment_mf = repository.GetRepository<emr_appointment_mf>().Queryable().Where(a => a.CompanyId == ID && a.PatientId == patientid);
                var emr_patient_bill = repository.GetRepository<emr_patient_bill>().Queryable().Where(a => a.CompanyId == ID);
                var sys_drop_down = repository.GetRepository<sys_drop_down_value>().Queryable().Where(a => a.DropDownID == 1);
                var obj = (from appointment_mf in emr_appointment_mf
                           select new
                           {
                               ID = appointment_mf.ID,
                               DoctorName = appointment_mf.adm_user_mf2.Name,
                               PatientName = appointment_mf.emr_patient_mf.PatientName,
                               AppointmentDate = appointment_mf.AppointmentDate,
                               AppointmentTime = appointment_mf.AppointmentTime,
                               DoctorId = appointment_mf.DoctorId,
                               PatientId = appointment_mf.PatientId,
                               CompanyId = appointment_mf.CompanyId,
                               Note = appointment_mf.Notes,
                               PatientProblem = appointment_mf.PatientProblem,
                               CreatedBy = appointment_mf.adm_user_mf.Name,
                               Status = sys_drop_down.Where(a => a.ID == appointment_mf.StatusId).FirstOrDefault().Value,

                           }).OrderByDescending(A => A.ID).ToList();

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    var DataList = obj.AsEnumerable().Select(m => new
                    {
                        ID = m.ID,
                        DoctorName = m.DoctorName,
                        PatientName = m.PatientName,
                        DoctorId = m.DoctorId,
                        Note = m.Note,
                        PatientProblem = m.PatientProblem,
                        PatientId = m.PatientId,
                        CompanyId = m.CompanyId,
                        Amount = emr_patient_bill.Where(a => a.PatientId == m.PatientId && a.DoctorId == m.DoctorId && a.AppointmentId == m.ID && a.BillDate.Day == m.AppointmentDate.Day && a.BillDate.Month == m.AppointmentDate.Month && a.BillDate.Year == m.AppointmentDate.Year).FirstOrDefault() == null ? 0 : emr_patient_bill.Where(a => a.PatientId == m.PatientId && a.DoctorId == m.DoctorId && a.AppointmentId==m.ID && a.BillDate.Day == m.AppointmentDate.Day && a.BillDate.Month == m.AppointmentDate.Month && a.BillDate.Year == m.AppointmentDate.Year).Sum(z => z.PaidAmount),
                        OutStandingAmount = emr_patient_bill.Where(a => a.PatientId == m.PatientId && a.DoctorId == m.DoctorId && a.AppointmentId == m.ID && a.BillDate.Day == m.AppointmentDate.Day && a.BillDate.Month == m.AppointmentDate.Month && a.BillDate.Year == m.AppointmentDate.Year).FirstOrDefault() == null ? 0 : emr_patient_bill.Where(a => a.PatientId == m.PatientId && a.DoctorId == m.DoctorId && a.AppointmentId == m.ID && a.BillDate.Day == m.AppointmentDate.Day && a.BillDate.Month == m.AppointmentDate.Month && a.BillDate.Year == m.AppointmentDate.Year).Sum(z => z.OutstandingBalance),
                        CreatedBy = m.CreatedBy,
                        AppointmentDate = m.AppointmentDate,
                        Status = m.Status,
                        StartDate = m.AppointmentDate.ToString("yyyy-MM-dd") + " " + m.AppointmentTime.ToString("hh\\:mm\\:ss"),
                        EndDate = m.AppointmentDate.ToString(),
                        BillingStatus = "Open"
                    }).OrderByDescending(A => A.ID).ToList();

                    var PreviousPatientList = DataList.Where(x => x.AppointmentDate.Date <= DateTime.Now.Date).ToList();

                    var PageResult = PreviousPatientList.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    objResult.DataList = (from c in PageResult select c).ToList<object>();
                }
                else
                {

                    var DataList = obj.AsEnumerable().Select(m => new
                    {
                        ID = m.ID,
                        DoctorName = m.DoctorName,
                        PatientName = m.PatientName,
                        DoctorId = m.DoctorId,
                        Note = m.Note,
                        PatientProblem = m.PatientProblem,
                        PatientId = m.PatientId,
                        CompanyId = m.CompanyId,
                        Amount = emr_patient_bill.Where(a => a.PatientId == m.PatientId && a.BillDate.Day == m.AppointmentDate.Day && a.BillDate.Month == m.AppointmentDate.Month && a.BillDate.Year == m.AppointmentDate.Year).FirstOrDefault() == null ? 0 : emr_patient_bill.Where(a => a.PatientId == m.PatientId  && a.BillDate.Day == m.AppointmentDate.Day && a.BillDate.Month == m.AppointmentDate.Month && a.BillDate.Year == m.AppointmentDate.Year).Sum(z => z.PaidAmount),
                        OutStandingAmount = emr_patient_bill.Where(a => a.PatientId == m.PatientId && a.BillDate.Day == m.AppointmentDate.Day && a.BillDate.Month == m.AppointmentDate.Month && a.BillDate.Year == m.AppointmentDate.Year).FirstOrDefault() == null ? 0 : emr_patient_bill.Where(a => a.PatientId == m.PatientId &&  a.BillDate.Day == m.AppointmentDate.Day && a.BillDate.Month == m.AppointmentDate.Month && a.BillDate.Year == m.AppointmentDate.Year).Sum(z => z.OutstandingBalance),
                        CreatedBy = m.CreatedBy,
                        AppointmentDate = m.AppointmentDate,
                        Status = m.Status,
                        StartDate = m.AppointmentDate.ToString("yyyy-MM-dd") + " " + m.AppointmentTime.ToString("hh\\:mm\\:ss"),
                        EndDate = m.AppointmentDate.ToString(),
                        BillingStatus = "Open"
                    }).OrderByDescending(A => A.ID).ToList();

                    var PreviousPatientList = DataList.Where(x => x.AppointmentDate.Date <= DateTime.Now.Date).ToList();
                    var FuturePatientList = DataList.Where(x => x.AppointmentDate.Date >= DateTime.Now.Date).ToList();
                    objResult.OtherDataModel = FuturePatientList;
                    var PageResult = PreviousPatientList.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord);
                    objResult.DataList = (from c in PageResult select c).ToList<object>();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }

        public static PaginationResult AdmitAppointmentLoad(this IRepository<emr_appointment_mf> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string PatientId, string AdmissionId, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_appointment_mf, bool>> predicate = (a => a.CompanyId == ID && a.PatientId.ToString() == PatientId && a.AdmissionId.ToString() == AdmissionId && a.IsAdmission == true);
                bool DisplayName;
                bool OrderByName;

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Notes") > -1;
                    predicate = (c =>
                    (DisplayName && c.Notes.ToLower().Contains(PFilter.SearchText.ToLower()
                    )));
                }
                IQueryable<emr_appointment_mf> filteredData = repository.Queryable().Where(predicate);
                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("Notes") > -1;
                Expression<Func<emr_appointment_mf, string>> orderingFunction = (c =>
                                                              OrderByName ? c.Notes.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);
                objResult.TotalRecord = filteredData.Count();
                if (IgnorePaging)
                {
                    objResult.PatientList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord).Select(z => new PatientList
                    {
                        ID = z.ID,
                        DoctorName = z.adm_user_mf2.Name,
                        PatientName = z.emr_patient_mf.PatientName,
                        AppointmentDate = z.AppointmentDate,
                        AppointmentTime = z.AppointmentTime,
                        DoctorId = z.DoctorId,
                        StatusId = z.StatusId,
                        CNIC = z.emr_patient_mf.CNIC,
                        PatientId = z.PatientId,
                        CreatedBy = z.adm_user_mf.Name,
                        Gender = z.emr_patient_mf.Gender,
                        Image = z.emr_patient_mf.Image,
                        Note = z.Notes,
                    }).OrderByDescending(a => a.StatusId).ThenBy(a => a.AppointmentDate).ToList();
                }
                else
                {
                    objResult.PatientList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord).Select(z => new PatientList
                    {
                        ID = z.ID,
                        DoctorName = z.adm_user_mf2.Name,
                        PatientName = z.emr_patient_mf.PatientName,
                        AppointmentDate = z.AppointmentDate,
                        AppointmentTime = z.AppointmentTime,
                        DoctorId = z.DoctorId,
                        StatusId = z.StatusId,
                        CNIC = z.emr_patient_mf.CNIC,
                        PatientId = z.PatientId,
                        CreatedBy = z.adm_user_mf.Name,
                        Gender = z.emr_patient_mf.Gender,
                        Image = z.emr_patient_mf.Image,
                        Note = z.Notes,
                    }).OrderByDescending(a => a.StatusId).ThenBy(a => a.AppointmentDate).ToList();
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

