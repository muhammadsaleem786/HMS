using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class pr_time_entryModel
    {
        public decimal ID { get; set; }
        public string Date { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string Status { get; set; }
        public string Employee { get; set; }
    }
}
