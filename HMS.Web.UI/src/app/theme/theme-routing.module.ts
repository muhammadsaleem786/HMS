import { NgModule } from '@angular/core';
import { ThemeComponent } from './theme.component';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../CommonService/AuthGuard';
import { LayoutModule } from '../theme/layouts/layout.module';
import { AppComponent } from '../app.component';
import { CookieService } from 'ngx-cookie-service';
import { DashboardMenuService } from '../CommonService/DashboardMenuService';
import { ChangepasswordComponent } from './Module/changepassword/changepassword.component';
import { SetupDashboardComponent } from './Module/setup/SetupDashboard/SetupDashboardComponent';
import { ReportComponentForm } from '../theme/Module/Reports/ReportComponentForm';
import { DashboardForm } from '../theme/Module/Dashboard/DashboardForm';
import { PageListComponent } from '../CommonComponent/PageList/PageListComponent';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { MatStepperModule } from '@angular/material';
import { MatInputModule, MatAutocompleteModule } from '@angular/material';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MyDatePickerModule } from 'mydatepicker';
import { PatientComponentForm } from '../theme/Module/setup/Patient/PatientComponentForm';
import { PatientComponentList } from '../theme/Module/setup/Patient/PatientComponentList';
import { ExpenseComponentForm } from '../theme/Module/setup/Expense/ExpenseComponentForm';
import { ExpenseComponentList } from '../theme/Module/setup/Expense/ExpenseComponentList';
import { PrescriptionComponentForm } from '../theme/Module/setup/Prescription/PrescriptionComponentForm';
import { PrescriptionComponentList } from '../theme/Module/setup/Prescription/PrescriptionComponentList';
import { BillingComponentForm } from '../theme/Module/setup/Billing/BillingComponentForm';
import { BillingComponentList } from '../theme/Module/setup/Billing/BillingComponentList';
import { IncomeComponentForm } from '../theme/Module/setup/Income/IncomeComponentForm';
import { IncomeComponentList } from '../theme/Module/setup/Income/IncomeComponentList';
import { SummeryComponentList } from '../theme/Module/setup/Patient/SummeryComponentList';
import { VitalComponentList } from '../theme/Module/setup/Patient/VitalComponentList';
import { BillSummeryComponentForm } from '../theme/Module/setup/Patient/BillSummeryComponentForm';
import { AppointSummeryComponentList } from '../theme/Module/setup/Patient/AppointSummeryComponentList';
import { DocumentSummeryComponentList } from '../theme/Module/setup/Patient/DocumentSummeryComponentList';
import { CommonModule } from '@angular/common';
import { CommonUtilsModule } from '../common/common-utils.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MultiselectDropdownModule } from 'angular-2-dropdown-multiselect';
import { NgxEchartsModule } from 'ngx-echarts';
//admission module
import { AdmitComponentList } from '../theme/Module/setup/Admission/AdmitComponentList';
import { AdmitComponentForm } from '../theme/Module/setup/Admission/AdmitComponentForm';
import { AdmitSummeryComponentList } from '../theme/Module/setup/Admission/AdmitSummeryComponentList';
import { AdmitAppointSummeryComponentList } from '../theme/Module/setup/Admission/AdmitAppointSummeryComponentList';
import { AdmitDocumentSummeryComponentList } from '../theme/Module/setup/Admission/AdmitDocumentSummeryComponentList';
import { AdmitVitalComponentList } from '../theme/Module/setup/Admission/AdmitVitalComponentList';
import { AdmitLabsComponentList } from '../theme/Module/setup/Admission/AdmitLabsComponentList';
import { AdmitMedicationList } from '../theme/Module/setup/Admission/AdmitMedicationList';
import { AdmitImagingList } from '../theme/Module/setup/Admission/AdmitImagingList';
import { AdmitNoteComponentList } from '../theme/Module/setup/Admission/AdmitNoteComponentList';
import { AdmitProcedureComponentList } from '../theme/Module/setup/Admission/AdmitProcedureComponentList';
import { AdmitChargeComponentList } from '../theme/Module/setup/Admission/AdmitChargeComponentList';
import { AdmitDetailComponentList } from '../theme/Module/setup/Admission/AdmitDetailComponentList';
import { DischargeReportComponent } from '../theme/Module/setup/Admission/DischargeReportComponent';
import { AdmitPatientComponentList } from '../theme/Module/setup/Admission/AdmitPatientComponentList'
//end
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

import { ReminderComponentList } from '../theme/Module/setup/Setting/Reminder/ReminderComponentList';
import { ReminderComponentForm } from '../theme/Module/setup/Setting/Reminder/ReminderComponentForm';

