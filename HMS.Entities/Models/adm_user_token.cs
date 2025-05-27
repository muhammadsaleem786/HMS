using System;
using System.Collections.Generic;
using Repository.Pattern.Ef6;
namespace HMS.Entities.Models
{
    public partial class adm_user_token : Entity
    {
        public decimal ID { get; set; }
        public decimal UserID { get; set; }
        public string TokenKey { get; set; }
        public System.DateTime ExpiryDate { get; set; }
        public bool IsExpired { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
    }
}
