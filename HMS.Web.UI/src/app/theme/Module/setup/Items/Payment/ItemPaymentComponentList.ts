import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { PaymentService } from './PaymentService';
import { PaymentModel } from './PaymentModel';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { GlobalVariable } from '../../../../../AngularConfig/global';
@Component({
    moduleId: module.id,
    templateUrl: 'ItemPaymentComponentList.html',
    providers: [PaymentService],
})
export class ItemPaymentComponentList {
    public ActiveToggle: boolean = false;
    public isCollapsed = true;
    public Id: string;
    public Form1: FormGroup;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public PaymentList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Keywords: any[] = [];
    public submitted: boolean;
    public model = new PaymentModel();
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public Rights: any;
    public ControlRights: any;
    public valueForUser: any;
    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public _fb: FormBuilder, public loader: LoaderService, public _PaymentService: PaymentService
    , public _router: Router, public toastr: CommonToastrService, private modalService: NgbModal) {
    this.loader.ShowLoader();
    this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
    this.ControlRights = this._CommonService.ScreenRights("64");
    this.valueForUser = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
}
ngOnInit() {
    this.PModel.SortName = "";
    this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
    this.selectPage(this.PModel.CurrentPage);
}
Refresh() {
    this.loader.ShowLoader();
    this.PModel.VisibleColumnInfo = "Amount#Amount,Notes#Notes,PaymentMethod#PaymentMethod";
    this._PaymentService
        .GetPaymentList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.PModel.TotalRecord = m.TotalRecord;
            this.PaymentList = m.DataList;
            this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
            this.loader.HideLoader();
        });
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

AddRecord(id: string) {
    if (id != "0") {
        this.loader.ShowLoader();
        this._router.navigate(['home/Payment/savepayment']);
    }
    this.Id = id;
    this.IsList = false;
    this._router.navigate(['home/Payment/savepayment'], { queryParams: { id: this.Id } });
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
        this._PaymentService.Delete(id).then(m => {

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