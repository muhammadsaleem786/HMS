using HMS.Entities.CustomModel;
using System.Threading.Tasks;

namespace HMS.Web.API.Interface
{
    interface IERPAPIInterface<T>
    {
        ResponseInfo Load();
        ResponseInfo GetList();
        ResponseInfo GetById(string Id);
        ResponseInfo GetById(string Id, int NextPreviousIndex);
        Task<ResponseInfo> Save(T Model);
        Task<ResponseInfo> Update(T Model);
        Task<ResponseInfo> Delete(string Id);
        PaginationResult Pagination(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText, bool IgnorePaging = false);
        PaginationResult PaginationWithParm(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText,string FilterID, bool IgnorePaging = false);
        ResponseInfo ExportData(int ExportType, int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText);
    }
}
