using System;

namespace HMS.Entities.CustomModel
{
    public class TableColumn
    {
        public string table_name { get; set; }
        public string column_name { get; set; }
        public string data_type { get; set; }
        public string is_nullable { get; set; }
        public string primary { get; set; }
        public Int32 IsIdentity { get; set; }

    }
}
