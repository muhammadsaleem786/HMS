// theme.module.ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ThemeComponent } from './theme.component';
import { LayoutModule } from './layouts/layout.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
@NgModule({
    declarations: [ThemeComponent],
    imports: [
        CommonModule,
        FormsModule,         
        ReactiveFormsModule,
        LayoutModule,
        RouterModule.forChild([
            {
                path: '',
                component: ThemeComponent,
                children: [
                    {
                        path: '',
                        loadChildren: './theme-main.module#ThemeMainModule'
                    },
                    {
                        path: '',
                        loadChildren: './theme-setting.module#ThemeSettingModule'
                    },
                    {
                        path: '',
                        loadChildren: '../theme/Module/HTMLReports/report.module#ReportModule'
                    }
                ]
            }
        ])
    ]
})
export class ThemeModule { }
