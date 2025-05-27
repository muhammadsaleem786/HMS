using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using HMS.Repository.Repositories.Items;
using System.Collections.Generic;

namespace HMS.Service.Services.Items
{
    

    public interface Ipur_invoice_mfService : IService<pur_invoice_mf>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);

    }
    public class pur_invoice_mfService : Service<pur_invoice_mf>, Ipur_invoice_mfService
    {
        private readonly IRepositoryAsync<pur_invoice_mf> _repository;
        public pur_invoice_mfService(IRepositoryAsync<pur_invoice_mf> repository) : base(repository)
        {
            _repository = repository;
        }
        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
       
    }
}
