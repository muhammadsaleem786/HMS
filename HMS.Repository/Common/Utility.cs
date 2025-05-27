using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Repository.Common
{
    public class Utility
    {
        public static PaginationParamModel SetPaginationFilter(int CurrentPageNo, int RecordPerPage, string VisibleColumnInfo, string SortName, string SortOrder, string SearchText)
        {
            PaginationParamModel Obj = new PaginationParamModel();

            Obj.IsOrderAsc = false;
            Obj.TakeRecord = RecordPerPage;
            Obj.SkipRecord = (CurrentPageNo - 1) * RecordPerPage;
            Obj.VisibleColumnInfo = VisibleColumnInfo;
            Obj.VisibleColumnInfoList = VisibleColumnInfo.Split(',').Select(s => s.Split('#')[0].Trim()).ToList();
            Obj.SearchText = SearchText;
            Obj.OrderBy = SortName;
            Obj.IsOrderAsc = (SortOrder == "Asc" ? true : false);

            return Obj;
        }
    }
}
