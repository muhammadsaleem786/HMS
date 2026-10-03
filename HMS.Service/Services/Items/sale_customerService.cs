using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using HMS.Repository.Repositories.Items;

namespace HMS.Service.Services.Items
{
    public interface Isale_customerService : IService<sale_customer>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
    }
    public class sale_customerService : Service<sale_customer>, Isale_customerService
    {
        private readonly IRepositoryAsync<sale_customer> _repository;
        public sale_customerService(IRepositoryAsync<sale_customer> repository) : base(repository)
        {
            _repository = repository;
        }
        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
    }
}
