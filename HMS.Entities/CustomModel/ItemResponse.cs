using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class ItemResponse
    {
        public decimal ID {  get; set; }
        public string Name { get; set; }        
        public string Code { get; set; }
        public decimal? Stock { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }
        public decimal SalePrice { get; set; }
        public decimal CostPrice { get; set; }
        public string SaveStatus { get; set; }
        public string BatchSarialNumber {  get; set; }
        public string TypeValue { get;set; }
        public string Group {  get; set; }
    }
}
