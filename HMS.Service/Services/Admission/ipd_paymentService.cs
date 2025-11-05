using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using HMS.Repository.Repositories.Items;
using System.Collections.Generic;
using HMS.Repository.Repositories.Admission;

namespace HMS.Service.Services.Admission
{


    public interface Iipd_paymentService : IService<ipd_payment>
    {
        PaginationResult PaginationWithParm(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText,string FilterID, bool IgnorePaging = false);
   
    }
    public class ipd_paymentService : Service<ipd_payment>, Iipd_paymentService
    {
        private readonly IRepositoryAsync<ipd_payment> _repository;
        public ipd_paymentService(IRepositoryAsync<ipd_payment> repository) : base(repository)
        {
            _repository = repository;
        }
        public PaginationResult PaginationWithParm(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText,string FilterID, bool IgnorePaging = false)
        {
            return _repository.PaginationWithParm(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, FilterID, IgnorePaging);
        }

    }
}
