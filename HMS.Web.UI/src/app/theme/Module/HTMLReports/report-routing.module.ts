import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../../../CommonService/AuthGuard';
import { ThemeComponent } from '../../../theme/theme.component';
import { PatientDetailComponentForm } from '../../../theme/Module/HTMLReports/PatientReport/PatientDetailComponentForm';
import { FeeComponentForm } from '../../../theme/Module/HTMLReports/PatientReport/FeeComponentForm';
import { ProcedureComponentRptForm } from '../../../theme/Module/HTMLReports/PatientReport/ProcedureComponentRptForm';
import { AppointmentComponentReportForm } from '../../../theme/Module/HTMLReports/PatientReport/AppointmentComponentReportForm';
import { BirthdayComponentRptForm } from '../../../theme/Module/HTMLReports/PatientReport/BirthdayComponentRptForm';
import { DetailedPatientComponentRptForm } from '../../../theme/Module/HTMLReports/PatientReport/DetailedPatientComponentRptForm';
import { FollowupComponentRptForm } from '../../../theme/Module/HTMLReports/PatientReport/FollowupComponentRptForm';
import { OutstandingComponentRptForm } from '../../../theme/Module/HTMLReports/PatientReport/OutstandingComponentRptForm';
import { PaymentComponentRptForm } from '../../..//theme/Module/HTMLReports/PatientReport/PaymentComponentRptForm';
import { CashFlowRptForm } from '../../..//theme/Module/HTMLReports/FinancialReport/CashFlowRptForm';
import { MedicinewiseRptForm } from '../../../theme/Module/HTMLReports/FinancialReport/MedicinewiseRptForm';
import { OutstandingIncomeRptForm } from '../../../theme/Module/HTMLReports/FinancialReport/OutstandingIncomeRptForm';
import { PaymentSummaryRptForm } from '../../../theme/Module/HTMLReports/FinancialReport/PaymentSummaryRptForm';
import { TreatmentwiseRptForm } from '../../../theme/Module/HTMLReports/FinancialReport/TreatmentwiseRptForm';
import { ClinicwiseComponentRptForm } from '../../../theme/Module/HTMLReports/DoctorReport/ClinicwiseComponentRptForm';
import { DoctorswiseComponentRptForm } from '../../../theme/Module/HTMLReports/DoctorReport/DoctorswiseComponentRptForm';
import { DoctorswisePaymentComponentRptForm } from '../../../theme/Module/HTMLReports/DoctorReport/DoctorswisePaymentComponentRptForm';
import { DoctorPaymentSummaryComponentRptForm } from '../../../theme/Module/HTMLReports/DoctorReport/DoctorPaymentSummaryComponentRptForm';
import { CurrentStockRptForm } from '../../../theme/Module/HTMLReports/InventoryReport/CurrentStockRptForm';
import { SaleByPatientRptForm } from '../../../theme/Module/HTMLReports/InventoryReport/SaleByPatientRptForm';
import { PurchaseRptForm } from '../../../theme/Module/HTMLReports/InventoryReport/PurchaseRptForm';
import { ProfitLossRptForm } from '../../../theme/Module/HTMLReports/FinancialReport/ProfitLossRptForm';



const routes: Routes = [
    {
        'path': '',
        'component': ThemeComponent,
        'canActivate': [AuthGuard],
        children: [
            { path: 'PatientRpt', component: PatientDetailComponentForm, canActivate: [AuthGuard] },
            { path: 'FeeRpt', component: FeeComponentForm, canActivate: [AuthGuard] },
            { path: 'ProRpt', component: ProcedureComponentRptForm, canActivate: [AuthGuard] },
            { path: 'ProlossRpt', component: ProfitLossRptForm, canActivate: [AuthGuard] },
            { path: 'CurrentRpt', component: CurrentStockRptForm, canActivate: [AuthGuard] },
            { path: 'SaleRpt', component: SaleByPatientRptForm, canActivate: [AuthGuard] },
            { path: 'AppRpt', component: AppointmentComponentReportForm, canActivate: [AuthGuard] },
            { path: 'CashFlowRpt', component: CashFlowRptForm, canActivate: [AuthGuard] },
            { path: 'MedicinewiseRpt', component: MedicinewiseRptForm, canActivate: [AuthGuard] },
            { path: 'OutstandingIncomeRpt', component: OutstandingIncomeRptForm, canActivate: [AuthGuard] },
            { path: 'PaymentSummaryRpt', component: PaymentSummaryRptForm, canActivate: [AuthGuard] },
            { path: 'TreatmentwiseRpt', component: TreatmentwiseRptForm, canActivate: [AuthGuard] },
            { path: 'BirthdayRpt', component: BirthdayComponentRptForm, canActivate: [AuthGuard] },
            { path: 'DetailedPatientRpt', component: DetailedPatientComponentRptForm, canActivate: [AuthGuard] },
            { path: 'FollowupRpt', component: FollowupComponentRptForm, canActivate: [AuthGuard] },
            { path: 'OutstandingRpt', component: OutstandingComponentRptForm, canActivate: [AuthGuard] },
            { path: 'PaymentRpt', component: PaymentComponentRptForm, canActivate: [AuthGuard] },
            { path: 'ClinicRpt', component: ClinicwiseComponentRptForm, canActivate: [AuthGuard] },
            { path: 'DocSummaryRpt', component: DoctorPaymentSummaryComponentRptForm, canActivate: [AuthGuard] },
            { path: 'DoctorswiseRpt', component: DoctorswiseComponentRptForm, canActivate: [AuthGuard] },
            { path: 'PurRpt', component: PurchaseRptForm, canActivate: [AuthGuard] },
            { path: 'DocPaymentRpt', component: DoctorswisePaymentComponentRptForm, canActivate: [AuthGuard] },
               
        ]
    }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportRoutingModule { }
