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
 
    public interface Iemr_appointment_mfService : IService<emr_appointment_mf>
    {
        PaginationResult Pagination(string date, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
        PaginationResult PreviousAppointmentLoad(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string PatientId, bool IgnorePaging = false);

        PaginationResult AdmitAppointmentLoad(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string PatientId,string AdmissionId, bool IgnorePaging = false);

    }

    public class emr_appointment_mfService : Service<emr_appointment_mf>, Iemr_appointment_mfService
    {
        private readonly IRepositoryAsync<emr_appointment_mf> _repository;
        public emr_appointment_mfService(IRepositoryAsync<emr_appointment_mf> repository) : base(repository)
        {
            _repository = repository;
        }

        public PaginationResult Pagination(string date, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(date, CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
        public PaginationResult PreviousAppointmentLoad(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string PatientId, bool IgnorePaging = false)
        {
            return _repository.PreviousAppointmentLoad(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, PatientId, IgnorePaging);
        }
        public PaginationResult AdmitAppointmentLoad(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string PatientId,string AdmissionId, bool IgnorePaging = false)
        {
            return _repository.AdmitAppointmentLoad(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, PatientId, AdmissionId, IgnorePaging);
        }

    }
}
