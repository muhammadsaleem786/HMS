import { NgModule } from '@angular/core';
import { FooterComponent } from './footer/footer.component';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HeaderUserComponent } from './header-user/header-user.component';
import { AsidebarComponent } from './asidebar/asidebar.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
    declarations: [
        FooterComponent,
        HeaderUserComponent,
        AsidebarComponent,
    ],
    exports: [
        FooterComponent,
        HeaderUserComponent, AsidebarComponent
    ],
    imports: [
        CommonModule,
        RouterModule, FormsModule, ReactiveFormsModule
    ]
})
export class LayoutModule {
}