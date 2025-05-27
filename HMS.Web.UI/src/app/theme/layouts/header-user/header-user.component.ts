import { Component, OnInit, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { CommonService } from '../../../CommonService/CommonService';
//declare let mLayout: any;

@Component({
    selector: 'app-header-user',
    templateUrl: "./header-user.component.html",
    encapsulation: ViewEncapsulation.None,
})

export class HeaderUserComponent implements OnInit {
    public PayrollRegion: string;
    constructor(public commonservice: CommonService) {
        this.commonservice.IsPayrollRegion();
        this.PayrollRegion = this.commonservice.getPayrollRegion();
    }
    ngOnInit() {

    }
    //  ngAfterViewInit() {

    //    //mLayout.initHeader();

    //}

}
