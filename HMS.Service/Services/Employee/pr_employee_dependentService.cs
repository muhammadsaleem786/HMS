using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using HMS.Repository.Repositories.Employee;

namespace HMS.Service.Services.Employee
{
    public interface Ipr_employee_dependentService : IService<pr_employee_Dependent>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
    }

    public class pr_employee_dependentService : Service<pr_employee_Dependent>, Ipr_employee_dependentService
    {
        private readonly IRepositoryAsync<pr_employee_Dependent> _repository;
        public pr_employee_dependentService(IRepositoryAsync<pr_employee_Dependent> repository) : base(repository)
        {
            _repository = repository;
        }
        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
    }
}
