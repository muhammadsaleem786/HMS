using System;
using System.Collections.Generic;
using Repository.Pattern.Ef6;
namespace HMS.Entities.Models
{
    public partial class adm_multilingual_mf : Entity
    {
        public adm_multilingual_mf()
        {
            this.adm_multilingual_dt = new List<adm_multilingual_dt>();
        }

        public decimal MultilingualId { get; set; }
        public string MultilingualName { get; set; }
        public virtual ICollection<adm_multilingual_dt> adm_multilingual_dt { get; set; }
    }
}
