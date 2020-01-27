import { Component, Input, ViewEncapsulation } from '@angular/core';
import { DataService, RequestJson } from './data.service';

@Component({
  selector: 'app-root',
  template: `
  <div style="display:inline" data-Selector [json]=item *ngFor="let item of dataService.json.List; trackBy trackBy"></div>  
  `,
  styleUrls: ['./app.component.scss'],
  encapsulation: ViewEncapsulation.None // Prevent html 5 invalid attributes like "_nghost-sc0", "_ngcontent-sc0"
})
export class AppComponent {
  title = 'application';

  constructor(public dataService: DataService){
  }

  trackBy(index: any, item: any) {
    return item.TrackBy;
  }
}

/* Selector */
@Component({
  selector: '[data-Selector]',
  template: `
  <div data-Button style="display:inline" *ngIf="json.Type=='Button'" [json]=json></div>
  <div data-Div [ngClass]="json.CssClass" *ngIf="json.Type=='Div'" [json]=json></div>
  <div data-DivContainer [ngClass]="json.CssClass" *ngIf="json.Type=='DivContainer'" [json]=json></div>
  <div data-BootstrapNavbar [ngClass]="json.CssClass" *ngIf="json.Type=='BootstrapNavbar'" [json]=json></div>
  <div data-Grid2 [ngClass]="json.CssClass" *ngIf="json.Type=='Grid2' && !json.IsHide" [json]=json></div>
  <div data-Page [ngClass]="json.CssClass" *ngIf="json.Type=='Page' && !json.IsHide" [json]=json></div>
  <div data-Html style="display:inline" *ngIf="json.Type=='Html'" [json]=json></div>
  `
})
export class Selector {
  @Input() json: any
}

/* Page */
@Component({
  selector: '[data-Page]',
  template: `
  <div style="display:inline" data-Selector [json]=item *ngFor="let item of json.List; trackBy trackBy"></div>
  `
})
export class Page {
  @Input() json: any
  dataService: DataService;

  trackBy(index: any, item: any) {
    return item.TrackBy;
  }
}

/* Html */
@Component({
  selector: '[data-Html]',
  template: `<div style="display:inline" [ngClass]="json.CssClass" [innerHtml]="json.TextHtml"></div>`
})
export class Html {
  @Input() json: any
}

/* Button */
@Component({
  selector: '[data-Button]',
  template: `
  <button [ngClass]="json.CssClass" (click)="click();" [innerHtml]="json.TextHtml"></button>
  <i *ngIf="json.IsShowSpinner" class="fas fa-spinner fa-spin"></i>  
  `
})
export class Button {
  constructor(dataService: DataService){
    this.dataService = dataService;
  }

  @Input() json: any
  dataService: DataService;

  click(){
    this.json.IsShowSpinner = true;
    this.dataService.update(<RequestJson> { Command: 1, ComponentId: this.json.Id });
  } 
}

/* Div */
@Component({
  selector: '[data-Div]',
  template: `
  <div style="display:inline" data-Selector [json]=item *ngFor="let item of json.List; trackBy trackBy"></div>
  `
})
export class Div {
  @Input() json: any
  dataService: DataService;

  trackBy(index: any, item: any) {
    return item.TrackBy;
  }
}

/* DivContainer */
@Component({
  selector: '[data-DivContainer]',
  template: `
  <div [ngClass]="item.CssClass" data-Div [json]=item *ngFor="let item of json.List; trackBy trackBy"></div>
  `
})
export class DivContainer {
  @Input() json: any
  
  trackBy(index, item) {
    return index; // or item.id
  }
}
  