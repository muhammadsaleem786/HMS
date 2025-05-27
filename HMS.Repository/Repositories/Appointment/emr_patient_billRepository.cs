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

namespace HMS.Repository.Repositories.Appointment
{
    public static class emr_patient_billRepository
    {
        public static PaginationResult Pagination(this IRepository<emr_patient_bill> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_patient_bill, bool>> predicate = (e => e.CompanyId == ID);

                bool DisplayPatientName, DisplayAdmissionNo;
                bool OrderByPatientName, OrderByAdmissionNo;

                var adm_user_company = repository.GetRepository<adm_user_company>().Queryable();
                var adm_role_mf = repository.GetRepository<adm_role_mf>().Queryable();
                var adm_user_mf = repository.GetRepository<adm_user_mf>().Queryable();
                var sys_drop_down = repository.GetRepository<sys_drop_down_value>().Queryable();
                var obj = (from usercompany in adm_user_company
                           join usermf in adm_user_mf on usercompany.UserID equals usermf.ID
                           join rolemf in adm_role_mf on new { a = usercompany.RoleID, b = usercompany.adm_role_mf.RoleName, c = usercompany.CompanyID }
                           equals new { a = rolemf.ID, b = "Doctor", c = ID }
                           select new DoctorList
                           {
                               Name = usermf.Name,
                               ID = usermf.ID,
                           }).OrderByDescending(a => a.ID).ToList();

                // PFilter.VisibleColumnInfoList.IndexOf("Doctor") > -1;

                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("Patient") > -1;
                    DisplayAdmissionNo = PFilter.VisibleColumnInfoList.IndexOf("AdmissionNo") > -1;
                    predicate = (c => c.CompanyId == ID &&
                    (DisplayPatientName && c.emr_patient_mf.PatientName.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayAdmissionNo && c.ipd_admission.AdmissionNo.ToString().Contains(PFilter.SearchText.ToLower())
                    )));

                }

                IQueryable<emr_patient_bill> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByPatientName = PFilter.OrderBy.IndexOf("Patient") > -1;
                OrderByAdmissionNo = PFilter.OrderBy.IndexOf("AdmissionNo") > -1;
                Expression<Func<emr_patient_bill, string>> orderingFunction = (c =>
                                                               OrderByPatientName ? c.emr_patient_mf.PatientName.ToString() :
                                                               OrderByAdmissionNo ? c.ipd_admission.AdmissionNo.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);







                filteredData = filteredData.OrderByDescending(d => (d.ID)).AsQueryable();
                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                           .Select(s => new
                           {
                               ID = s.ID,
                               Price = s.Price,
                               Service = s.emr_service_mf.ServiceName,
                               PatientName = s.emr_patient_mf.PatientName,
                               Date = s.BillDate,
                               PaidAmount = s.PaidAmount,
                               Discount = s.Discount,
                               Amount = s.Price,
                               OutstandingBalance = s.OutstandingBalance,
                               CreatedBy = s.adm_user_mf.Name,
                               DoctorId = s.DoctorId,
                               AdmissionId = s.AdmissionId,
                               AppointmentId = s.AppointmentId,
                               PatientId = s.PatientId,
                               AdmissionNo =s.ipd_admission.AdmissionNo,
                           }).ToList().Select(z => new
                           {
                               ID = z.ID,
                               Price = z.Price,
                               Service = z.Service,
                               PatientName = z.PatientName,
                               Date = z.Date,
                               PaidAmount = z.PaidAmount,
                               Discount = z.Discount,
                               Amount = z.Amount,
                               OutstandingBalance = z.OutstandingBalance,
                               CreatedBy = z.CreatedBy,
                               AdmissionNo=z.AdmissionNo,
                               DoctorId = z.DoctorId,
                               AdmissionId = z.AdmissionId,
                               AppointmentId = z.AppointmentId,
                               PatientId = z.PatientId,
                               DoctorName = z.DoctorId == 0 ? "" : obj.AsEnumerable().Where(a => a.ID == z.DoctorId).FirstOrDefault().Name,
                           }).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                          .Select(s => new
                          {
                              ID = s.ID,
                              Price = s.Price,
                              Service = s.emr_service_mf.ServiceName,
                              PatientName = s.emr_patient_mf.PatientName,
                              Date = s.BillDate,
                              PaidAmount = s.PaidAmount,
                              Discount = s.Discount,
                              Amount = s.Price,
                              OutstandingBalance = s.OutstandingBalance,
                              CreatedBy = s.adm_user_mf.Name,
                              DoctorId = s.DoctorId,
                              AdmissionId = s.AdmissionId,
                              AppointmentId = s.AppointmentId,
                              PatientId = s.PatientId,
                              AdmissionNo = s.ipd_admission.AdmissionNo,
                          }).ToList().Select(z => new
                          {
                              ID = z.ID,
                              Price = z.Price,
                              Service = z.Service,
                              PatientName = z.PatientName,
                              Date = z.Date,
                              PaidAmount = z.PaidAmount,
                              Discount = z.Discount,
                              Amount = z.Amount,
                              OutstandingBalance = z.OutstandingBalance,
                              CreatedBy = z.CreatedBy,
                              DoctorId = z.DoctorId,
                              AdmissionId = z.AdmissionId,
                              AppointmentId = z.AppointmentId,
                              PatientId = z.PatientId,
                              AdmissionNo = z.AdmissionNo,
                              DoctorName = z.DoctorId == 0 ? "" : obj.AsEnumerable().Where(a => a.ID == z.DoctorId).FirstOrDefault().Name,
                          }).ToList<object>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
          public static PaginationResult BillingList(this IRepository<emr_patient_bill> repository, decimal ID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string PatientId, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                decimal patientid = Convert.ToDecimal(PatientId);
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_patient_bill, bool>> predicate = (e => e.CompanyId == ID && e.PatientId == patientid);

                bool DisplayName;
                bool OrderByName;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Patient") > -1;
                    predicate = (c =>
                    (DisplayName && c.emr_patient_mf.PatientName.ToString().ToLower().Contains(PFilter.SearchText.ToLower()
                    )));
                }

