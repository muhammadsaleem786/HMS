using System;

namespace HMS.Entities.CustomModel
{
    public class ScreenModel 
    {
        public int ID { get; set; }
        public int DropDownID { get; set; }
        public string Value { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> DependedDropDownID { get; set; }
        public Nullable<int> DependedDropDownValueID { get; set; }
        public string ModuleName { get; set; }
    }
}
