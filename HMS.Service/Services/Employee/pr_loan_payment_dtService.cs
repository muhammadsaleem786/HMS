using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Repositories.Employee;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace HMS.Service.Services.Employee
{
    public interface Ipr_loan_payment_dtService : IService<pr_loan_payment_dt>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
    }

    public class pr_loan_payment_dtService : Service<pr_loan_payment_dt>, Ipr_loan_payment_dtService
    {
        private readonly IRepositoryAsync<pr_loan_payment_dt> _repository;
        public pr_loan_payment_dtService(IRepositoryAsync<pr_loan_payment_dt> repository) : base(repository)
        {
            _repository = repository;
        }

        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
    }
}
