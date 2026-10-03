using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using HMS.Repository.Repositories.Items;
using System.Collections.Generic;

namespace HMS.Service.Services.Items
{


    public interface Isale_paymentService : IService<sale_payment>
    {
        PaginationResult PaginationWithParm(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText,string FilterID, bool IgnorePaging = false);
   
    }
    public class sale_paymentService : Service<sale_payment>, Isale_paymentService
    {
        private readonly IRepositoryAsync<sale_payment> _repository;
        public sale_paymentService(IRepositoryAsync<sale_payment> repository) : base(repository)
        {
            _repository = repository;
        }
        public PaginationResult PaginationWithParm(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText,string FilterID, bool IgnorePaging = false)
        {
            return _repository.PaginationWithParm(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, FilterID, IgnorePaging);
        }

    }
}
