using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Repositories.Employee;
using Repository.Pattern.Repositories;
using Service.Pattern;
namespace HMS.Service.Services.Employee
{
    public interface Ipr_employee_allowanceService : IService<pr_employee_allowance>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
    }

    public class pr_employee_allowanceService : Service<pr_employee_allowance>, Ipr_employee_allowanceService
    {
        private readonly IRepositoryAsync<pr_employee_allowance> _repository;
        public pr_employee_allowanceService(IRepositoryAsync<pr_employee_allowance> repository) : base(repository)
        {
            _repository = repository;
        }

        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
    }
}
