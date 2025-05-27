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
    public static class ipd_admission_vitalRepository
    {
        public static PaginationResult Pagination(this IRepository<ipd_admission_vital> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string AdmitId, string PatientId, string Appointmentid, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<ipd_admission_vital, bool>> predicate = (e => e.CompanyId == CompanyID && e.AdmissionId.ToString() == AdmitId && e.PatientId.ToString() == PatientId && e.AppointmentId.ToString() == Appointmentid);

                bool DisplayPatientName, DisplayDOB, DisplayEmail, DisplayMobile, DisplayMRNO, DisplayCNIC;
                bool OrderByPatientName, OrderByDOB, OrderByEmail, OrderByMobile, OrderByMRNO, OrderByCNIC;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("DateRecorded") > -1;
                    DisplayDOB = PFilter.VisibleColumnInfoList.IndexOf("Temperature") > -1;
                    DisplayEmail = PFilter.VisibleColumnInfoList.IndexOf("Weight") > -1;
                    DisplayMobile = PFilter.VisibleColumnInfoList.IndexOf("Height") > -1;
                    DisplayMRNO = PFilter.VisibleColumnInfoList.IndexOf("BP") > -1;
                    DisplayCNIC = PFilter.VisibleColumnInfoList.IndexOf("SPO2") > -1;
                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayPatientName && c.DateRecorded.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayDOB && c.Temperature.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayEmail && c.Weight.ToString().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayMobile && c.Height.ToString().Contains(PFilter.SearchText.ToLower()) ||
                      (DisplayMRNO && c.BP.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayCNIC && c.SPO2.ToString().Contains(PFilter.SearchText)

                    )))))));
                }

                IQueryable<ipd_admission_vital> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByPatientName = PFilter.OrderBy.IndexOf("DateRecorded") > -1;
                OrderByDOB = PFilter.OrderBy.IndexOf("Temperature") > -1;
                OrderByEmail = PFilter.OrderBy.IndexOf("Weight") > -1;
                OrderByMobile = PFilter.OrderBy.IndexOf("Height") > -1;
                OrderByMRNO = PFilter.OrderBy.IndexOf("BP") > -1;
                OrderByCNIC = PFilter.OrderBy.IndexOf("SPO2") > -1;


                Expression<Func<ipd_admission_vital, string>> orderingFunction = (c =>
                                                              OrderByPatientName ? c.DateRecorded.ToString() :
                                                                OrderByDOB ? c.Temperature.ToString() :
                                                                  OrderByEmail ? c.Weight.ToString() :
                                                                   OrderByMobile ? c.Height.ToString() :
                                                                    OrderByMRNO ? c.BP.ToString() :
                                                              OrderByCNIC ? c.SPO2.ToString() : ""
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
                            TakenBy = s.adm_user_mf.Name,
                            DateRecorded = s.DateRecorded,
                            TimeRecorded = s.TimeRecorded,
                            Temperature = s.Temperature,
                            Weight = s.Weight,
                            Height = s.Height,
                            BP = s.BP,
                            SPO2 = s.SPO2,
                            HeartRate = s.HeartRate,
                            RespiratoryRate = s.RespiratoryRate,
                        }).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           TakenBy = s.adm_user_mf.Name,
                           s.ID,
                           TimeRecorded = s.TimeRecorded,
                           DateRecorded = s.DateRecorded,
                           Temperature = s.Temperature,
                           Weight = s.Weight,
                           Height = s.Height,
                           BP = s.BP,
                           SPO2 = s.SPO2,
                           HeartRate = s.HeartRate,
                           RespiratoryRate = s.RespiratoryRate,
                       }).ToList<object>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
        public static PaginationResult OrderPagination(this IRepository<ipd_admission_vital> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string AdmitId, string PatientId, string Appointmentid, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<ipd_admission_vital, bool>> predicate = (e => e.CompanyId == CompanyID && e.AdmissionId.ToString() == AdmitId && e.PatientId.ToString() == PatientId);

                bool DisplayPatientName, DisplayDOB, DisplayEmail, DisplayMobile, DisplayMRNO, DisplayCNIC;
                bool OrderByPatientName, OrderByDOB, OrderByEmail, OrderByMobile, OrderByMRNO, OrderByCNIC;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("DateRecorded") > -1;
                    DisplayDOB = PFilter.VisibleColumnInfoList.IndexOf("Temperature") > -1;
                    DisplayEmail = PFilter.VisibleColumnInfoList.IndexOf("Weight") > -1;
                    DisplayMobile = PFilter.VisibleColumnInfoList.IndexOf("Height") > -1;
                    DisplayMRNO = PFilter.VisibleColumnInfoList.IndexOf("BP") > -1;
                    DisplayCNIC = PFilter.VisibleColumnInfoList.IndexOf("SPO2") > -1;
                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayPatientName && c.DateRecorded.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayDOB && c.Temperature.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayEmail && c.Weight.ToString().Contains(PFilter.SearchText.ToLower()) ||
                     (DisplayMobile && c.Height.ToString().Contains(PFilter.SearchText.ToLower()) ||
                      (DisplayMRNO && c.BP.ToLower().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayCNIC && c.SPO2.ToString().Contains(PFilter.SearchText)

                    )))))));
                }

                IQueryable<ipd_admission_vital> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByPatientName = PFilter.OrderBy.IndexOf("DateRecorded") > -1;
                OrderByDOB = PFilter.OrderBy.IndexOf("Temperature") > -1;
                OrderByEmail = PFilter.OrderBy.IndexOf("Weight") > -1;
                OrderByMobile = PFilter.OrderBy.IndexOf("Height") > -1;
                OrderByMRNO = PFilter.OrderBy.IndexOf("BP") > -1;
                OrderByCNIC = PFilter.OrderBy.IndexOf("SPO2") > -1;


                Expression<Func<ipd_admission_vital, string>> orderingFunction = (c =>
                                                              OrderByPatientName ? c.DateRecorded.ToString() :
                                                                OrderByDOB ? c.Temperature.ToString() :
                                                                  OrderByEmail ? c.Weight.ToString() :
                                                                   OrderByMobile ? c.Height.ToString() :
                                                                    OrderByMRNO ? c.BP.ToString() :
                                                              OrderByCNIC ? c.SPO2.ToString() : ""
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
                            TakenBy = s.adm_user_mf.Name,
                            DateRecorded = s.DateRecorded,
                            Temperature = s.Temperature,
                            Weight = s.Weight,
                            Height = s.Height,
                            BP = s.BP,
                            SPO2 = s.SPO2,
                            HeartRate = s.HeartRate,
                            RespiratoryRate = s.RespiratoryRate,
                        }).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           TakenBy = s.adm_user_mf.Name,
                           s.ID,
                           DateRecorded = s.DateRecorded,
                           Temperature = s.Temperature,
                           Weight = s.Weight,
                           Height = s.Height,
                           BP = s.BP,
                           SPO2 = s.SPO2,
                           HeartRate = s.HeartRate,
                           RespiratoryRate = s.RespiratoryRate,
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
