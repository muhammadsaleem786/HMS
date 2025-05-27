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
    public interface Iipd_admission_chargesService : IService<ipd_admission_charges>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText,string AdmitId, bool IgnorePaging = false);

    }

    public class ipd_admission_chargesService : Service<ipd_admission_charges>, Iipd_admission_chargesService
    {
        private readonly IRepositoryAsync<ipd_admission_charges> _repository;
        public ipd_admission_chargesService(IRepositoryAsync<ipd_admission_charges> repository) : base(repository)
        {
            _repository = repository;
        }

        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText,string AdmitId, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, AdmitId, IgnorePaging);
        }
    }
}
