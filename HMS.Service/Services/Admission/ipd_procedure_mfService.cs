using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Repositories.Admission;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service.Services.Admission
{
    public interface Iipd_procedure_mfService : IService<ipd_procedure_mf>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText,string AdmitId, string PatientId, string Appointmentid, bool IgnorePaging = false);

    }

    public class ipd_procedure_mfService : Service<ipd_procedure_mf>, Iipd_procedure_mfService
    {
        private readonly IRepositoryAsync<ipd_procedure_mf> _repository;
        public ipd_procedure_mfService(IRepositoryAsync<ipd_procedure_mf> repository) : base(repository)
        {
            _repository = repository;
        }

        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText,string AdmitId, string PatientId, string Appointmentid, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, AdmitId, PatientId, Appointmentid, IgnorePaging);
        }
    }
}
