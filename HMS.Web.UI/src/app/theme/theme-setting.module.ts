//theme-setting.module.ts
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { AuthGuard } from '../CommonService/AuthGuard';
import { CommonUtilsModule } from './../common/common-utils.module';
import { ReportComponentForm } from './Module/Reports/ReportComponentForm';
//setting module
import { ObservationComponentList } from '../theme/Module/setup/Setting/Observation/ObservationComponentList';
import { ComplaintComponentList } from '../theme/Module/setup/Setting/Complaint/ComplaintComponentList';
import { InvestigationComponentList } from '../theme/Module/setup/Setting/Investigation/InvestigationComponentList';
import { DiagnosComponentList } from '../theme/Module/setup/Setting/Diagnos/DiagnosComponentList';
import { MedicineInstructionComponentList } from '../theme/Module/setup/Setting/MedicineInstruction/MedicineInstructionComponentList';
import { CompanyComponentForm } from '../theme/Module/setup/Setting/Company/CompanyComponentForm';
import { ApplicationUserComponentForm } from '../theme/Module/setup/Setting/ApplicationUser/ApplicationUserComponentForm';
import { ApplicationUserComponentList } from '../theme/Module/setup/Setting/ApplicationUser/ApplicationUserComponentList';
import { UserRoleComponentList } from '../theme/Module/setup/Setting/userrole/UserRoleComponentList';
import { UserRoleComponentForm } from '../theme/Module/setup/Setting/userrole/UserRoleComponentForm';
import { ProfileUserComponentForm } from '../theme/Module/setup/Setting/ApplicationUser/ProfileUserComponentForm';
import { MedicineComponentForm } from '../theme/Module/setup/Setting/Medicine/MedicineComponentForm';
import { MedicineComponentList } from '../theme/Module/setup/Setting/Medicine/MedicineComponentList';
import { DropDownComponentList } from '../theme/Module/setup/Setting/DropDown/DropDownComponentList';
import { ServiceComponentList } from '../theme/Module/setup/Setting/Service/ServiceComponentList';
import { ServiceComponentForm } from '../theme/Module/setup/Setting/Service/ServiceComponentForm';
import { UserShareComponentForm } from '../theme/Module/setup/Setting/UserShare/UserShareComponentForm';
import { UserShareComponentList } from '../theme/Module/setup/Setting/UserShare/UserShareComponentList';
import { ReminderComponentList } from '../theme/Module/setup/Setting/Reminder/ReminderComponentList';
import { ReminderComponentForm } from '../theme/Module/setup/Setting/Reminder/ReminderComponentForm';
import { TemplateComponentList } from '../theme/Module/setup/Setting/Template/TemplateComponentList';
import { SessionComponentList } from '../theme/Module/setup/Setting/UserSession/SessionComponentList';
//Item module
import { ItemComponentList } from '../theme/Module/setup/Items/AddItems/ItemComponentList';
import { ItemComponentForm } from '../theme/Module/setup/Items/AddItems/ItemComponentForm';
import { ImportItemComponent } from '../theme/Module/setup/Items/AddItems/ImportItemComponent';
import { RestockComponentList } from '../theme/Module/setup/Items/AddItems/RestockComponentList';
import { ExpireComponentList } from '../theme/Module/setup/Items/AddItems/ExpireComponentList';
import { VendorComponentList } from '../theme/Module/setup/Items/Vendor/VendorComponentList';
import { VendorComponentForm } from '../theme/Module/setup/Items/Vendor/VendorComponentForm';
import { InvoiceComponentList } from '../theme/Module/setup/Items/Invoice/InvoiceComponentList';
import { InvoiceComponentForm } from '../theme/Module/setup/Items/Invoice/InvoiceComponentForm';
import { ItemPaymentComponentForm } from '../theme/Module/setup/Items/Payment/ItemPaymentComponentForm';
import { ItemPaymentComponentList } from '../theme/Module/setup/Items/Payment/ItemPaymentComponentList';
import { IncomeComponentForm } from '../theme/Module/setup/Income/IncomeComponentForm';
import { IncomeComponentList } from '../theme/Module/setup/Income/IncomeComponentList';
import { ExpenseComponentForm } from '../theme/Module/setup/Expense/ExpenseComponentForm';
import { ExpenseComponentList } from '../theme/Module/setup/Expense/ExpenseComponentList';

