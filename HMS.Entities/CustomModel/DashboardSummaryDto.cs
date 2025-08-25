using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class DashboardSummaryDto
    {
        public int Child { get; set; }
        public int NewPatients { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int Other { get; set; }
        public int PatientVisits { get; set; }
        public int TotalAppointments { get; set; }
        public int MissedAppointments { get; set; }
        public decimal PaymentCollection { get; set; }
        public decimal OutstandingAmount { get; set; }
        public decimal ProfessionalFees { get; set; }
        public decimal Expenses { get; set; }
        public decimal ProExpenses { get; set; }
        public decimal PurPayment { get; set; }
        public decimal OutStandingIncome { get; set; }
        public decimal DueIncome { get; set; }

        public decimal Income { get; set; } // always 0 per SP
        public decimal PharmacyIncome { get; set; }
        public decimal IPDIncome { get; set; }
        public decimal PathologyIncome { get; set; }
        public decimal GeneralIncome { get; set; }
        public List<AppointmentDto> TodaysAppointments { get; set; }
        public List<BirthdayDto> TodaysBirthdays { get; set; } 
        public List<FollowUpDto> TodaysFollowUps { get; set; } 
        public List<IncomeAndExpense> IncomeAndExpense { get; set; }
    }
    public class IncomeAndExpense
    {
        public int MonthNumber { get; set; }
        public string MonthName { get; set; } = "";
        public decimal ExAmount { get; set; }
        public decimal Income { get; set; }
    }
    public class AppointmentDto
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string PatientName { get; set; }
        public string ScheduledStatus { get; set; }
        public string DoctorName { get; set; }
    }

    public class BirthdayDto
    {
        public decimal Id {  get; set; }
        public string PatientName { get; set; }
        public DateTime? DOB { get; set; }
        public string Image { get; set; }
        public int Gender { get; set; }
    }

    public class FollowUpDto
    {
        public string AppointmentTime { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public decimal Id { get; set; }   // adjust if your PatientId is int
        public bool IsCreateAppointment { get; set; }
        public string EndTime { get; set; }
        public string Mobile { get; set; }
        public string FollowUpNotes { get; set; }
    }
}
