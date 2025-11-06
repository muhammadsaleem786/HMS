using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using HMS.Repository.Repositories.Admin;

namespace HMS.Service.Services.Admin
{
    public interface Iadm_user_shareService : IService<adm_user_share>
    {
        PaginationResult PaginationWithParm(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false);

    }

    public class adm_user_shareService : Service<adm_user_share>, Iadm_user_shareService
    {
        private readonly IRepositoryAsync<adm_user_share> _repository;
        public adm_user_shareService(IRepositoryAsync<adm_user_share> repository) : base(repository)
        {
            _repository = repository;
        }
        public PaginationResult PaginationWithParm(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
               return _repository.PaginationWithParm(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, FilterID, IgnorePaging);
        }
    }
}
