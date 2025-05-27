using System;
using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Repositories.Appointment;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace HMS.Service.Services.Appointment
{
    public interface Iemr_patientService : IService<emr_patient_mf>
    {
       
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
        PaginationResult DoctorList(decimal CompanyID,decimal userid);
        PaginationResult SerachDoctorList(decimal CompanyID, decimal userid);
    }

    public class emr_patientService : Service<emr_patient_mf>, Iemr_patientService
    {
        private readonly IRepositoryAsync<emr_patient_mf> _repository;

        public emr_patientService(IRepositoryAsync<emr_patient_mf> repository) : base(repository)
        {
            _repository = repository;
        }

        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
        public PaginationResult DoctorList(decimal companyid,decimal userid)
        {
            return _repository.DoctorList(companyid, userid);
        }
        public PaginationResult SerachDoctorList(decimal companyid, decimal userid)
        {
            return _repository.SerachDoctorList(companyid, userid);
        }
    }
}
