using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Repositories.Appointment;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service.Services.Appointment
{ 
    public interface Iemr_patient_billService : IService<emr_patient_bill>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
        PaginationResult BillingList(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string PatientId, bool IgnorePaging = false);

    }
    public class emr_patient_billService : Service<emr_patient_bill>, Iemr_patient_billService
    {
        private readonly IRepositoryAsync<emr_patient_bill> _repository;
        public emr_patient_billService(IRepositoryAsync<emr_patient_bill> repository) : base(repository)
        {
            _repository = repository;
        }
        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
        public PaginationResult BillingList(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string PatientId, bool IgnorePaging = false)
        {
            return _repository.BillingList(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, PatientId, IgnorePaging);
        }
    }
}
