using HMS.Entities.CustomModel;
using HMS.Entities.Models;
using HMS.Repository.Repositories.Appointment;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service.Services.Appointment
{
 
    public interface Iemr_expenseService : IService<emr_expense>
    {
        PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
        PaginationResult PaginationWithParm(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false);

    }

    public class emr_expenseService : Service<emr_expense>, Iemr_expenseService
    {
        private readonly IRepositoryAsync<emr_expense> _repository;
        public emr_expenseService(IRepositoryAsync<emr_expense> repository) : base(repository)
        {
            _repository = repository;
        }

        public PaginationResult Pagination(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false)
        {
            return _repository.Pagination(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, IgnorePaging);
        }
        public PaginationResult PaginationWithParm(decimal CompanyID, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, string FilterID, bool IgnorePaging = false)
        {
            return _repository.PaginationWithParm(CompanyID, CurrentPageNo, RecordPerPage, VisibleColumnInfo, SortName, SortOrder, SearchText, FilterID, IgnorePaging);
        }
    }
}
