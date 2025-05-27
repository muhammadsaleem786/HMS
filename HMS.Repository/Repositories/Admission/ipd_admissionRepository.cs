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
    public static class ipd_admissionRepository
    {
        public static PaginationResult Pagination(this IRepository<ipd_admission> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<ipd_admission, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayPatientName, DisplayDOB, DisplayEmail, DisplayMobile, DisplayMRNO, DisplayCNIC;
                bool OrderByPatientName, OrderByDOB, OrderByEmail, OrderByMobile, OrderByMRNO, OrderByCNIC;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("PatientName") > -1;
                    DisplayDOB = PFilter.VisibleColumnInfoList.IndexOf("DOB") > -1;
                    DisplayEmail = PFilter.VisibleColumnInfoList.IndexOf("Email") > -1;
                    DisplayMobile = PFilter.VisibleColumnInfoList.IndexOf("Mobile") > -1;
                    DisplayMRNO = PFilter.VisibleColumnInfoList.IndexOf("MRNO") > -1;
                    DisplayCNIC = PFilter.VisibleColumnInfoList.IndexOf("CNIC") > -1;
                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayPatientName && c.emr_patient_mf.PatientName.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayDOB && c.emr_patient_mf.DOB.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayEmail && c.emr_patient_mf.Email.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayMobile && c.emr_patient_mf.Mobile.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                      (DisplayMRNO && c.emr_patient_mf.MRNO.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayCNIC && c.emr_patient_mf.CNIC.ToString().Contains(PFilter.SearchText)

                    )))))));
                }

                IQueryable<ipd_admission> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByPatientName = PFilter.OrderBy.IndexOf("PatientName") > -1;
                OrderByDOB = PFilter.OrderBy.IndexOf("DOB") > -1;
                OrderByEmail = PFilter.OrderBy.IndexOf("Email") > -1;
                OrderByMobile = PFilter.OrderBy.IndexOf("Mobile") > -1;
                OrderByMRNO = PFilter.OrderBy.IndexOf("MRNO") > -1;
                OrderByCNIC = PFilter.OrderBy.IndexOf("CNIC") > -1;


                Expression<Func<ipd_admission, string>> orderingFunction = (c =>
                                                              OrderByPatientName ? c.emr_patient_mf.PatientName.ToString() :
                                                                OrderByDOB ? c.emr_patient_mf.DOB.ToString() :
                                                                  OrderByEmail ? c.emr_patient_mf.Email :
                                                                   OrderByMobile ? c.emr_patient_mf.Mobile :
                                                                    OrderByMRNO ? c.emr_patient_mf.MRNO :
                                                              OrderByCNIC ? c.emr_patient_mf.CNIC.ToString() : ""
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
                            AdmissionNo = s.AdmissionNo,
                            Type = s.sys_drop_down_value.Value,
                            AdmissionDate = s.AdmissionDate,
                            AdmissionTime = s.AdmissionTime,
                            PatientName = s.emr_patient_mf.PatientName,
                            DOB = s.emr_patient_mf.DOB,
                            PatientId = s.PatientId,
                            Email = s.emr_patient_mf.Email,
                            Mobile = s.emr_patient_mf.Mobile,
                            MRNO = s.emr_patient_mf.MRNO,
                            CNIC = s.emr_patient_mf.CNIC,
                            Image = s.emr_patient_mf.Image,
                            Gender = s.emr_patient_mf.Gender,
                            Ward = s.sys_drop_down_value1.Value,
                            Bed = s.sys_drop_down_value2.Value,
                            Room = s.sys_drop_down_value3.Value,
                            Status = s.DischargeDate == null ? "Admitted" : "Discharge",
                            DaysCount = filteredData.Count() == 0 || s.AdmissionDate == null ? 0 : (s.AdmissionDate.Date - DateTime.Now.Date).TotalDays
                            //appid = s.emr_patient_mf.emr_appointment_mf.Where(a => a.AdmissionId == s.ID).FirstOrDefault(),
                        }).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           AdmissionNo = s.AdmissionNo,
                           Type = s.sys_drop_down_value.Value,
                           AdmissionDate = s.AdmissionDate,
                           AdmissionTime = s.AdmissionTime,
                           PatientName = s.emr_patient_mf.PatientName,
                           DOB = s.emr_patient_mf.DOB,
                           PatientId = s.PatientId,
                           Email = s.emr_patient_mf.Email,
                           Mobile = s.emr_patient_mf.Mobile,
                           MRNO = s.emr_patient_mf.MRNO,
                           Gender = s.emr_patient_mf.Gender,
                           Image = s.emr_patient_mf.Image,
                           CNIC = s.emr_patient_mf.CNIC,
                           Ward = s.sys_drop_down_value1.Value,
                           Bed = s.sys_drop_down_value2.Value,
                           Room = s.sys_drop_down_value3.Value,
                           Status = s.DischargeDate == null ? "Admitted" : "Discharge",
                           DaysCount = 0//filteredData.Count()==0 || s.AdmissionDate == null ? 0 : (s.AdmissionDate.Date - DateTime.Now.Date).TotalDays
                       }).ToList<object>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
        public static PaginationResult PaginationWithParm(this IRepository<ipd_admission> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<ipd_admission, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayPatientName, DisplayDOB, DisplayEmail, DisplayMobile, DisplayMRNO, DisplayCNIC;
                bool OrderByPatientName, OrderByDOB, OrderByEmail, OrderByMobile, OrderByMRNO, OrderByCNIC;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("PatientName") > -1;
                    DisplayDOB = PFilter.VisibleColumnInfoList.IndexOf("DOB") > -1;
                    DisplayEmail = PFilter.VisibleColumnInfoList.IndexOf("Email") > -1;
                    DisplayMobile = PFilter.VisibleColumnInfoList.IndexOf("Mobile") > -1;
                    DisplayMRNO = PFilter.VisibleColumnInfoList.IndexOf("MRNO") > -1;
                    DisplayCNIC = PFilter.VisibleColumnInfoList.IndexOf("CNIC") > -1;
                    predicate = (c => c.CompanyId == CompanyID 
                    && (FilterID == "0" || c.AdmissionTypeId.ToString() == FilterID) 
                    &&(
                    (DisplayPatientName && c.emr_patient_mf.PatientName.ToString().Contains(PFilter.SearchText.ToLower())) ||
                    (DisplayDOB && c.emr_patient_mf.DOB.ToString().Contains(PFilter.SearchText.ToLower())) ||
                    (DisplayEmail && c.emr_patient_mf.Email.ToLower().Contains(PFilter.SearchText.ToLower())) ||
                     (DisplayMobile && c.emr_patient_mf.Mobile.ToLower().Contains(PFilter.SearchText.ToLower())) ||
                      (DisplayMRNO && c.emr_patient_mf.MRNO.ToLower().Contains(PFilter.SearchText.ToLower())) ||
                    (DisplayCNIC && c.emr_patient_mf.CNIC.ToString().Contains(PFilter.SearchText))

                    )
                    );
                }
                if (FilterID != "0")
                {
                    predicate = (c => c.CompanyId == CompanyID && c.AdmissionTypeId.ToString()== FilterID);
                }
                IQueryable<ipd_admission> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByPatientName = PFilter.OrderBy.IndexOf("PatientName") > -1;
                OrderByDOB = PFilter.OrderBy.IndexOf("DOB") > -1;
                OrderByEmail = PFilter.OrderBy.IndexOf("Email") > -1;
                OrderByMobile = PFilter.OrderBy.IndexOf("Mobile") > -1;
                OrderByMRNO = PFilter.OrderBy.IndexOf("MRNO") > -1;
                OrderByCNIC = PFilter.OrderBy.IndexOf("CNIC") > -1;


                Expression<Func<ipd_admission, string>> orderingFunction = (c =>
                                                              OrderByPatientName ? c.emr_patient_mf.PatientName.ToString() :
                                                                OrderByDOB ? c.emr_patient_mf.DOB.ToString() :
                                                                  OrderByEmail ? c.emr_patient_mf.Email :
                                                                   OrderByMobile ? c.emr_patient_mf.Mobile :
                                                                    OrderByMRNO ? c.emr_patient_mf.MRNO :
                                                              OrderByCNIC ? c.emr_patient_mf.CNIC.ToString() : ""
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
                            AdmissionNo = s.AdmissionNo,
                            Type = s.sys_drop_down_value.Value,
                            AdmissionDate = s.AdmissionDate,
                            AdmissionTime = s.AdmissionTime,
                            PatientName = s.emr_patient_mf.PatientName,
                            DOB = s.emr_patient_mf.DOB,
                            PatientId = s.PatientId,
                            Email = s.emr_patient_mf.Email,
                            Mobile = s.emr_patient_mf.Mobile,
                            MRNO = s.emr_patient_mf.MRNO,
                            CNIC = s.emr_patient_mf.CNIC,
                            Image = s.emr_patient_mf.Image,
                            Gender = s.emr_patient_mf.Gender,
                            Ward = s.sys_drop_down_value1.Value,
                            Bed = s.sys_drop_down_value2.Value,
                            Room = s.sys_drop_down_value3.Value,
                            Status = s.DischargeDate == null ? "Admitted" : "Discharge",
                            DaysCount = filteredData.Count() == 0 || s.AdmissionDate == null ? 0 : (s.AdmissionDate.Date - DateTime.Now.Date).TotalDays
                            //appid = s.emr_patient_mf.emr_appointment_mf.Where(a => a.AdmissionId == s.ID).FirstOrDefault(),
                        }).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           AdmissionNo = s.AdmissionNo,
                           Type = s.sys_drop_down_value.Value,
                           AdmissionDate = s.AdmissionDate,
                           AdmissionTime = s.AdmissionTime,
                           PatientName = s.emr_patient_mf.PatientName,
                           DOB = s.emr_patient_mf.DOB,
                           PatientId = s.PatientId,
                           Email = s.emr_patient_mf.Email,
                           Mobile = s.emr_patient_mf.Mobile,
                           MRNO = s.emr_patient_mf.MRNO,
                           Gender = s.emr_patient_mf.Gender,
                           Image = s.emr_patient_mf.Image,
                           CNIC = s.emr_patient_mf.CNIC,
                           Ward = s.sys_drop_down_value1.Value,
                           Bed = s.sys_drop_down_value2.Value,
                           Room = s.sys_drop_down_value3.Value,
                           Status = s.DischargeDate == null ? "Admitted" : "Discharge",
                           DaysCount = 0//filteredData.Count()==0 || s.AdmissionDate == null ? 0 : (s.AdmissionDate.Date - DateTime.Now.Date).TotalDays
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
