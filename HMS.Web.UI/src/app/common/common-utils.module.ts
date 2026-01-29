//common-utils.modules.ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxEchartsModule } from 'ngx-echarts';
import { MultiselectDropdownModule } from 'angular-2-dropdown-multiselect';
import { MyDatePickerModule } from 'mydatepicker';
import { LayoutModule } from '../theme/layouts/layout.module';
import { RouterModule } from '@angular/router';
import { DatetimePickerComponent } from '../CommonComponent/DatetimePickerComponent/DatetimePickerComponent';
import { DialogboxComponent } from '../CommonComponent/DialogboxComponent/DialogboxComponent';
import { ViewerComponent } from '../CommonComponent/ReportViewer/ViewerComponent';
import { FileUploadComponent } from '../CommonComponent/FileUploadComponent/FileUploadComponent';
import { PaginationComponent } from '../CommonComponent/PaginationComponent';
import { PhoneMaskDirective } from '../CommonComponent/PhoneMaskDirective';
import { MatStepperModule } from '@angular/material';
import { MatInputModule, MatAutocompleteModule } from '@angular/material';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxPrintModule } from 'ngx-print';
import { FilterPipe } from '../angularfilter/filter.pipe'
@NgModule({
    imports: [
        MatAutocompleteModule,
       NgxPrintModule,
        RouterModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        LayoutModule,
        MatStepperModule,
        MatFormFieldModule,
        MatFormFieldModule,
        NgbModule,
        MatCheckboxModule,
        MatSelectModule,
        MatIconModule,
        MatDialogModule,
        MatTableModule,
        MatPaginatorModule,
        MatSortModule,
        MyDatePickerModule,
        MultiselectDropdownModule,
        NgxEchartsModule        
    ],
    declarations: [
        FileUploadComponent,
        ViewerComponent,
        PaginationComponent,
        PhoneMaskDirective,
        DatetimePickerComponent,
        DialogboxComponent,
        FilterPipe
    ],
    exports: [
        RouterModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        LayoutModule,
        MatStepperModule,
        MatFormFieldModule,
        NgbModule,
        MatInputModule,
        MatCheckboxModule,
        MatSelectModule,
        MatIconModule,
        MatDialogModule,
        MatTableModule,
        MatPaginatorModule,
        MatSortModule,
        MyDatePickerModule,
        MultiselectDropdownModule,
        NgxEchartsModule,
        FileUploadComponent,
        ViewerComponent,
        PaginationComponent,
        PhoneMaskDirective,
        DatetimePickerComponent,
        DialogboxComponent,
        NgxPrintModule,
        FilterPipe
    ],
    providers: [
       
    ]
})
export class CommonUtilsModule {
   
}
