using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Repositories.Admin;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace HMS.Service.Services.Admin
{
    public interface Isys_drop_down_mfService : IService<sys_drop_down_mf>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
    }
    public class sys_drop_down_mfService : Service<sys_drop_down_mf>, Isys_drop_down_mfService
    {
        private readonly IRepositoryAsync<sys_drop_down_mf> _repository;
        public sys_drop_down_mfService(IRepositoryAsync<sys_drop_down_mf> repository) : base(repository)
        {
            _repository = repository;
        }
        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
    }
}