//end
//admin module
import { PaymentComponentForm } from '../theme/Module/setup/Admin/Payment/PaymentComponentForm';
import { PaymentComponentList } from '../theme/Module/setup/Admin/Payment/PaymentComponentList';
import { SubscriberComponentList } from '../theme/Module/setup/Admin/Subscriber/SubscriberComponentList';
import { RegisterComponentForm } from '../theme/Module/setup/Admin/Payment/RegisterComponentForm';
import { IntegrationComponent } from '../theme/Module/setup/Integration/IntegrationComponent';
//end
//employee module
import { EmployeeComponentList } from '../theme/Module/setup/Employee/AddEmp/EmployeeComponentList';
import { EmployeeComponent } from '../theme/Module/setup/Employee/AddEmp/EmployeeComponent';
import { LoanComponentList } from '../theme/Module/setup/Employee/Loans/LoanComponentList';
import { LoanComponentForm } from '../theme/Module/setup/Employee/Loans/LoanComponentForm';
import { DepartmentComponentList } from '../theme/Module/setup/Employee/Department/DepartmentComponentList';
import { DepartmentComponentForm } from '../theme/Module/setup/Employee/Department/DepartmentComponentForm';
import { designationComponentList } from '../theme/Module/setup/Employee/Designation/designationComponentList';
import { designationComponentForm } from '../theme/Module/setup/Employee/Designation/designationComponentForm';
import { AttendanceComponentList } from '../theme/Module/setup/Employee/Attendance/AttendanceComponentList';
import { AttendanceComponentForm } from '../theme/Module/setup/Employee/Attendance/AttendanceComponentForm';
import { LeaveApplicationComponentList } from '../theme/Module/setup/Employee/LeaveApplication/LeaveApplicationComponentList';
import { LeaveApplicationComponentForm } from '../theme/Module/setup/Employee/LeaveApplication/LeaveApplicationComponentForm';
import { TimeAttendanceComponentList } from '../theme/Module/setup/Employee/TimeAttendance/TimeAttendanceComponentList';
import { TimeAttendanceComponentForm } from '../theme/Module/setup/Employee/TimeAttendance/TimeAttendanceComponentForm';
import { PayrollComponentList } from '../theme/Module/setup/Employee/EmployeePayroll/EmployeePayrollComponentList';
import { PayrollComponentForm } from '../theme/Module/setup/Employee/EmployeePayroll/EmployeePayrollComponentForm';
import { ZKTComponentList } from '../theme/Module/setup/Employee/ZKT/ZKTComponentList';
import { AllowanceComponentList } from '../theme/Module/setup/Employee/Allowance/AllowanceComponentList';
import { AllowanceComponentForm } from '../theme/Module/setup/Employee/Allowance/AllowanceComponentForm';
import { DeductionContributionComponentList } from '../theme/Module/setup/Employee/DeductionContribution/DeductionContributionComponentList';
import { DeductionContributionComponentForm } from '../theme/Module/setup/Employee/DeductionContribution/DeductionContributionComponentForm';
import { holidayComponentList } from '../theme/Module/setup/Employee/Holidays/holidayComponentList';
import { holidayComponentForm } from '../theme/Module/setup/Employee/Holidays/holidayComponentForm';
import { LocationsComponentList } from '../theme/Module/setup/Employee/Locations/LocationsComponentList';
import { LocationsComponentForm } from '../theme/Module/setup/Employee/Locations/LocationsComponentForm';
import { PayScheduleComponentList } from '../theme/Module/setup/Employee/Payschedule/PayScheduleComponentList';
import { PayScheduleComponentForm } from '../theme/Module/setup/Employee/Payschedule/PayScheduleComponentForm';
import { VacationSickLeaveComponentList } from '../theme/Module/setup/Employee/VacationSickLeave/VacationSickLeaveComponentList';
import { VacationSickLeaveComponentForm } from '../theme/Module/setup/Employee/VacationSickLeave/VacationSickLeaveComponentForm';
const routes: Routes = [
    { path: 'report', component: ReportComponentForm, canActivate: [AuthGuard] },    
    //setting module
    { path: 'DropDown', component: DropDownComponentList, canActivate: [AuthGuard] },
    { path: 'reminder', component: ReminderComponentList, canActivate: [AuthGuard] },
    { path: 'template', component: TemplateComponentList, canActivate: [AuthGuard] },
    { path: 'reminder/savereminder', component: ReminderComponentForm, canActivate: [AuthGuard] },
    { path: 'reminder/savereminder:id', component: ReminderComponentForm, canActivate: [AuthGuard] },
    { path: 'session', component: SessionComponentList, canActivate: [AuthGuard] },
    { path: 'share', component: UserShareComponentList, canActivate: [AuthGuard] },
    { path: 'share/saveshare', component: UserShareComponentForm, canActivate: [AuthGuard] },
    { path: 'share/saveshare:id', component: UserShareComponentForm, canActivate: [AuthGuard] },
    { path: 'Service', component: ServiceComponentList, canActivate: [AuthGuard] },
    { path: 'Service/saveservice', component: ServiceComponentForm, canActivate: [AuthGuard] },
    { path: 'Service/saveservice:id', component: ServiceComponentForm, canActivate: [AuthGuard] },
    { path: 'Observation', component: ObservationComponentList, canActivate: [AuthGuard] },
    { path: 'Complaint', component: ComplaintComponentList, canActivate: [AuthGuard] },
    { path: 'Invest', component: InvestigationComponentList, canActivate: [AuthGuard] },
    { path: 'Diagnos', component: DiagnosComponentList, canActivate: [AuthGuard] },
    { path: 'MedicineInst', component: MedicineInstructionComponentList, canActivate: [AuthGuard] },
    { path: 'aplicationUser', component: ApplicationUserComponentList, canActivate: [AuthGuard] },
    { path: 'aplicationUser/saveuser:id', component: ApplicationUserComponentForm, canActivate: [AuthGuard] },
    { path: 'aplicationUser/saveuser', component: ApplicationUserComponentForm, canActivate: [AuthGuard] },
    { path: 'userrole', component: UserRoleComponentList, canActivate: [AuthGuard] },
    { path: 'userrole/saverole:id', component: UserRoleComponentForm, canActivate: [AuthGuard] },
    { path: 'userrole/saverole', component: UserRoleComponentForm, canActivate: [AuthGuard] },
    { path: 'Profile/saveuser', component: ProfileUserComponentForm, canActivate: [AuthGuard] },
    { path: 'Profile/saveuser:id', component: ProfileUserComponentForm, canActivate: [AuthGuard] },
    { path: 'Medicine', component: MedicineComponentList, canActivate: [AuthGuard] },
    { path: 'Medicine/saveMedicine:id', component: MedicineComponentForm, canActivate: [AuthGuard] },
    { path: 'Medicine/saveMedicine', component: MedicineComponentForm, canActivate: [AuthGuard] },
    { path: 'Appoint/company', component: CompanyComponentForm, canActivate: [AuthGuard] },
    //end
    //Item module
    { path: 'Item/saveitem:id', component: ItemComponentForm },
    { path: 'Item/saveitem', component: ItemComponentForm },
    { path: 'ImportItem', component: ImportItemComponent },
    { path: 'Item', component: ItemComponentList },
    { path: 'Restock', component: RestockComponentList },
    { path: 'Expire', component: ExpireComponentList },
    { path: 'Vendor/savevendor:id', component: VendorComponentForm },
    { path: 'Vendor/savevendor', component: VendorComponentForm },
    { path: 'Vendor', component: VendorComponentList },
    { path: 'Invoice/saveinvoice:id', component: InvoiceComponentForm },
    { path: 'Invoice/saveinvoice', component: InvoiceComponentForm },
    { path: 'Invoice', component: InvoiceComponentList },    
    { path: 'Payment/savepayment:id', component: ItemPaymentComponentForm },
    { path: 'Payment/savepayment', component: ItemPaymentComponentForm },
    { path: 'Payment', component: PaymentComponentList },
    { path: 'Income', component: IncomeComponentList, canActivate: [AuthGuard] },
    { path: 'Income:id', component: IncomeComponentList, canActivate: [AuthGuard] },
    { path: 'Income/saveIncome:id', component: IncomeComponentForm, canActivate: [AuthGuard] },
    { path: 'Income/saveIncome', component: IncomeComponentForm, canActivate: [AuthGuard] },
    { path: 'Expense', component: ExpenseComponentList, canActivate: [AuthGuard] },
    { path: 'Expense/saveExpense:id', component: ExpenseComponentForm, canActivate: [AuthGuard] },
    { path: 'Expense/saveExpense', component: ExpenseComponentForm, canActivate: [AuthGuard] },

    //end
    //admin module
    { path: 'Admin', component: PaymentComponentList, canActivate: [AuthGuard] },
    { path: 'Admin/setting:id', component: PaymentComponentForm, canActivate: [AuthGuard] },
    { path: 'Admin/setting', component: PaymentComponentForm, canActivate: [AuthGuard] },
    { path: 'Admin/Subscriber', component: SubscriberComponentList, canActivate: [AuthGuard] },
    { path: 'Admin/register', component: RegisterComponentForm, canActivate: [AuthGuard] },
    {
        path: 'integration', component: IntegrationComponent, canActivate: [AuthGuard]
    },
    //end
    //employee module
    { path: 'Attendance', component: AttendanceComponentList, canActivate: [AuthGuard] },
    { path: 'Attendance/saveAttend', component: AttendanceComponentForm, canActivate: [AuthGuard] },
    { path: 'Attendance/saveAttend:id', component: AttendanceComponentForm, canActivate: [AuthGuard] },
    { path: 'Leaveentry', component: LeaveApplicationComponentList, canActivate: [AuthGuard] },
    { path: 'Leaveentry/saveleave', component: LeaveApplicationComponentForm, canActivate: [AuthGuard] },
    { path: 'Leaveentry/saveleave:id', component: LeaveApplicationComponentForm, canActivate: [AuthGuard] },
    { path: 'Timeattendance', component: TimeAttendanceComponentForm, canActivate: [AuthGuard] },
    { path: 'Timelist', component: TimeAttendanceComponentList, canActivate: [AuthGuard] },
    { path: 'Loan', component: LoanComponentList, canActivate: [AuthGuard] },
    { path: 'Employee/saveemployee:id', component: EmployeeComponent },
    { path: 'Employee/saveemployee', component: EmployeeComponent },
    { path: 'Employee', component: EmployeeComponentList },
    { path: 'Department', component: DepartmentComponentList, canActivate: [AuthGuard] },
    { path: 'Designation', component: designationComponentList, canActivate: [AuthGuard] },
    { path: 'Payroll', component: PayrollComponentList, canActivate: [AuthGuard] },
    { path: 'zktlist', component: ZKTComponentList, canActivate: [AuthGuard] },
    { path: 'Allowance', component: AllowanceComponentList, canActivate: [AuthGuard] },
    { path: 'Deduction', component: DeductionContributionComponentList, canActivate: [AuthGuard] },
    { path: 'holiday', component: holidayComponentList, canActivate: [AuthGuard] },
    { path: 'holiday/saveholiday', component: holidayComponentForm, canActivate: [AuthGuard] },
    { path: 'holiday/saveholiday:id', component: holidayComponentForm, canActivate: [AuthGuard] },
    { path: 'locations', component: LocationsComponentList },
    { path: 'Schedule', component: PayScheduleComponentList, canActivate: [AuthGuard] },
    { path: 'vacationsickleave', component: VacationSickLeaveComponentList },
    //end
];
@NgModule({
    declarations: [
        ReportComponentForm,
        //employee module
        DepartmentComponentList, PayrollComponentList, PayrollComponentForm,
        AttendanceComponentList, AttendanceComponentForm,
        DepartmentComponentForm, designationComponentList,
        designationComponentForm, LoanComponentList, LoanComponentForm,
        TimeAttendanceComponentList, TimeAttendanceComponentForm, LeaveApplicationComponentForm,
        EmployeeComponent, EmployeeComponentList, ZKTComponentList,
        VacationSickLeaveComponentList, VacationSickLeaveComponentForm, AllowanceComponentList,
        AllowanceComponentForm, DeductionContributionComponentList, DeductionContributionComponentForm,
        PayScheduleComponentList, PayScheduleComponentForm, LeaveApplicationComponentList,
        LocationsComponentList, LocationsComponentForm, holidayComponentList, holidayComponentForm,
        //end
        //setting module
        MedicineInstructionComponentList, ObservationComponentList, ComplaintComponentList, InvestigationComponentList, DiagnosComponentList, DropDownComponentList,
        ApplicationUserComponentList, ApplicationUserComponentForm, MedicineComponentList, MedicineComponentForm, CompanyComponentForm, UserRoleComponentList
        , ProfileUserComponentForm, ServiceComponentList, ServiceComponentForm, UserShareComponentList, UserShareComponentForm,
        UserRoleComponentForm, SessionComponentList, ReminderComponentList, TemplateComponentList, ReminderComponentForm,
        //end
        //Item Module
        IncomeComponentForm, IncomeComponentList, ItemComponentList,ItemComponentForm, VendorComponentList, VendorComponentForm,
        InvoiceComponentList, InvoiceComponentForm, ItemPaymentComponentForm
        , ExpenseComponentList, ExpenseComponentForm, ItemPaymentComponentList, RestockComponentList, ExpireComponentList, ImportItemComponent,
        //end
        //admin module
        PaymentComponentForm, PaymentComponentList, SubscriberComponentList, RegisterComponentForm,
        IntegrationComponent
        //end
       

    ],
    imports: [
        CommonUtilsModule,
        RouterModule.forChild(routes)
    ],
    providers: [CookieService, AuthGuard]
})
export class ThemeSettingModule { }
