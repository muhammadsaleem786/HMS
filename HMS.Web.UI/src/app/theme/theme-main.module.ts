//theme-main.module.ts
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardForm } from './Module/Dashboard/DashboardForm';

import { LabDashboardComponent } from './Module/setup/Lab/LabDashboardComponent';
import { AuthGuard } from '../CommonService/AuthGuard';
import { CommonUtilsModule } from './../common/common-utils.module';
import { ChangepasswordComponent } from './Module/changepassword/changepassword.component';
import { SetupDashboardComponent } from './Module/setup/SetupDashboard/SetupDashboardComponent';
import { PatientComponentForm } from '../theme/Module/setup/Patient/PatientComponentForm';
import { PatientComponentList } from '../theme/Module/setup/Patient/PatientComponentList';
import { PrescriptionComponentForm } from '../theme/Module/setup/Prescription/PrescriptionComponentForm';
import { PrescriptionComponentList } from '../theme/Module/setup/Prescription/PrescriptionComponentList';
import { BillingComponentForm } from '../theme/Module/setup/Billing/BillingComponentForm';
import { BillingComponentList } from '../theme/Module/setup/Billing/BillingComponentList';

import { SummeryComponentList } from '../theme/Module/setup/Patient/SummeryComponentList';
import { VitalComponentList } from '../theme/Module/setup/Patient/VitalComponentList';
import { BillSummeryComponentForm } from '../theme/Module/setup/Patient/BillSummeryComponentForm';
import { AppointSummeryComponentList } from '../theme/Module/setup/Patient/AppointSummeryComponentList';
import { DocumentSummeryComponentList } from '../theme/Module/setup/Patient/DocumentSummeryComponentList';
//appointment module
import { AppointmentComponentList } from '../theme/Module/setup/Appointment/AppointmentComponentList';
import { VitalPresComponentList } from '../theme/Module/setup/Appointment/VitalPresComponentList';
import { BillComponentForm } from '../theme/Module/setup/Appointment/BillComponentForm';
import { AppointComponentList } from '../theme/Module/setup/Appointment/AppointComponentList';
import { DocumentComponentList } from '../theme/Module/setup/Appointment/DocumentComponentList';
//end
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
import { SaleComponentList } from '../theme/Module/setup/Items/POS/SaleComponentList';
import { SaleHoldComponentList } from '../theme/Module/setup/Items/POS/SaleHoldComponentList';
import { SaleComponentForm } from '../theme/Module/setup/Items/POS/SaleComponentForm';

const routes: Routes = [
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
     { path: 'Prescription', component: PrescriptionComponentList, canActivate: [AuthGuard] },
    { path: 'Prescription/savePrescription:id', component: PrescriptionComponentForm, canActivate: [AuthGuard] },
    { path: 'Prescription/savePrescription', component: PrescriptionComponentForm, canActivate: [AuthGuard] },
    { path: 'Billing', component: BillingComponentList, canActivate: [AuthGuard] },
    { path: 'Billing/savebill:id', component: BillingComponentForm, canActivate: [AuthGuard] },
    { path: 'Billing/savebill', component: BillingComponentForm, canActivate: [AuthGuard] },
    
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
    //lab module
    { path: 'lab', component: LabDashboardComponent, canActivate: [AuthGuard] },
    { path: 'Sale/savesale:id', component: SaleComponentForm },
    { path: 'Sale/savesale', component: SaleComponentForm },
    { path: 'Sale', component: SaleComponentList },
    { path: 'Salehold', component: SaleHoldComponentList },
];
@NgModule({
    declarations: [
        SummeryComponentList, AppointSummeryComponentList,
        BillSummeryComponentForm,
        VitalComponentList,
        DocumentSummeryComponentList,
        ChangepasswordComponent,
        SetupDashboardComponent,       
        DashboardForm, PatientComponentList, PatientComponentForm, 
        PrescriptionComponentList, PrescriptionComponentForm, BillingComponentForm, BillingComponentList,
        //appointment module
        AppointmentComponentList, VitalPresComponentList, BillComponentForm, AppointComponentList, DocumentComponentList,
        //end
        LabDashboardComponent,
        //admission module
        AdmitSummeryComponentList, AdmitDetailComponentList, DischargeReportComponent, AdmitPatientComponentList,
        AdmitComponentList, AdmitComponentForm, AdmitAppointSummeryComponentList,
        AdmitDocumentSummeryComponentList, AdmitVitalComponentList, AdmitLabsComponentList, AdmitMedicationList,
        AdmitImagingList, AdmitNoteComponentList, AdmitProcedureComponentList, AdmitChargeComponentList,
        //end
         SaleHoldComponentList, SaleComponentList, SaleComponentForm
    ],
    imports: [
        CommonUtilsModule,
        RouterModule.forChild(routes)
    ],
    providers: [AuthGuard]
})
export class ThemeMainModule { }
