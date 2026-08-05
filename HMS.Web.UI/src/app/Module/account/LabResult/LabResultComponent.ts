import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Input, TemplateRef } from '@angular/core';
import { lab_result } from './LabResultModel';
import { FormGroup, FormControl, FormBuilder, Validators, FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LoaderService } from '../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../CommonService/CommonToastrService';
import { LabResultService } from './LabResultService';
import { GlobalVariable } from '../../../AngularConfig/global';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { EncryptionService } from '../../../CommonService/encryption.service';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
@Component({
    selector: 'app-lab-result',
    templateUrl: './LabResultComponent.html',
    styleUrls: ['./LabResultComponent.css'],
    providers: [LabResultService],
})
export class LabResultComponent implements OnInit {
    public model = new lab_result();
    public PayrollRegion: string;
    public ReportList: any[] = [];
    public ReportModel: any;
    public IsEdit: boolean = false;
    public Form1: FormGroup;
    captchaToken: string;
    SelectedReport: any;
    siteKey = '6Lcd9j0tAAAAACd0nNQ2ZD6zDUYytxtwch07s-aN';
    @ViewChild('ReportModal') ReportModal: TemplateRef<any>;
    private _reportModalRef: NgbModalRef;
    constructor(public _fb: FormBuilder, public _router: Router
        , public toastr: CommonToastrService, private modalService: NgbModal
        , public loader: LoaderService, private encrypt: EncryptionService, public _LabResultService: LabResultService) {
    }
    ngOnInit() {
        this.Form1 = this._fb.group({
            InvoiceNo: ['', [Validators.required]],
            AccessCode: ['', [Validators.required]]
        });
    }
    resolved(token: string) {
        this.captchaToken = token;
    }
    SearchReports(valid: boolean) {
        if (!valid) {
            return;
        }
        if (!this.captchaToken) {
            alert('Please complete the reCAPTCHA.');
            return;
        }
        
        this.ReportList = [];
        this.loader.ShowLoader();
        this._LabResultService.GetById(this.model.InvoiceNo, this.model.AccessCode).then(m => {
            if (m.IsSuccess) {
                this.ReportList = m.ResultSet.result;
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    ReportView(id: any) {
        
        this.ReportModel = null;
        if (id != null && id !== '') {
            this.loader.ShowLoader();
            this._LabResultService.ReportGenerate(id).then(m => {
                if (m.IsSuccess) {
                    this.ReportModel = m.ResultSet;
                    if (m.ResultSet.CollectionCenterImage != null)
                        this.ReportModel.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + m.ResultSet.CompanyID + '/' + m.ResultSet.CollectionCenterImage;
                    this.ReportModel.ViewDate = new Date();
                    this._reportModalRef = this.modalService.open(this.ReportModal, { size: 'lg', centered: true, windowClass: 'DashboardWraper' });
                    this.loader.HideLoader();
                } else
                    this.loader.HideLoader();
            });
        } else {
            this.toastr.Error('Error', 'Invalid invoice no.');
            return;
        }
    }
    public DownloadPDF(): void {
        this.loader.ShowLoader();
        var data = document.getElementById('printReport');
        html2canvas(data, {
            scale: 3,
            useCORS: true,
            scrollY: -window.scrollY
        }).then(canvas => {
            const pageWidth = 210; // A4 width in mm
            const margin = 10;     // 10mm margin
            const imgWidth = pageWidth - 2 * margin;
            const imgHeight = (canvas.height * imgWidth) / canvas.width;
            const pageHeight = imgHeight + 2 * margin; // auto page height 
            const contentDataURL = canvas.toDataURL('image/png')
            const pdf = new jsPDF('p', 'mm', [pageHeight, pageWidth]); // tall page
            var position = 0;
            pdf.addImage(contentDataURL, 'PNG', margin, margin, imgWidth, imgHeight);
            pdf.save('LabReport.pdf');
            this.loader.HideLoader();
        });
    }
    public ViewPdf(): void {
        this.loader.ShowLoader();
        var data = document.getElementById('printReport');
        html2canvas(data, {
            scale: 3,
            useCORS: true,
            scrollY: -window.scrollY
        }).then(canvas => {
            const pageWidth = 210; // A4 width in mm
            const margin = 10;     // 10mm margin
            const imgWidth = pageWidth - 2 * margin;
            const imgHeight = (canvas.height * imgWidth) / canvas.width;
            const pageHeight = imgHeight + 2 * margin; // auto page height 
            const contentDataURL = canvas.toDataURL('image/png')
            const pdf = new jsPDF('p', 'mm', [pageHeight, pageWidth]); // tall page
            var position = 0;
            pdf.addImage(contentDataURL, 'PNG', margin, margin, imgWidth, imgHeight);
            const blob = pdf.output('blob');
            const url = URL.createObjectURL(blob);
            window.open(url, '_blank');
            this.loader.HideLoader();
        });
    }
}