                IQueryable<emr_patient_bill> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("Patient") > -1;

                Expression<Func<emr_patient_bill, string>> orderingFunction = (c =>
                                                              OrderByName ? c.emr_patient_mf.PatientName.ToString() : ""
                                                              );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                var adm_user_company = repository.GetRepository<adm_user_company>().Queryable();
                var adm_role_mf = repository.GetRepository<adm_role_mf>().Queryable();
                var adm_user_mf = repository.GetRepository<adm_user_mf>().Queryable();
                var sys_drop_down = repository.GetRepository<sys_drop_down_value>().Queryable();
                var obj = (from usercompany in adm_user_company
                           join usermf in adm_user_mf on usercompany.UserID equals usermf.ID
                           join rolemf in adm_role_mf on new { a = usercompany.RoleID, b = usercompany.adm_role_mf.RoleName, c = usercompany.CompanyID }
                           equals new { a = rolemf.ID, b = "Doctor", c = ID }
                           select new DoctorList
                           {
                               Name = usermf.Name,
                               ID = usermf.ID,
                           }).OrderByDescending(a => a.ID).ToList();

                filteredData = filteredData.OrderByDescending(d => (d.ID)).AsQueryable();

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    var DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                                           .Select(s => new
                                           {
                                               ID = s.ID,
                                               Price = s.Price,
                                               Service = s.emr_service_mf.ServiceName,
                                               PatientName = s.emr_patient_mf.PatientName,
                                               Date = s.BillDate,
                                               PaidAmount = s.PaidAmount,
                                               Discount = s.Discount,
                                               Amount = s.Price,
                                               OutstandingBalance = s.OutstandingBalance,
                                               CreatedBy = s.adm_user_mf.Name,
                                               DoctorId = s.DoctorId,
                                           }).ToList();

                    objResult.DataList = DataList.AsEnumerable()
                                    .Select(z => new
                                    {
                                        ID = z.ID,
                                        Price = z.Price,
                                        Service = z.Service,
                                        PatientName = z.PatientName,
                                        Date = z.Date,
                                        PaidAmount = z.PaidAmount,
                                        Discount = z.Discount,
                                        Amount = z.Amount,
                                        OutstandingBalance = z.OutstandingBalance,
                                        CreatedBy = z.CreatedBy,
                                        DoctorId = z.DoctorId,
                                        DoctorName = z.DoctorId == null ? "" : obj.AsEnumerable().Where(a => a.ID == z.DoctorId).FirstOrDefault().Name,
                                    }).ToList<object>();
                }
                else
                {

                    var DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                                           .Select(s => new
                                           {
                                               ID = s.ID,
                                               Price = s.Price,
                                               Service = s.emr_service_mf.ServiceName,
                                               PatientName = s.emr_patient_mf.PatientName,
                                               Date = s.BillDate,
                                               PaidAmount = s.PaidAmount,
                                               Discount = s.Discount,
                                               Amount = s.Price,
                                               OutstandingBalance = s.OutstandingBalance,
                                               CreatedBy = s.adm_user_mf.Name,
                                               DoctorId = s.DoctorId,
                                           }).ToList();

                    objResult.DataList = DataList.AsEnumerable()
                                    .Select(z => new
                                    {
                                        ID = z.ID,
                                        Price = z.Price,
                                        Service = z.Service,
                                        PatientName = z.PatientName,
                                        Date = z.Date,
                                        PaidAmount = z.PaidAmount,
                                        Discount = z.Discount,
                                        Amount = z.Amount,
                                        OutstandingBalance = z.OutstandingBalance,
                                        CreatedBy = z.CreatedBy,
                                        DoctorId = z.DoctorId,
                                        DoctorName = z.DoctorId == null ? "" : obj.AsEnumerable().Where(a => a.ID == z.DoctorId).FirstOrDefault().Name,
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
