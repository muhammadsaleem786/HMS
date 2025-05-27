using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class contact : Entity
    {
        public decimal ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Speciality { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
