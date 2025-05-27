import { Component, Injectable } from '@angular/core';

@Injectable()
export class LoaderService {

    private Loading: boolean = false;

    IsLoading(): boolean {
        return this.Loading;
    }

    ShowLoader(): void {
        this.Loading = true;
    }

    HideLoader(): void {
        this.Loading = false;
    }
}

