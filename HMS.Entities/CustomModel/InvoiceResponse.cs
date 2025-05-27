using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class InvoiceResponse
    {
        public decimal ID { get; set; }
        public string Invoiceno { get; set; }
        public string date { get; set; }
        public string duedate { get; set; }
        public string name { get; set; }
        public decimal? amount { get; set; }
        public decimal ? DueBalance {  get; set; }
        public string status { get; set; }
        public string CompanyName { get; set; }
        public bool isSelected { get; set; }
        public string SaveStatus { get; set; }
    }
}
