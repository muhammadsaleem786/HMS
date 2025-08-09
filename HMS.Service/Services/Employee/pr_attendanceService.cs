using HMS.Entities.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using HMS.Entities.CustomModel;
using HMS.Repository.Repositories.Employee;

namespace HMS.Service.Services.Employee
{
    public interface Ipr_attendanceService : IService<pr_attendance>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
    }

    public class pr_attendanceService : Service<pr_attendance>, Ipr_attendanceService
    {
        private readonly IRepositoryAsync<pr_attendance> _repository;
        public pr_attendanceService(IRepositoryAsync<pr_attendance> repository) : base(repository)
        {
            _repository = repository;
        }

        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
    }
}
