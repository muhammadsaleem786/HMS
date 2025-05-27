using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Repositories.Admin;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace HMS.Service.Services.Admin
{
    public interface Iuser_paymentService : IService<user_payment>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
    }

    public class user_paymentService : Service<user_payment>, Iuser_paymentService
    {
        private readonly IRepositoryAsync<user_payment> _repository;
        public user_paymentService(IRepositoryAsync<user_payment> repository) : base(repository)
        {
            _repository = repository;
        }

        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
    }
}
