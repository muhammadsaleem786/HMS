using System;
using System.Collections.Generic;
using Repository.Pattern.Ef6;
namespace HMS.Entities.Models
{
    public partial class adm_setting : Entity
    {
        public int ID { get; set; }
        public string SettingIdOrName { get; set; }
        public string SettingIdOrNameValue { get; set; }
        public string SettingIdOrNameDepAllowValue { get; set; }
        public string SettingIdOrNameDepDedValue { get; set; }
    }
}
