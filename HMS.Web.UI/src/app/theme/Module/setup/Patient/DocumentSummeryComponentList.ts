import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { emr_patient, emr_Appointment, emr_document, emr_vital, patientInfo, DoctorInfo } from './../Appointment/AppointmentModel';
import { emr_patient_bill } from '../Billing/BillingModel';
import { emr_prescription_mf, emr_prescription_complaint, emr_prescription_diagnos, emr_prescription_investigation, emr_prescription_observation, emr_prescription_treatment, emr_prescription_treatment_template, emr_medicine } from './../Prescription/PrescriptionModel';
import { Observable } from 'rxjs';
import { AppointmentService } from './../Appointment/AppointmentService';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { filter } from 'rxjs/operators';
import { IMyDateModel } from 'mydatepicker';
import { CommonService } from '../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { GlobalVariable } from '../../../../AngularConfig/global';
@Component({
    templateUrl: './DocumentSummeryComponentList.html',
    moduleId: module.id,
    providers: [AppointmentService],
})

export class DocumentSummeryComponentList implements OnInit {
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public Id: string; public DocumentImage: string = '';
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public filterdData = [];
    public ID: number = 10;
    public IsAdmin: boolean = false;
    public model = new emr_patient();
    public PayrollRegion: string; public PModel = new PaginationModel(); public PConfig = new PaginationConfig();
    public Keywords: any[] = [];
    public sub: any; public IsLoadDocument: boolean = false;
    public IsEdit: boolean = false; public IsVital: boolean = false;
    public CompanyInfo: any[] = [];
    public BillModel = new emr_patient_bill();
    public DocumentList: any[] = []; public GenderList: any[] = [];  
    public VitalModel = new emr_vital(); public FutureAppList: any[] = []; public PreviousAppList: any[] = [];
    public Docmodel = new emr_document(); public IsNewPatientImage: boolean = true; public IsNewImage: boolean = true;
    public PatientId: any; public PatientImage: string = '';
    public DocumentTypeList = [];
    public ClinicName: string = "";
    public MedicineList: any[] = [];
    public DoctorInfo = new DoctorInfo();
    public PatientRXmodel = new patientInfo();
    public IsList: boolean = true;
    public Form4: FormGroup;
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public Rights: any; public CompanyObj: any;
    public ControlRights: any;
    @ViewChild("DocumentContent") DocumentContent: TemplateRef<any>;
    openDocumentModal(DocumentContent) {
        this.loader.ShowLoader();
        this._AppointmentService.GetDocmentType().then(m => {
            if (m.IsSuccess) {
                this.loader.HideLoader();
                this.Docmodel.Date = new Date();
                this.DocumentTypeList = m.ResultSet.DocumentType;
                this.modalService.open(DocumentContent, { size: 'md' });
            } else
                this.loader.HideLoader();
        });

    }
    constructor(public _fb: FormBuilder,
        public loader: LoaderService,
        public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public _CommonService: CommonService, public route: ActivatedRoute, public _router: Router, public _AppointmentService: AppointmentService, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("25");
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.ClinicName = this.CompanyObj.CompanyName;
    }
    ngOnInit() {
        this.PatientId = localStorage.getItem('PatientId');
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
        this.Form4 = this._fb.group({
            Remarks: [''],
            DocumentUpload: [''],
            Date: ['', [Validators.required]],
            DocumentTypeId: ['', [Validators.required]],
        });
    }
    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.GetDocList();
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
   //Document
   
    DocSaveOrUpdate(isValid: boolean): void {
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._AppointmentService.DocumentSave(this.Docmodel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll(this.DocumentContent)
                    this.GetDocList();
                    this.loader.HideLoader();
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }
    GetDocList() {
        this.DocumentGridList();
    }
    DocumentGridList() {
        this.PModel.VisibleColumnInfo = "Date#Date,Remarks#Remarks,Tag#Tag,Uploaddate#Uploaddate";
            this.loader.ShowLoader();
        this.Id = "0";
        this._AppointmentService
            .GetDocList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.DocumentList = m.DataList;
                this.DocumentList.forEach(x => {
                    if (x.DocumentUpload != null && x.DocumentUpload != undefined)
                        x.DocumentImage = GlobalVariable.BASE_Temp_File_URL + '' + x.DocumentUpload;
                });
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.loader.HideLoader();
            });
    }
    Delete(id: any) {
         var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._AppointmentService.DeleteDoc(id).then(m => {

                if (m.ErrorMessage != null) {

                    alert(m.ErrorMessage);
                }
                this.GetDocList();
            });
        }
    }
    AddDocument(id: string) {
        this.loader.ShowLoader();
        this._AppointmentService.DocGetById(parseInt(id)).then(m => {
            this.DocumentTypeList = [];
            this.DocumentTypeList = m.ResultSet.DocumentType;
            this.Docmodel = m.ResultSet.emr_documentObj;
            if (this.Docmodel.DocumentUpload != null || this.Docmodel.DocumentUpload != undefined) {
                this.getDocImageUrlName(this.Docmodel.DocumentUpload);
                this.IsNewImage = false;
            } else this.IsNewImage = true;
            this.modalService.open(this.DocumentContent, { size: 'md' });
        });
        this.loader.HideLoader();
    }
    getDocImageUrlName(FName) {
        this.Docmodel.DocumentUpload = FName;
        if (this.IsEdit && !this.IsNewImage) {
            this.DocumentImage = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        } else {
            this.DocumentImage = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        }
    }
    onDocDateChanged(event: IMyDateModel) {
        if (event) {
        }
    }
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._AppointmentService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    ViewDoc(id: any) {
        window.open(id, '_blank');
    }
    UpdateDocRecord(id: any) {
        if (id > 0) {
            this.loader.ShowLoader();
            this.IsEdit = true;
            this._AppointmentService.GetDocById(id).then(m => {
                this.Docmodel = new emr_document();
                this.DocumentTypeList = m.ResultSet.DocumentType;
                this.Docmodel = m.ResultSet.emr_documentObj;
                if (this.Docmodel.DocumentUpload != null || this.Docmodel.DocumentUpload != undefined) {
                    this.getDocImageUrlName(this.Docmodel.DocumentUpload);
                    this.IsNewImage = false;
                } else this.IsNewImage = true;
                this.modalService.open(this.DocumentContent, { size: 'md' });
                this.loader.HideLoader();
            });
        } else {
            this.BillModel.BillDate = new Date();
        }
    }
}
