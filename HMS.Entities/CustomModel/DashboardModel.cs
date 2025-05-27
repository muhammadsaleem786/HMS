using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.DashboardModel
{
    public partial class DashboardModel
    {
        public int NewPatients { get; set; }
        public int Appointment { get; set; }
        public int PatientVisits { get; set; }
        public int MissedAppointment { get; set; }
        public int ProfessionalFees { get; set; }
        public int PaymentCollection { get; set; }
        public int OutstandingAmount { get; set; }
        public int Expenses { get; set; }
    }
}
