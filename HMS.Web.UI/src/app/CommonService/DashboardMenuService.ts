import { Component, Injectable } from '@angular/core';

@Injectable()
export class DashboardMenuService {

    private DashboardMenu: boolean = false;

    IsShowMenus(): boolean {
        return this.DashboardMenu;
    }

    ShowMenues(): void {
        this.DashboardMenu = true;
    }

    HideMenues(): void {
        this.DashboardMenu = false;
    }
}

