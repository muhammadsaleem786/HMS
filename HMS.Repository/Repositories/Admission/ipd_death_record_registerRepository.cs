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
    public static class ipd_death_record_registerRepository
    {
        public static PaginationResult Pagination(this IRepository<ipd_death_record_register> repository, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            var objResult = new PaginationResult();
            try
            {
                var PFilter = Utility.SetPaginationFilter(CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText);
                Expression<Func<ipd_death_record_register, bool>> predicate = (e => e.CompanyID == CompanyID);

                bool DisplayPatientName, DisplayDOB, DisplayEmail, DisplayMobile, DisplayMRNO, DisplayCNIC;
                bool OrderByPatientName, OrderByDOB, OrderByEmail, OrderByMobile, OrderByMRNO, OrderByCNIC;


                if (!string.IsNullOrEmpty(PFilter.SearchText))
                {
                    DisplayPatientName = PFilter.VisibleColumnInfoList.IndexOf("PatientName") > -1;
                    DisplayDOB = PFilter.VisibleColumnInfoList.IndexOf("DeathDate") > -1;
                    predicate = (c => c.CompanyID == CompanyID &&
                    (DisplayPatientName && c.PatientName.ToString().Contains(PFilter.SearchText.ToLower()) ||
                    (DisplayDOB && c.DeathDate.ToString().Contains(PFilter.SearchText.ToLower()))));
                }

                IQueryable<ipd_death_record_register> filteredData = repository.Queryable().Where(predicate);

                if (string.IsNullOrEmpty(PFilter.OrderBy))
                    PFilter.OrderBy = "ID";

                OrderByPatientName = PFilter.OrderBy.IndexOf("PatientName") > -1;
                OrderByDOB = PFilter.OrderBy.IndexOf("DeathDate") > -1;


                Expression<Func<ipd_death_record_register, string>> orderingFunction = (c =>
                                                              OrderByPatientName ? c.PatientName.ToString() :
                                                                OrderByDOB ? c.DeathDate.ToString()  : ""
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
                            DeathDate = s.DeathDate,
                            MRNo = s.MRNo,
                            FatherHusband = s.FatherHusbandName,
                            s.IDCardNo,s.ContractNo,
                            s.DateTimeOfDeath,
                            s.ProbableCauseOfDeath,
                            s.DeathCertificateHandedOverTo,
                        }).ToList<object>();
                }
                else
                {

                    objResult.DataList = filteredData.Skip(PFilter.SkipRecord).Take(PFilter.TakeRecord)
                       .Select(s => new
                       {
                           s.ID,
                           PatientName = s.PatientName,
                           DeathDate = s.DeathDate,
                           MRNo = s.MRNo,
                           FatherHusband = s.FatherHusbandName,
                           s.IDCardNo,
                           s.ContractNo,
                           s.DateTimeOfDeath,
                           s.ProbableCauseOfDeath,
                           s.DeathCertificateHandedOverTo,
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
