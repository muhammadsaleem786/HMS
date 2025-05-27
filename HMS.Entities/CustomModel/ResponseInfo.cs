using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class ResponseInfo
    {
        public ResponseInfo() { IsSuccess = true; }
        public bool IsSuccess { get; set; }
        public object ResultSet { get; set; }
        public string FilePath { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public object data { get; set; }
    }
}
