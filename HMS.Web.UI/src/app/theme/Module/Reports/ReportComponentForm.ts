import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, Type } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ComponentRef, ViewContainerRef, ComponentFactoryResolver } from '@angular/core';
import { LoaderService } from '../../../CommonService/LoaderService';
import { EncryptionService } from '../../../CommonService/encryption.service';
import { ValidationVariables } from '../../../AngularConfig/global';
import { CommonService } from '../../../CommonService/CommonService';
//import { IMultiSelectOption, IMultiSelectSettings, IMultiSelectTexts } from 'angular-2-dropdown-multiselect';
//import * as XLSX from 'xlsx';

@Component({
    selector: 'report',
    templateUrl: 'ReportComponentForm.html',
    moduleId: module.id,
    providers: [FormBuilder]
})

export class ReportComponentForm implements OnInit {
    HideElement = false;
    firstday: Date;
    lastday: Date;
    public compRef: any;
    public ID: number;
    public CompanyName: string;
    public Rights: number[] = [];
    public Modules: number[] = [];

    public Keywords: any[] = [];
    public Currency: string = '';
    public PayrollRegion: string = '';
    public CompanyObj: any;
    public PackageID: number;
  
    @ViewChild('table') table: ElementRef;
    public ControlRights: any;
    constructor(public loader: LoaderService, private encrypt: EncryptionService, public resolver: ComponentFactoryResolver, public vcRef: ViewContainerRef
        , public _commonservice: CommonService) {
        this.Currency = this._commonservice.getCurrency();
        this.PayrollRegion = this._commonservice.getPayrollRegion();
      this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.Modules = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Modules')))
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')))
        this.CompanyName = this.CompanyObj.CompanyName;
    }
    ngOnInit() {

    }
    onPrint(): void {
        let printContents, popupWin;
        printContents = document.getElementById('container').innerHTML;
        popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
        popupWin.document.open();
        popupWin.document.write('<html><head><title>Print tab</title><style></style></head><body onload="window.print();window.close()">' + printContents + '</body></html>');
        popupWin.document.close();
    }
}