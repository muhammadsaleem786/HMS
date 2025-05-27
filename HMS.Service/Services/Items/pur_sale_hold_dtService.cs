using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using HMS.Repository.Repositories.Items;
using System.Collections.Generic;

namespace HMS.Service.Services.Items
{
    

    public interface Ipur_sale_hold_dtService : IService<pur_sale_hold_dt>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);

    }
    public class pur_sale_hold_dtService : Service<pur_sale_hold_dt>, Ipur_sale_hold_dtService
    {
        private readonly IRepositoryAsync<pur_sale_hold_dt> _repository;
        public pur_sale_hold_dtService(IRepositoryAsync<pur_sale_hold_dt> repository) : base(repository)
        {
            _repository = repository;
        }
        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
       
    }
}
