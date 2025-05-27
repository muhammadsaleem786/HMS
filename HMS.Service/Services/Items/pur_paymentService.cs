using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using HMS.Repository.Repositories.Items;
using System.Collections.Generic;

namespace HMS.Service.Services.Items
{


    public interface Ipur_paymentService : IService<pur_payment>
    {
        PaginationResult PaginationWithParm(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText,string FilterID, bool IgnorePaging = false);
   
    }
    public class pur_paymentService : Service<pur_payment>, Ipur_paymentService
    {
        private readonly IRepositoryAsync<pur_payment> _repository;
        public pur_paymentService(IRepositoryAsync<pur_payment> repository) : base(repository)
        {
            _repository = repository;
        }
        public PaginationResult PaginationWithParm(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText,string FilterID, bool IgnorePaging = false)
        {
            return _repository.PaginationWithParm(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, FilterID, IgnorePaging);
        }

    }
}
