import { Component, Injectable } from '@angular/core';

@Injectable()
export class AsidebarService {
    private _MenuId: number = 1;
    private _SelectId: string = "";
    private _IntervalId: number = 0;
    private _accountId: any = [];
    MenuId(): number {
        return this._MenuId;
    }
    IntervalId(): number {
        return this._IntervalId;
    }
    SetMenuId(id: number): void {
        this._MenuId = id;
    }
    HomeMenu(): void {
        this._MenuId = 1;
    }
    SetIntervalId(id: any): void {
        this._IntervalId = id;
    }
    SetClickValue(val: string) {
        this._SelectId = val;
    }
    GetClickValue(): string {
        return this._SelectId;
    }
    SetSaleHoldValue(val: string) {
        this._SelectId = val;
    }
    GetSaleHoldValue(): string {
        return this._SelectId;
    }
   
}

