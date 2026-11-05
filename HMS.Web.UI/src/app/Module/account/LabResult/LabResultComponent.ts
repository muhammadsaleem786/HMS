import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Input, TemplateRef } from '@angular/core';
import { lab_result } from './LabResultModel';
import { FormGroup, FormControl, FormBuilder, Validators, FormsModule } from '@angular/forms';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
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
        , public toastr: CommonToastrService, private modalService: NgbModal, private sanitizer: DomSanitizer
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
    ReportView(id: any, CompanyId: any) { 
        this.ReportModel = null;
        if (id != null && id !== '') {
            this.loader.ShowLoader();
            this._LabResultService.ReportGenerate(id, CompanyId).then(m => {
                if (m.IsSuccess) {
                    this.ReportModel = m.ResultSet;
                    if (m.ResultSet.CollectionCenterImage != null)
                        this.ReportModel.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + m.ResultSet.CompanyId + '/' + m.ResultSet.CollectionCenterImage;
                    this.ReportModel.safeHeader = this.sanitizer.bypassSecurityTrustHtml(m.ResultSet.Header);
                    this.ReportModel.safeFooter = this.sanitizer.bypassSecurityTrustHtml(m.ResultSet.Footer);
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
    private GeneratePDF(download: boolean): void {
        this.loader.ShowLoader();
        const data = document.getElementById('printReport');
        if (!data) {
            this.loader.HideLoader();
            return;
        }
        html2canvas(data, {
            scale: 2,
            useCORS: true,
            allowTaint: false,
            backgroundColor: '#ffffff',
            scrollY: 0,
            scrollX: 0,
            windowWidth: data.scrollWidth,
            windowHeight: data.scrollHeight
        }).then(canvas => {
            const pdf = new jsPDF('p', 'mm', 'a4');
            const pageWidth = pdf.internal.pageSize.getWidth();
            const pageHeight = pdf.internal.pageSize.getHeight();
            const margin = 10;
            const imgWidth = pageWidth - (margin * 2);
            const imgHeight = (canvas.height * imgWidth) / canvas.width;
            const contentDataURL = canvas.toDataURL('image/png');
            let heightLeft = imgHeight;
            let position = margin;
            // First page
            pdf.addImage(
                contentDataURL,
                'PNG',
                margin,
                position,
                imgWidth,
                imgHeight
            );
            heightLeft -= (pageHeight - margin * 2);
            // Additional pages
            while (heightLeft > 0) {
                pdf.addPage();
                position = margin - (imgHeight - heightLeft);
                pdf.addImage(
                    contentDataURL,
                    'PNG',
                    margin,
                    position,
                    imgWidth,
                    imgHeight
                );
                heightLeft -= (pageHeight - margin * 2);
            }
            if (download) {
                pdf.save('LabReport.pdf');
            } else {
                const blob = pdf.output('blob');
                const url = URL.createObjectURL(blob);
                window.open(url, '_blank');
            }
            this.loader.HideLoader();
        }).catch(error => {
            console.error('PDF generation error:', error);
            this.loader.HideLoader();
        });
    }
    public DownloadPDF(): void {
        this.GeneratePDF(true);
    }
    public ViewPdf(): void {
        this.GeneratePDF(false);
    }
}
