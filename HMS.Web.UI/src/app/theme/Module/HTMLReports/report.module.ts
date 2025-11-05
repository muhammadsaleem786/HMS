import { NgModule } from '@angular/core';
import { ReportRoutingModule } from './report-routing.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonUtilsModule } from '../../../common/common-utils.module';
import { CookieService } from 'ngx-cookie-service';
import { AuthGuard } from '../../../CommonService/AuthGuard';
import { DashboardMenuService } from '../../../CommonService/DashboardMenuService';

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

@NgModule({
    imports: [
        CommonUtilsModule, NgbModule, 
        ReportRoutingModule,
    ],
    declarations: [
        CashFlowRptForm, MedicinewiseRptForm, OutstandingIncomeRptForm, PaymentSummaryRptForm, TreatmentwiseRptForm,
        BirthdayComponentRptForm, DetailedPatientComponentRptForm, FollowupComponentRptForm, OutstandingComponentRptForm, PaymentComponentRptForm,
        ClinicwiseComponentRptForm, DoctorswisePaymentComponentRptForm, DoctorswiseComponentRptForm, DoctorPaymentSummaryComponentRptForm,
        PatientDetailComponentForm, ProfitLossRptForm, CurrentStockRptForm, SaleByPatientRptForm, ProcedureComponentRptForm, FeeComponentForm, AppointmentComponentReportForm, PurchaseRptForm
    ],
    providers: [
        CookieService,
        AuthGuard,
        DashboardMenuService
    ]
})
export class ReportModule { }
