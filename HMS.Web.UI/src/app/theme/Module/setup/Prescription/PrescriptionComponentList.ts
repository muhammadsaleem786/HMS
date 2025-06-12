import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { PrescriptionService } from './PrescriptionService';
import { emr_patient, emr_Appointment, emr_document, emr_vital, patientInfo, DoctorInfo } from './../Appointment/AppointmentModel';
import { emr_prescription_mf, emr_prescription_complaint, emr_prescription_diagnos, emr_prescription_investigation, emr_prescription_observation, emr_prescription_treatment } from './PrescriptionModel';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../CommonService/CommonService';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { AppointmentService } from './../Appointment/AppointmentService';
import { strictEqual } from 'assert';
import { GlobalVariable } from '../../../../AngularConfig/global';
@Component({
    moduleId: module.id,
    templateUrl: 'PrescriptionComponentList.html',
    providers: [PrescriptionService, AppointmentService],
})
export class PrescriptionComponentList {
    public ActiveToggle: boolean = false;
    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public PrescriptionList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public ControlRights: any;
    public MedicineList = [];
    public emr_prescription_dynamicArray = [];
    public emr_prescription_complaint_dynamicArray = [];
    public emr_prescription_diagnos_dynamicArray = [];
    public emr_prescription_investigation_dynamicArray = [];
    public emr_prescription_observation_dynamicArray = [];
    public emr_prescription_MedicineArray = [];
    public emr_prescription_treatment_dynamicArray = [];
    public PatientVitalList: any[] = [];
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = []; public UsersObj: any;
    public DoctorInfo = new DoctorInfo();
    public PatientRXmodel = new patientInfo();
    @ViewChild("PrintRx") PrintRxContent: TemplateRef<any>;
    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public _AppointmentService: AppointmentService, private modalService: NgbModal, public loader: LoaderService, public _PrescriptionService: PrescriptionService
        , public _router: Router, public toastr: CommonToastrService,) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("21");
        this.UsersObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
    }
    ngOnInit() {

        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
    }
    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.Refresh();
    }
    getPages(totalRecord: number, recordPerPage: number) {
        if (!isNaN(totalRecord))
            this.totalPages = this.getTotalPages(totalRecord, recordPerPage);
        this.getpagesRange();
    }
    getpagesRange() {
        if (this.pagesRange.indexOf(this.PModel.CurrentPage) == -1 || this.totalPages <= 10)
            this.papulatePagesRange();
        else if (this.pagesRange.length == 1 && this.totalPages > 10)
            this.papulatePagesRange();
    }
    papulatePagesRange() {
        this.pagesRange = [];
        var Result = Math.ceil(Math.max(this.PModel.CurrentPage, 1) / Math.max(this.PModel.RecordPerPage, 1));
        this.previousPage = ((Result - 1) * this.PModel.RecordPerPage)
        this.nextPage = (Result * this.PModel.RecordPerPage) + 1;
        if (this.nextPage > this.totalPages)
            this.nextPage = this.totalPages;
        for (var i = 1; i <= 10; i++) {
            if ((this.previousPage + i) > this.totalPages) return;
            this.pagesRange.push(this.previousPage + i)
        }
    }
    getTotalPages(totalRecord: number, recordPerPage: number): number {

        return Math.ceil(Math.max(totalRecord, 1) / Math.max(recordPerPage, 1));
    }
    Refresh() {
        this.PModel.VisibleColumnInfo = "AppointmentDate#AppointmentDate,PatientName#PatientName,medicine#medicine,amount#amount";
        this.loader.ShowLoader();
        this.Id = "0";
        this._PrescriptionService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {

                this.PModel.TotalRecord = m.TotalRecord;
                this.PrescriptionList = m.OtherDataModel || [];
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.loader.HideLoader();
            });
    }
    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/Prescription/savePrescription']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Prescription/savePrescription'], { queryParams: { id: this.Id } });
    }
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    Delete(id: string) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._PrescriptionService.Delete(id).then(m => {

                if (m.ErrorMessage != null) {

                    alert(m.ErrorMessage);
                }
                this.Refresh();
            });
        }
    }
    GetList() {
        this.Refresh();
    }
    Close(idpar) {
        this.IsList = true;
        if (idpar == 0)
            this.Id = '0';
        else
            this.Refresh();
    }
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._PrescriptionService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
    PrintRxform(id: any) {
        this.loader.ShowLoader();
        this._AppointmentService.PrintRXById(id).then(m => {
            if (m.IsSuccess) {
                this.emr_prescription_complaint_dynamicArray = [];
                this.emr_prescription_observation_dynamicArray = [];
                this.emr_prescription_investigation_dynamicArray = [];
                this.emr_prescription_diagnos_dynamicArray = [];
                this.emr_prescription_treatment_dynamicArray = [];
                this.PatientVitalList = [];
                if (m.ResultSet.doctor != null)
                    this.DoctorInfo = m.ResultSet.doctor;

                if (this.DoctorInfo == null || this.DoctorInfo == undefined) {
                    this.DoctorInfo.Name = "";
                    this.DoctorInfo.Designation = "";
                    this.DoctorInfo.Qualification = "";
                    this.DoctorInfo.PhoneNo = "";
                    this.DoctorInfo.TemplateId = "";
                }
                this.PatientRXmodel.PatientName = m.ResultSet.result.PatientName;
                if (m.ResultSet.result.ClinicIogo != null)
                    this.PatientRXmodel.ClinicIogo = GlobalVariable.BASE_Temp_File_URL + '' + m.ResultSet.result.ClinicIogo;
                this.PatientRXmodel.Age = m.ResultSet.result.Age;
                this.PatientRXmodel.AppointmentDate = this._CommonService.GetFormatDate(m.ResultSet.result.AppointmentDate);
                this.PatientVitalList = m.ResultSet.vitallist;
                if (m.ResultSet.result.emr_prescription_complaint != null) {
                    m.ResultSet.result.emr_prescription_complaint.forEach((item, index) => {
                        this.emr_prescription_complaint_dynamicArray.push(item);
                    });
                }
                if (m.ResultSet.result.emr_prescription_diagnos != null) {
                    m.ResultSet.result.emr_prescription_diagnos.forEach((item, index) => {
                        this.emr_prescription_diagnos_dynamicArray.push(item);
                    });
                }
                if (m.ResultSet.result.emr_prescription_investigation != null) {
                    m.ResultSet.result.emr_prescription_investigation.forEach((item, index) => {
                        this.emr_prescription_investigation_dynamicArray.push(item);
                    });
                }
                if (m.ResultSet.result.emr_prescription_observation != null) {

                    m.ResultSet.result.emr_prescription_observation.forEach((item, index) => {
                        this.emr_prescription_observation_dynamicArray.push(item);
                    });
                }
                if (m.ResultSet.result.emr_prescription_treatment != null) {
                    m.ResultSet.result.emr_prescription_treatment.forEach((item, index) => {
                        var medicineName = m.ResultSet.MedicineList.filter(a => a.Id == item.MedicineId)[0];
                        if (medicineName != null && medicineName != undefined)
                            item.MedicineName = medicineName.Medicine;
                        else
                            item.MedicineName = item.MedicineName;

                        item.MedicineType = medicineName.Type;
                        item.TypeId = medicineName.TypeId;
                        this.emr_prescription_treatment_dynamicArray.push(item);
                    });
                }
                this.modalService.open(this.PrintRxContent, { size: 'lg' });
            } else
                this.toastr.Error('Error', m.ErrorMessage);
            this.loader.HideLoader();
        });
    }
    loadMedicine(Ids: any): string {
        let Name = "";
        Ids.forEach((item, index) => {
            item.forEach((item1, index) => {
                var aa = item1;
                var name = this.MedicineList.filter(a => a.Id == item1.mid);
                if (Name != "")
                    Name += "," + name[0].Medicine;
                else
                    Name += name[0].Medicine;
            });
        });
        return Name;
    };
}