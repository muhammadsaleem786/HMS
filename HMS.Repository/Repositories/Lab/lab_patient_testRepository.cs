using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Common;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Repository.Repositories.Lab
{
    public static class lab_patient_testRepository
    {
        public static PaginationResult Pagination(this IRepository<lab_patient_test> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {

            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<lab_patient_test, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayName, DisplayPatientName, DisplaySampleName;
                bool OrderByName, OrderByPatientName, OrderBySampleName;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Test") > -1;
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("Patient") > -1;
                    DisplaySampleName = PFilter.VisibleColumnInfoList.IndexOf("Sample") > -1;

                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayName && c.ID.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                     || (DisplayPatientName && c.emr_patient_mf.PatientName.ToLower().Contains(PFilter.SearchText.ToLower()))
                     || (DisplaySampleName && c.SerialNo.ToLower().Contains(PFilter.SearchText.ToLower()))));
                }

                IQueryable<lab_patient_test> filteredData = repository.Queryable().Where(predicate);

                var baseQuery = repository.Queryable().Where(x => x.CompanyId == CompanyID);
                var dashboardCounts = baseQuery
                       .GroupBy(x => 1)
                       .Select(g => new
                       {
                           PendingTests = g.Count(x => x.StatusId == 1),
                           InProgressTests = g.Count(x => x.StatusId == 2),
                           CompletedToday = g.Count(x => x.StatusId == 4 && x.ModifiedDate.HasValue && DbFunctions.TruncateTime(x.ModifiedDate.Value) == DbFunctions.TruncateTime(DateTime.Now)),
                           UrgentTests = g.Count(x => x.PriorityId == 2)
                       }).FirstOrDefault();

                objResult.OtherDataModel = dashboardCounts;

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("Test") > -1;
                OrderByPatientName = PFilter.OrderBy.IndexOf("PatientName") > -1;
                OrderBySampleName = PFilter.OrderBy.IndexOf("SampleName") > -1;

                Expression<Func<lab_patient_test, string>> orderingFunction = (c =>
                                                              OrderByName ? c.ID.ToString() :
                                                                OrderByPatientName ? c.emr_patient_mf.PatientName :
                                                                  OrderBySampleName ? c.SerialNo : "0"
                                                             );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                      .Include(i => i.sys_drop_down_value)
                      .Select(s => new
                      {
                          testId = s.ID,
                          s.PatientId,
                          patientName = s.emr_patient_mf.PatientName,
                          patientCode = s.emr_patient_mf.MRNO,
                          serialNo = s.SerialNo,
                          testName = s.emr_investigation.Investigation,
                          sampleType = s.emr_investigation.sys_drop_down_value1.Value,
                          department = s.emr_investigation.sys_drop_down_value.Value,
                          priority = s.sys_drop_down_value.Value,
                          status = s.sys_drop_down_value1.Value,
                          statusId = s.StatusId,
                          orderedDate = s.CreatedDate,
                          catalogId = s.CatalogId
                      }).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                    .Include(i => i.sys_drop_down_value)
                    .Select(s => new
                    {
                        testId = s.ID,
                        s.PatientId,
                        patientName = s.emr_patient_mf.PatientName,
                        patientCode = s.emr_patient_mf.MRNO,
                        serialNo = s.SerialNo,
                        testName = s.emr_investigation.Investigation,
                        sampleType = s.emr_investigation.sys_drop_down_value1.Value,
                        department = s.emr_investigation.sys_drop_down_value.Value,
                        priority = s.sys_drop_down_value.Value,
                        status = s.sys_drop_down_value1.Value,
                        statusId = s.StatusId,
                        orderedDate = s.CreatedDate,
                        catalogId = s.CatalogId
                    }).ToList<object>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objResult;
        }
        public static PaginationResult PaginationWithParm(this IRepository<lab_patient_test> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            string departmentId = "";
            string statusId = "";
            string sampleTypeId = "";
            if (!string.IsNullOrWhiteSpace(FilterID))
            {
                var filters = FilterID.Split(',');

                departmentId = filters.Length > 0 ? filters[0] : "";
                statusId = filters.Length > 1 ? filters[1] : "";
                sampleTypeId = filters.Length > 2 ? filters[2] : "";
            }
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<lab_patient_test, bool>> predicate = (e => e.CompanyId == CompanyID);

                bool DisplayName, DisplayPatientName, DisplaySampleName;
                bool OrderByName, OrderByPatientName, OrderBySampleName;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayName = PFilter.VisibleColumnInfoList.IndexOf("Test") > -1;
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("Patient") > -1;
                    DisplaySampleName = PFilter.VisibleColumnInfoList.IndexOf("Sample") > -1;

                    predicate = (c => c.CompanyId == CompanyID &&
                    (DisplayName && c.ID.ToString().ToLower().Contains(PFilter.SearchText.ToLower())
                     || (DisplayPatientName && c.emr_patient_mf.PatientName.ToLower().Contains(PFilter.SearchText.ToLower()))
                     || (DisplaySampleName && c.SerialNo.ToLower().Contains(PFilter.SearchText.ToLower()))));
                }

                IQueryable<lab_patient_test> filteredData = repository.Queryable().Where(predicate);
                if (!string.IsNullOrEmpty(departmentId))
                {
                    int deptId = Convert.ToInt32(departmentId);
                    filteredData = filteredData.Where(x =>
                        x.emr_investigation.CategoryId == deptId);
                }

                if (!string.IsNullOrEmpty(statusId))
                {
                    int statId = Convert.ToInt32(statusId);
                    filteredData = filteredData.Where(x =>
                        x.StatusId == statId);
                }

                if (!string.IsNullOrEmpty(sampleTypeId))
                {
                    int sampleId = Convert.ToInt32(sampleTypeId);
                    filteredData = filteredData.Where(x =>
                        x.emr_investigation.SampleId == sampleId);
                }
                var baseQuery = repository.Queryable().Where(x => x.CompanyId == CompanyID);
                var dashboardCounts = baseQuery
                       .GroupBy(x => 1)
                       .Select(g => new
                       {
                           PendingTests = g.Count(x => x.StatusId == 1),
                           InProgressTests = g.Count(x => x.StatusId == 2),
                           CompletedToday = g.Count(x => x.StatusId == 4 && x.ModifiedDate.HasValue && DbFunctions.TruncateTime(x.ModifiedDate.Value) == DbFunctions.TruncateTime(DateTime.Now)),
                           UrgentTests = g.Count(x => x.PriorityId == 2)
                       }).FirstOrDefault();

                objResult.OtherDataModel = dashboardCounts;

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByName = PFilter.OrderBy.IndexOf("Test") > -1;
                OrderByPatientName = PFilter.OrderBy.IndexOf("PatientName") > -1;
                OrderBySampleName = PFilter.OrderBy.IndexOf("SampleName") > -1;

                Expression<Func<lab_patient_test, string>> orderingFunction = (c =>
                                                              OrderByName ? c.ID.ToString() :
                                                                OrderByPatientName ? c.emr_patient_mf.PatientName :
                                                                  OrderBySampleName ? c.SerialNo : "0"
                                                             );

                if (PFilter.IsOrderAsc)
                    filteredData = filteredData.OrderBy(orderingFunction);
                else
                    filteredData = filteredData.OrderByDescending(orderingFunction);

                objResult.TotalRecord = filteredData.Count();

                if (IgnorePaging)
                {
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                      .Include(i => i.sys_drop_down_value)
                      .Select(s => new
                      {
                          testId = s.ID,
                          s.PatientId,
                          patientName = s.emr_patient_mf.PatientName,
                          patientCode = s.emr_patient_mf.MRNO,
                          serialNo = s.SerialNo,
                          testName = s.emr_investigation.Investigation,
                          sampleType = s.emr_investigation.sys_drop_down_value1.Value,
                          department = s.emr_investigation.sys_drop_down_value.Value,
                          priority = s.sys_drop_down_value.Value,
                          status = s.sys_drop_down_value1.Value,
                          statusId = s.StatusId,
                          orderedDate = s.CreatedDate,
                          catalogId = s.CatalogId,
                          invoiceno = s.InvoiceNo
                      }).OrderByDescending(a=>a.testId).ToList<object>();
                }
                else
                {
                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                    .Include(i => i.sys_drop_down_value)
                    .Select(s => new
                    {
                        testId = s.ID,
                        s.PatientId,
                        patientName = s.emr_patient_mf.PatientName,
                        patientCode = s.emr_patient_mf.MRNO,
                        serialNo = s.SerialNo,
                        testName = s.emr_investigation.Investigation,
                        sampleType = s.emr_investigation.sys_drop_down_value1.Value,
                        department = s.emr_investigation.sys_drop_down_value.Value,
                        priority = s.sys_drop_down_value.Value,
                        status = s.sys_drop_down_value1.Value,
                        statusId = s.StatusId,
                        orderedDate = s.CreatedDate,
                        catalogId = s.CatalogId,
                        invoiceno = s.InvoiceNo
                    }).OrderByDescending(a => a.testId).ToList<object>();
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
