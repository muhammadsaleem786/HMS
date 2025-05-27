using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Repositories.Employee;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace HMS.Service.Services.Employee
{
    public interface Ipr_leave_applicationService : IService<pr_leave_application>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
    }

    public class pr_leave_applicationService : Service<pr_leave_application>, Ipr_leave_applicationService
    {
        private readonly IRepositoryAsync<pr_leave_application> _repository;
        public pr_leave_applicationService(IRepositoryAsync<pr_leave_application> repository) : base(repository)
        {
            _repository = repository;
        }

        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
    }
}
