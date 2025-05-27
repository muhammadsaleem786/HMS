import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeComponent } from '../theme/theme.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
//import { MatStepperModule, MatFormFieldModule, MatInputModule, MatCheckboxModule, MatSelectModule, MatIconModule, MatDialogModule, MatTableModule, MatPaginatorModule, MatSortModule } from '@angular/material';
import { NgxEchartsModule } from 'ngx-echarts';
//import { MultiselectDropdownModule } from '../AngularElement/multiselect-dropdown';
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
import { NgxPrintModule } from 'ngx-print';
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
        NgxEchartsModule        
    ],
    declarations: [
        ThemeComponent,
        FileUploadComponent,
        ViewerComponent,
        PaginationComponent,
        PhoneMaskDirective,
        DatetimePickerComponent,
        DialogboxComponent,
    ],
    exports: [
        RouterModule,
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        LayoutModule,
        MatStepperModule,
        MatFormFieldModule,
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
        ThemeComponent,
        FileUploadComponent,
        ViewerComponent,
        PaginationComponent,
        PhoneMaskDirective,
        DatetimePickerComponent,
        DialogboxComponent,
        NgxPrintModule
    ],
    providers: [
       
    ]
})
export class CommonUtilsModule {
   
}
