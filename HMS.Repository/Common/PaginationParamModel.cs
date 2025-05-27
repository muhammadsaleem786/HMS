using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Repository.Common
{
    public class PaginationParamModel
    {
        public int TakeRecord { get; set; }
        public int SkipRecord { get; set; }
        public string VisibleColumnInfo { get; set; }
        public List<string> VisibleColumnInfoList { get; set; }
        public bool IsOrderAsc { get; set; }
        public string OrderBy { get; set; }
        public string SearchText { get; set; }
    }
}
