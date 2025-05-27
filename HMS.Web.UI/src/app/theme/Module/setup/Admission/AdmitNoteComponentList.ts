import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { emr_patient, ipd_admission_vital, ipd_admission_notes } from './AdmitModel';
import { Observable } from 'rxjs';
import { AdmitService } from './AdmitService';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { filter } from 'rxjs/operators';
import { IMyDateModel } from 'mydatepicker';
import { CommonService } from '../../../../CommonService/CommonService';
import { GlobalVariable } from '../../../../AngularConfig/global';
@Component({
    templateUrl: './AdmitNoteComponentList.html',
    moduleId: module.id,
    providers: [AdmitService],
})

export class AdmitNoteComponentList implements OnInit {
    public Form1: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public Id: string;
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public IsSameModuleName: string;
    public IsChecked: boolean;
    public IsEmpExist: boolean = false;
    public ID: number = 10;
    public IsAdmin: boolean = false;
    public model = new emr_patient();
    public PayrollRegion: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public Keywords: any[] = [];
    public AdmitId: any;
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public NoteList: any = [];
    public noteModel = new ipd_admission_notes();
    public Rights: any;
    public NoteRights: any;
    public DischargeDate: any;
    nextPage: number = 1;
    totalPages: number = 0;
    public ClinicName: string = "";
    pagesRange: number[] = []; previousPage: number = 1;;
    public PatientId: any; public CompanyObj: any;
    @ViewChild("NoteModal") NoteModal: TemplateRef<any>;
    constructor(public _fb: FormBuilder,
        public loader: LoaderService,
        public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public _CommonService: CommonService, public route: ActivatedRoute, public _router: Router, public _AdmitService: AdmitService, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.NoteRights = this._CommonService.ScreenRights("44");
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.ClinicName = this.CompanyObj.CompanyName;
    }
    ngOnInit() {
        this.AdmitId = localStorage.getItem('AdmitId');
        this.PatientId = localStorage.getItem('PatientId');
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
        this.Form1 = this._fb.group({
            AdmissionId: [''],
            AppointmentId: [''],
            OnBehalfOf: [''],
            Note: [''],
        });
    }
    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.GetNoteList();
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
    GetNoteList() {
        this.NoteGridList();
    }
    NoteGridList() {
        this.PModel.VisibleColumnInfo = "Note#Note,Date#Date";
        this.loader.ShowLoader();
        this.Id = "0";
        this._AdmitService.GetNoteList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.AdmitId, this.PatientId, '0').then(m => {
            this.PModel.TotalRecord = m.TotalRecord;
            this.NoteList = m.DataList;
            //if (m.DataList.length != 0)
            //    this.DischargeDate = m.DataList[0].DischargeDate;
            this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
            this.loader.HideLoader();
        });
    }
    //Vital 

    NoteSaveOrUpdate(isValid: boolean): void {
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this.noteModel.AdmissionId = this.AdmitId;
            this.noteModel.PatientId = this.PatientId;
            this._AdmitService.NoteSaveOrUpdate(this.noteModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll();
                    this.NoteGridList();
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    AddNoteModal(NoteModal) {
        this.loader.ShowLoader();
        this.noteModel = new ipd_admission_notes();
        //let PatientId = localStorage.getItem('PatientId');
        //this._AdmitService.GetAppointmentList(PatientId).then(m => {
        //    if (m.IsSuccess) {
        //        this.AppointmentList = m.ResultSet.CurntPrevDateList;
        //        this.modalService.open(NoteModal, { size: 'md' });
        //    }
        //    this.loader.HideLoader();
        //});
        this.modalService.open(NoteModal, { size: 'md' });
        this.loader.HideLoader();
    }
    Delete(id: any) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._AdmitService.DeleteNote(id).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                this.NoteGridList();
                this.loader.HideLoader();
            });
        }
    }
    Edit(id: any) {
        this.loader.ShowLoader();
        let PatientId = localStorage.getItem('PatientId');
        this._AdmitService.NoteGetById(id, PatientId).then(m => {
            if (m.ResultSet != null) {

                this.IsEdit = true;
                //this.AppointmentList = m.ResultSet.CurntPrevDateList;
                this.noteModel = m.ResultSet.Result;
                this.modalService.open(this.NoteModal, { size: 'md' });
            }
            this.loader.HideLoader();
        });
    }
}
