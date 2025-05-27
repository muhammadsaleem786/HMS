using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
   public class EmailModel
    {
        public string From { get; set; }
        public string SendTo { get; set; }

        public string CC { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
        public string image { get; set; }
        public string PdfBody { get; set; }
    }
}
