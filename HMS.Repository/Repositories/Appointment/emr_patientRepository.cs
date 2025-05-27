using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Repository.Repositories.Appointment
{
    public static class emr_patientRepository
    {
        public static PaginationResult Pagination(this IRepository<emr_patient_mf> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<emr_patient_mf, bool>> predicate = (e => e.CompanyId == CompanyID);

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
                    (DisplayPatientName && c.PatientName.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayDOB && c.DOB.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayEmail && c.Email.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayMobile && c.Mobile.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                      (DisplayMRNO && c.MRNO.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayCNIC && c.CNIC.ToString().Contains(PFilter.SearchText)

                    )))))));
                }

                IQueryable<emr_patient_mf> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByPatientName = PFilter.OrderBy.IndexOf("PatientName") > -1;
                OrderByDOB = PFilter.OrderBy.IndexOf("DOB") > -1;
                OrderByEmail = PFilter.OrderBy.IndexOf("Email") > -1;
                OrderByMobile = PFilter.OrderBy.IndexOf("Mobile") > -1;
                OrderByMRNO = PFilter.OrderBy.IndexOf("MRNO") > -1;
                OrderByCNIC = PFilter.OrderBy.IndexOf("CNIC") > -1;


                Expression<Func<emr_patient_mf, string>> orderingFunction = (c =>
                                                              OrderByPatientName ? c.PatientName.ToString() :
                                                                OrderByDOB ? c.DOB.ToString() :
                                                                  OrderByEmail ? c.Email :
                                                                   OrderByMobile ? c.Mobile :
                                                                    OrderByMRNO ? c.MRNO :
                                                              OrderByCNIC ? c.CNIC.ToString() : ""
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
                            PatientName = s.PatientName,
                            DOB = s.DOB,
                            Email = s.Email,
                            Mobile = s.Mobile,
                            MRNO = s.MRNO,
                            CNIC = s.CNIC,
                            Image = s.Image,
                            Gender = s.Gender,
                            Age=s.Age
                        }).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           PatientName = s.PatientName,
                           DOB = s.DOB,
                           Email = s.Email,
                           Mobile = s.Mobile,
                           MRNO = s.MRNO,
                           Gender = s.Gender,
                           Image = s.Image,
                           CNIC = s.CNIC,
                           Age = s.Age

                       }).ToList<object>();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }

        public static PaginationResult SerachDoctorList(this IRepository<emr_patient_mf> repository, decimal CompanyID, decimal userid)
        {
            var objResult = new PaginationResult();
            try
            {

                var adm_user_company = repository.GetRepository<adm_user_company>().Queryable();
                var adm_role_mf = repository.GetRepository<adm_role_mf>().Queryable();
                var adm_user_mf = repository.GetRepository<adm_user_mf>().Queryable();

                var fileList = (from usercompany in adm_user_company
                                join usermf in adm_user_mf on usercompany.UserID equals usermf.ID
                                join rolemf in adm_role_mf on new { a = usercompany.RoleID, b = usercompany.adm_role_mf.RoleName, c = usercompany.CompanyID }
                                equals new { a = rolemf.ID, b = "Doctor", c = CompanyID }
                                select new DoctorList
                                {
                                    DoctorName = usermf.Name,
                                    DoctorId = usermf.ID,
                                }).OrderBy(a => a.DoctorId).ToList();


                objResult.DoctList = fileList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
        public static PaginationResult DoctorList(this IRepository<emr_patient_mf> repository, decimal CompanyID, decimal userid)
        {
            var objResult = new PaginationResult();
            try
            {
                DataAccessManager dataAccessManager = new DataAccessManager();
                var ht = new Hashtable();
                ht.Add("@CompanyId", CompanyID);
                ht.Add("@UserId", userid);
                var result = dataAccessManager.GetDataSet("SP_GetDoctorList", ht).Tables[0];
                List<DoctorList> Doclist = new List<DoctorList>();
                foreach (DataRow item in result.Rows)
                {
                    DoctorList lvm = new DoctorList();
                    lvm.Name = item["Name"].ToString();
                    lvm.ID = Convert.ToDecimal(item["ID"].ToString());
                    lvm.StartTime = item["StartTime"].ToString();
                    lvm.EndTime = item["EndTime"].ToString();
                    lvm.OffDay = item["OffDay"].ToString();
                    lvm.IsShowDoctor = item["IsShowDoctor"].ToString();
                    lvm.IsDoctor = Convert.ToBoolean(item["IsDoctor"].ToString());
                    lvm.Qualification = item["Qualification"].ToString();
                    lvm.Designation = item["Designation"].ToString();
                    lvm.PhoneNo = item["PhoneNo"].ToString();
                    lvm.Footer = item["RepotFooter"].ToString();
                    lvm.TemplateId= item["TemplateId"].ToString();
                    lvm.NameUrdu = item["NameUrdu"].ToString();
                    lvm.HeaderDescription = item["HeaderDescription"].ToString();
                    lvm.QualificationUrdu = item["QualificationUrdu"].ToString();
                    lvm.DesignationUrdu = item["DesignationUrdu"].ToString();
                    Doclist.Add(lvm);
                }
                objResult.DoctList = Doclist.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
    }
}
