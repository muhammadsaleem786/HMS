using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{

    public partial class adm_user_session : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public DateTime ShiftStartTime { get; set; }
        public DateTime? ShiftEndTime { get; set; }
        public decimal StartingCash { get; set; } = 0;
        public int TransactionsCount { get; set; } = 0;
        public decimal TotalCollection { get; set; } = 0;
        public decimal Refunds { get; set; } = 0;
        public decimal ClosingBalance { get; set; } = 0;

        [NotMapped]
        public string StartTime { get; set; }
        [NotMapped]
        public decimal ExpensesAmount { get; set; } = 0; 
        [NotMapped]
        public decimal PurchaseExpensesAmount { get; set; } = 0;
        [NotMapped]
        public decimal OtherIncome { get; set; } = 0;
        [NotMapped]
        public decimal IPDBalance { get; set; } = 0;
        [NotMapped]
        public decimal OPDBalance { get; set; } = 0;
        [NotMapped]
        public decimal PharmacyBalance { get; set; } = 0;

        public string OpeningComments {  get; set; }
        public string ClosingComments {  get; set; }
        public decimal CreatedBy { get; set; }
        public decimal UserId {  get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }        
    }
}
