using System;
using System.Collections.Generic;
using Repository.Pattern.Ef6;
namespace HMS.Entities.Models
{
    public partial class adm_multilingual_dt : Entity
    {
        public decimal ID { get; set; }
        public decimal MultilingualId { get; set; }
        public string Module { get; set; }
        public string Form { get; set; }
        public string Keyword { get; set; }
        public string Value { get; set; }
        public virtual adm_multilingual_mf adm_multilingual_mf { get; set; }
    }
}
