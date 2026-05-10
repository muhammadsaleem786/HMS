using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class DeepSeekRequest
    {
        public string ClinicalHistory { get; set; }
        public string Complaint { get; set; }
        public string Vital { get; set; }
    }
    public class DeepSeekResponse
    {
        public string Response { get; set; }
    }
}
