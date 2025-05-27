using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Repositories.Admin;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace HMS.Service.Services.Admin
{
    public interface IcontactService : IService<contact>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
    }

    public class contactService : Service<contact>, IcontactService
    {
        private readonly IRepositoryAsync<contact> _repository;
        public contactService(IRepositoryAsync<contact> repository) : base(repository)
        {
            _repository = repository;
        }

        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
    }
}
