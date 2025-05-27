using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using HMS.Repository.Repositories.Employee;

namespace HMS.Service.Services.Employee
{
    public interface Isys_holidaysService : IService<sys_holidays>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
    }

    public class sys_holidaysService : Service<sys_holidays>, Isys_holidaysService
    {
        private readonly IRepositoryAsync<sys_holidays> _repository;
        public sys_holidaysService(IRepositoryAsync<sys_holidays> repository) : base(repository)
        {
            _repository = repository;
        }

        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
    }
}