import { FilterPipe } from '../angularfilter/filter.pipe'
//end
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

import { SaleComponentList } from '../theme/Module/setup/Items/Sale/SaleComponentList';
import { SaleHoldComponentList } from '../theme/Module/setup/Items/Sale/SaleHoldComponentList';
import { SaleComponentForm } from '../theme/Module/setup/Items/Sale/SaleComponentForm';

import { ItemPaymentComponentForm } from '../theme/Module/setup/Items/Payment/ItemPaymentComponentForm';
import { ItemPaymentComponentList } from '../theme/Module/setup/Items/Payment/ItemPaymentComponentList';
//end
//appointment module
import { AppointmentComponentList } from '../theme/Module/setup/Appointment/AppointmentComponentList';
import { VitalPresComponentList } from '../theme/Module/setup/Appointment/VitalPresComponentList';
import { BillComponentForm } from '../theme/Module/setup/Appointment/BillComponentForm';
import { AppointComponentList } from '../theme/Module/setup/Appointment/AppointComponentList';
import { DocumentComponentList } from '../theme/Module/setup/Appointment/DocumentComponentList';
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
//end
const SecureRoutes: Routes = [
    {
        'path': '',
        'component': ThemeComponent,
        'canActivate': [AuthGuard],
        children: [
            { path: 'changepassword', component: ChangepasswordComponent, canActivate: [AuthGuard] },
            { path: 'setup', component: SetupDashboardComponent, canActivate: [AuthGuard] },
            { path: 'dashboard', component: DashboardForm, canActivate: [AuthGuard] },
            { path: 'dashboard', component: DashboardForm, canActivate: [AuthGuard] },
            { path: 'Patient', component: PatientComponentList, canActivate: [AuthGuard] },
            { path: 'Patient/savePatient:id', component: PatientComponentForm, canActivate: [AuthGuard] },
            { path: 'Patient/savePatient', component: PatientComponentForm, canActivate: [AuthGuard] },
            {
                path: 'Summary', component: SummeryComponentList, canActivate: [AuthGuard],
                children: [
                    { path: 'Vital:id', component: VitalComponentList, canActivate: [AuthGuard] },
                    { path: 'Vital', component: VitalComponentList, canActivate: [AuthGuard] },
                    { path: 'BillSummary:id', component: BillSummeryComponentForm, canActivate: [AuthGuard] },
                    { path: 'BillSummary', component: BillSummeryComponentForm, canActivate: [AuthGuard] },
                    { path: 'AppointSummery:id', component: AppointSummeryComponentList, canActivate: [AuthGuard] },
                    { path: 'AppointSummery', component: AppointSummeryComponentList, canActivate: [AuthGuard] },
                    { path: 'DocSummery:id', component: DocumentSummeryComponentList, canActivate: [AuthGuard] },
                    { path: 'DocSummery', component: DocumentSummeryComponentList, canActivate: [AuthGuard] },
                ]
            },
            { path: 'Expense', component: ExpenseComponentList, canActivate: [AuthGuard] },
            { path: 'Expense/saveExpense:id', component: ExpenseComponentForm, canActivate: [AuthGuard] },
            { path: 'Expense/saveExpense', component: ExpenseComponentForm, canActivate: [AuthGuard] },
            { path: 'Prescription', component: PrescriptionComponentList, canActivate: [AuthGuard] },
            { path: 'Prescription/savePrescription:id', component: PrescriptionComponentForm, canActivate: [AuthGuard] },
            { path: 'Prescription/savePrescription', component: PrescriptionComponentForm, canActivate: [AuthGuard] },
            { path: 'Billing', component: BillingComponentList, canActivate: [AuthGuard] },
            { path: 'Billing/savebill:id', component: BillingComponentForm, canActivate: [AuthGuard] },
            { path: 'Billing/savebill', component: BillingComponentForm, canActivate: [AuthGuard] },
            { path: 'report', component: ReportComponentForm, canActivate: [AuthGuard] },
            { path: 'Income', component: IncomeComponentList, canActivate: [AuthGuard] },
            { path: 'Income:id', component: IncomeComponentList, canActivate: [AuthGuard] },
            { path: 'Income/saveIncome:id', component: IncomeComponentForm, canActivate: [AuthGuard] },
            { path: 'Income/saveIncome', component: IncomeComponentForm, canActivate: [AuthGuard] },
            //admission module
            { path: 'Admit', component: AdmitComponentList, canActivate: [AuthGuard] },
            { path: 'Admit/saveAdmit:id', component: AdmitComponentForm, canActivate: [AuthGuard] },
            { path: 'Admit/saveAdmit', component: AdmitComponentForm, canActivate: [AuthGuard] },

            {
                path: 'AdmitSummary', component: AdmitSummeryComponentList, canActivate: [AuthGuard],
                children: [
                    { path: 'AdmitDocSummery', component: AdmitDocumentSummeryComponentList, canActivate: [AuthGuard] },
                    { path: 'DischargeReport', component: DischargeReportComponent, canActivate: [AuthGuard] },
                    { path: 'AdmitNote', component: AdmitNoteComponentList, canActivate: [AuthGuard] },
                    { path: 'AdmitLabs', component: AdmitLabsComponentList, canActivate: [AuthGuard] },
                    { path: 'AdmitMedication', component: AdmitMedicationList, canActivate: [AuthGuard] },
                    { path: 'AdmitImaging', component: AdmitImagingList, canActivate: [AuthGuard] },
                    { path: 'AdmitCharge', component: AdmitChargeComponentList, canActivate: [AuthGuard] },
                    { path: 'AdmitDetail', component: AdmitDetailComponentList, canActivate: [AuthGuard] },
                    {
                        path: 'PatientInfo', component: AdmitPatientComponentList, canActivate: [AuthGuard],
                        children: [
                            { path: '', component: AdmitAppointSummeryComponentList, canActivate: [AuthGuard] },
                        ]
                    },
                    {
                        path: 'AdmitCharge', component: AdmitChargeComponentList, canActivate: [AuthGuard],
                        children: [
                            { path: 'AdmitPro', component: AdmitProcedureComponentList, canActivate: [AuthGuard] }
                        ]
                    },
                ]
            },
            ///end
            //setting module
            { path: 'DropDown', component: DropDownComponentList, canActivate: [AuthGuard] },
            { path: 'reminder', component: ReminderComponentList, canActivate: [AuthGuard] },
            { path: 'reminder/savereminder', component: ReminderComponentForm, canActivate: [AuthGuard] },
            { path: 'reminder/savereminder:id', component: ReminderComponentForm, canActivate: [AuthGuard] },

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
            { path: 'Sale/savesale:id', component: SaleComponentForm },
            { path: 'Sale/savesale', component: SaleComponentForm },
            { path: 'Sale', component: SaleComponentList },
            { path: 'Salehold', component: SaleHoldComponentList },
            { path: 'Payment/savepayment:id', component: ItemPaymentComponentForm },
            { path: 'Payment/savepayment', component: ItemPaymentComponentForm },
            { path: 'Payment', component: PaymentComponentList },
            //end
            //appointment module
            {
                path: 'Appoint', component: AppointmentComponentList, canActivate: [AuthGuard],
                children: [
                    { path: 'VitalPres:id', component: VitalPresComponentList, canActivate: [AuthGuard] },
                    { path: 'VitalPres', component: VitalPresComponentList, canActivate: [AuthGuard] },
                    { path: 'BillPres:id', component: BillComponentForm, canActivate: [AuthGuard] },
                    { path: 'BillPres', component: BillComponentForm, canActivate: [AuthGuard] },
                    { path: 'AppointPres:id', component: AppointComponentList, canActivate: [AuthGuard] },
                    { path: 'AppointPres', component: AppointComponentList, canActivate: [AuthGuard] },
                    { path: 'DocPres:id', component: DocumentComponentList, canActivate: [AuthGuard] },
                    { path: 'DocPres', component: DocumentComponentList, canActivate: [AuthGuard] },
                ]
            },
            //end
            //admin module
            { path: 'Admin', component: PaymentComponentList, canActivate: [AuthGuard] },
            { path: 'Admin/setting:id', component: PaymentComponentForm, canActivate: [AuthGuard] },
            { path: 'Admin/setting', component: PaymentComponentForm, canActivate: [AuthGuard] },
            { path: 'Admin/Subscriber', component: SubscriberComponentList, canActivate: [AuthGuard] },
            { path: 'Admin/register', component: RegisterComponentForm, canActivate: [AuthGuard] },
            { path: 'integration', component: IntegrationComponent, canActivate: [AuthGuard] },
            //end
            //employee module
            { path: 'Attendance', component: AttendanceComponentList, canActivate: [AuthGuard] },
            { path: 'Attendance/saveAttend', component: AttendanceComponentForm, canActivate: [AuthGuard] },
            { path: 'Attendance/saveAttend:id', component: AttendanceComponentForm, canActivate: [AuthGuard] },
            { path: 'Leaveentry', component: LeaveApplicationComponentList, canActivate: [AuthGuard] },
            { path: 'Leaveentry/saveleave', component: LeaveApplicationComponentForm, canActivate: [AuthGuard] },
            { path: 'Leaveentry/saveleave:id', component: LeaveApplicationComponentForm, canActivate: [AuthGuard] },
            { path: 'Timeattendance', component: TimeAttendanceComponentForm, canActivate: [AuthGuard] },
            { path: 'Timeattendancelist', component: TimeAttendanceComponentList, canActivate: [AuthGuard] },
            { path: 'Loan', component: LoanComponentList, canActivate: [AuthGuard] },
            { path: 'Employee/saveemployee:id', component: EmployeeComponent },
            { path: 'Employee/saveemployee', component: EmployeeComponent },
            { path: 'Employee', component: EmployeeComponentList },
            { path: 'Department', component: DepartmentComponentList, canActivate: [AuthGuard] },
            { path: 'Designation', component: designationComponentList, canActivate: [AuthGuard] },
            { path: 'Payroll', component: PayrollComponentList, canActivate: [AuthGuard] },

            //end
        ],
    },
    {
        'path': '**',
        'redirectTo': 'login',
        'pathMatch': 'full',
    },
];
export const SecureComponent: any[] = [
    SummeryComponentList, AppointSummeryComponentList,
    BillSummeryComponentForm,
    IncomeComponentForm, IncomeComponentList, VitalComponentList,
    DocumentSummeryComponentList,
    ChangepasswordComponent,
    SetupDashboardComponent,
    ReportComponentForm,
    DashboardForm,
    PageListComponent,
    //employee module
    DepartmentComponentList, PayrollComponentList, PayrollComponentForm, AttendanceComponentList, AttendanceComponentForm, DepartmentComponentForm, designationComponentList,
    designationComponentForm, LoanComponentList, LoanComponentForm, TimeAttendanceComponentList, TimeAttendanceComponentForm, LeaveApplicationComponentForm, EmployeeComponent, EmployeeComponentList,
    //end
    //patient module
    AdmitSummeryComponentList, AdmitDetailComponentList, DischargeReportComponent, AdmitPatientComponentList,
    AdmitComponentList, LeaveApplicationComponentList, AdmitComponentForm, AdmitAppointSummeryComponentList,
    AdmitDocumentSummeryComponentList, AdmitVitalComponentList, AdmitLabsComponentList, AdmitMedicationList,
    AdmitImagingList, AdmitNoteComponentList, AdmitProcedureComponentList, AdmitChargeComponentList,
    //end
    //setting module
    MedicineInstructionComponentList, ObservationComponentList, ComplaintComponentList, InvestigationComponentList, DiagnosComponentList, DropDownComponentList,
    ApplicationUserComponentList, ApplicationUserComponentForm, MedicineComponentList, MedicineComponentForm, CompanyComponentForm, UserRoleComponentList
    , ProfileUserComponentForm, ServiceComponentList, ServiceComponentForm,
    UserRoleComponentForm, FilterPipe, ReminderComponentList, ReminderComponentForm,
    //end
    //Item Module
    ItemComponentList, SaleHoldComponentList, ItemComponentForm, VendorComponentList, VendorComponentForm,
    InvoiceComponentList, InvoiceComponentForm, SaleComponentList, SaleComponentForm, ItemPaymentComponentForm
    , ItemPaymentComponentList, RestockComponentList, ExpireComponentList, ImportItemComponent,
    //end
    //appointment module
    AppointmentComponentList, VitalPresComponentList, BillComponentForm, AppointComponentList, DocumentComponentList,
    //end
    //admin module
    PaymentComponentForm, PaymentComponentList, SubscriberComponentList, RegisterComponentForm,
    IntegrationComponent,
    //end
    PatientComponentList, PatientComponentForm, ExpenseComponentList, ExpenseComponentForm,
    PrescriptionComponentList, PrescriptionComponentForm, BillingComponentForm, BillingComponentList
];
@NgModule({
    imports: [CommonModule, FormsModule, ReactiveFormsModule, LayoutModule, RouterModule.forChild(SecureRoutes), MatStepperModule, MatFormFieldModule, MatInputModule, MatAutocompleteModule, MatCheckboxModule
        , MatSelectModule, MatIconModule, MatDialogModule, MatTableModule, MatPaginatorModule, MatSortModule, MyDatePickerModule, NgbModule, MultiselectDropdownModule, NgxEchartsModule
        , CommonUtilsModule],
    exports: [RouterModule],
    declarations: [
        SecureComponent
    ],
    bootstrap: [],
    providers: [CookieService, AuthGuard, DashboardMenuService]
})
export class ThemeRoutingModule { }