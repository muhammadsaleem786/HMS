using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Repositories.Employee;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace HMS.Service.Services.Employee
{
    public interface Ipr_employee_payroll_mfService : IService<pr_employee_payroll_mf>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
        PaginationResult PaginationDetail(decimal CompanyID, string UniqueId, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
        PaginationResult FilterPaginationDetail(string FilterParams, decimal CompanyID, string UniqueId, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
        PaginationResult DashboardEmpPagination(string filterParams, DashboardDefaultConDedModel DashbrdIdsmodel, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
        decimal[] GetPayschedueIds(decimal CompanyID);

    }

    public class pr_employee_payroll_mfService : Service<pr_employee_payroll_mf>, Ipr_employee_payroll_mfService
    {
        private readonly IRepositoryAsync<pr_employee_payroll_mf> _repository;
        public pr_employee_payroll_mfService(IRepositoryAsync<pr_employee_payroll_mf> repository) : base(repository)
        {
            _repository = repository;
        }

        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
        public PaginationResult PaginationDetail(decimal CompanyID, string UniqueId, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.PaginationDetail(CompanyID, UniqueId, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
        public PaginationResult FilterPaginationDetail(string FilterParams, decimal CompanyID, string UniqueId, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.FilterPaginationDetail(FilterParams, CompanyID, UniqueId, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
        public PaginationResult DashboardEmpPagination(string filterParams, DashboardDefaultConDedModel DashbrdIdsmodel, decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.DashboardEmpPagination(filterParams, DashbrdIdsmodel, CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
        public decimal[] GetPayschedueIds(decimal CompanyID)
        {
            return _repository.GetPayschedueIds(CompanyID);
        }
    }
}
