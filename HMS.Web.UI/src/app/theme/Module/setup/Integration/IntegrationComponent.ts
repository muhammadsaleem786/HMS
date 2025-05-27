import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { IntegrationService } from './IntegrationService';
import { IntegrationModel } from './IntegrationModel';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { GlobalVariable } from '../../../../AngularConfig/global';
@Component({
    moduleId: module.id,
    templateUrl: 'IntegrationComponent.html',
    providers: [IntegrationService],
})
export class IntegrationComponent {
    public ActiveToggle: boolean = false;
    public isCollapsed = true;
    public Id: string;
    public Form1: FormGroup;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public IsList: boolean = true;
    public ID: number = 10;
    public Keywords: any[] = [];
    public submitted: boolean;
    @ViewChild("adEmail") adEmail: TemplateRef<any>;
    @ViewChild("adSMS") adSMS: TemplateRef<any>;
    public model = new IntegrationModel();
    public Rights: any;
    public ControlRights: any;
    public valueForUser: any;
    SMSModel: any = {};
    EmailModel: any = {};
    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public _fb: FormBuilder, public loader: LoaderService, public _IntegrationService: IntegrationService
        , public _router: Router, public toastr: CommonToastrService, private modalService: NgbModal) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("72");
        this.valueForUser = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
    }
    ngOnInit() {
        this.Form1 = this._fb.group({
            UserName: [''],
            Password: [''],
            Masking: [''],
            SMTP: [''],
            PortNo: [''],
            IsActive: [''],
            Type: ['']
        });
        this.Refresh();
    }
    Refresh() {
        this.loader.ShowLoader();
        this._IntegrationService.FormLoad().then(m => {
            if (m.IsSuccess) {
                const integrations = m.ResultSet.Integrationlist;
                this.SMSModel = integrations.find((x: any) => x.Type === 'T') || {
                    ID: 0,
                    UserName: '',
                    Password: '',
                    Masking: '',
                    Type: 'T',
                    IsActive: false
                };
                this.EmailModel = integrations.find((x: any) => x.Type === 'E') || {
                    ID: 0,
                    UserName: '',
                    Password: '',
                    SMTP: '',
                    PortNo: '',
                    Type: 'E',
                    IsActive: false
                };
            }
            this.loader.HideLoader();
        });
    }
    openSMSModel(adSMS: any) {
        this.model = {} as IntegrationModel;
        this.model = this.SMSModel;
        this.modalService.open(adSMS, { size: 'lg' });
    }
    openEmailModel(adEmail: any) {
        this.model = {} as IntegrationModel;
        this.model = this.EmailModel;
        this.modalService.open(adEmail, { size: 'lg' });
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._IntegrationService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.modalService.dismissAll(this.adSMS);
                    this.modalService.dismissAll(this.adEmail);
                    this.toastr.Success(result.Message);
                    this.Refresh();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    SmsActivate() {
        this.model = this.SMSModel;
        this.model.IsActive = true;
        this.SaveOrUpdate(true);
    }
    EmailActivate() {
        this.model = this.EmailModel;
        this.model.IsActive = true;
        this.SaveOrUpdate(true);
    }
    SmsDectivate() {
        this.model = this.SMSModel;
        this.model.IsActive = false;
        this.SaveOrUpdate(true);
    }
    EmailDectivate() {
        this.model = this.EmailModel;
        this.model.IsActive = false;
        this.SaveOrUpdate(true);
    }
    AddRecord(id: string) {
    }
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    Delete(id: string) {
        var result = this.toastr.DeleteAlert(); //confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._IntegrationService.Delete(id).then(m => {

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

}