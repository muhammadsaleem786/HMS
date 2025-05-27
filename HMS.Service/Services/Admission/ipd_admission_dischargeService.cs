using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Repositories.Admin;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace HMS.Service.Services.Admin
{
    public interface Iipd_admission_dischargeService : IService<ipd_admission_discharge>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
    }

    public class ipd_admission_dischargeService : Service<ipd_admission_discharge>, Iipd_admission_dischargeService
    {
        private readonly IRepositoryAsync<ipd_admission_discharge> _repository;
        public ipd_admission_dischargeService(IRepositoryAsync<ipd_admission_discharge> repository) : base(repository)
        {
            _repository = repository;
        }

        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
    }
}
