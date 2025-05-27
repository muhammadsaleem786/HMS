using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;

namespace HMS.Entities.Models
{
    public partial class sys_drop_down_mf : Entity
    {
        public sys_drop_down_mf()
        {
            this.sys_drop_down_value = new List<sys_drop_down_value>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<sys_drop_down_value> sys_drop_down_value { get; set; }
    }
}
