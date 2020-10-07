import { Component, OnInit, ViewChild, Input, ElementRef } from '@angular/core';
import { DataService, CommandJson } from '../data.service';

/* Grid */
@Component({
  selector: '[data-Grid]',
  templateUrl: './grid.component.html',
  styles: [
  ]
})
export class GridComponent implements OnInit {

  constructor(private dataService: DataService) { }

  ngOnInit(): void {
  }

  @Input() json: any;

  @ViewChild('inputFileUpload', {static: false}) inputFileUpload: ElementRef<HTMLElement>;

  ngModelChange(cell) {
    cell.IsShowSpinner = true;
    this.dataService.update(<CommandJson> { CommandEnum: 9, ComponentId: this.json.Id, GridCellId: cell.Id, GridCellText: cell.Text });
  }

  clickSort(cell, event: MouseEvent) {
    cell.IsShowSpinner = true;
    this.dataService.update(<CommandJson> { CommandEnum: 8, ComponentId: this.json.Id, GridCellId: cell.Id });
  }

  clickConfig(cell, event: MouseEvent) {
    event.stopPropagation(); // Prevent underlying Grid to fire click event. (Lookup grid)
    cell.IsShowSpinner = true;
    this.dataService.update(<CommandJson> { CommandEnum: 12, ComponentId: this.json.Id, GridCellId: cell.Id });
  }

  private cellFileUpload: any;

  clickFileUpload(cell, event: MouseEvent) {
    event.stopPropagation(); // Prevent underlying Grid to fire click event. (Lookup grid)
    this.cellFileUpload = cell;
    this.inputFileUpload.nativeElement.click();
  }

  changeInputFileUpload(files: FileList) {
    const file = files.item(0);

    const cellFileUpload = this.cellFileUpload;
    const dataService = this.dataService;
    const json = this.json;

    var reader = new FileReader();
    reader.readAsDataURL(file.slice()); 
    reader.onloadend = function() {
        var base64data = reader.result;                
        console.log(base64data);
        cellFileUpload.IsShowSpinner = true;
        dataService.update(<CommandJson> { CommandEnum: 9, ComponentId: json.Id, GridCellId: cellFileUpload.Id, GridCellText: cellFileUpload.Text, GridCellTextBase64: base64data, GridCellTextBase64FileName: file.name });
    }
  }

  clickCell(cell, event: MouseEvent) {
    if (!(event.target instanceof HTMLInputElement)) {
      this.focus(cell);
    }
  }

  focus(cell) {
    console.log("F");
    if (!cell.IsSelect) {
      cell.IsShowSpinner = true;
      this.dataService.update(<CommandJson> { CommandEnum: 11, ComponentId: this.json.Id, GridCellId: cell.Id });
    }
  }

  focusout(cell) {
    if (cell.TextLeave != null) {
      if (cell.Text != cell.TextLeave) {
        cell.Text = cell.TextLeave;
        cell.TextLeave = null;
        cell.IsShowSpinner = true;
        this.dataService.update(<CommandJson> { CommandEnum: 13, ComponentId: this.json.Id, GridCellId: cell.Id });
      }
    }
  }

  styleColumnList(): string[] {
    let result: string[] = [];
    this.json.CellList.forEach(cell => {
      if (cell.CellEnum == 4){
        if (cell.Width == null) {
          result.push("minmax(0, 1fr)");
        }
        else {
          result.push(cell.Width);
        }
      }
    });
    return result;
  }

  resizeColumnIndex: number; // If not null, user column resize in progress
  resizeColumnWidthValue: number;
  resizePageX: number;
  resizeTableWidth: number;

  click(event: MouseEvent) {
    event.stopPropagation(); // Prevent sort after column resize
  }

  mouseDown(columnIndex, event: MouseEvent): boolean {
    event.stopPropagation();
    this.resizeColumnIndex = columnIndex;
    this.resizePageX = event.pageX;
    this.resizeColumnWidthValue = null;
    this.resizeTableWidth = (<HTMLElement>event.currentTarget).parentElement.parentElement.parentElement.parentElement.clientWidth;
    return false;
  }    

  documentMouseMove(event: MouseEvent) {
    if (this.resizeColumnIndex != null) {
      let styleColumn = this.json.StyleColumnList[this.resizeColumnIndex];
      let widthValue = styleColumn.WidthValue;
      if (this.resizeColumnWidthValue == null) {
        this.resizeColumnWidthValue = widthValue;
      }
      let offset = event.pageX - this.resizePageX;
      let offsetPercent = (offset / this.resizeTableWidth) * 100;
      let columnWidthNew = Math.round((this.resizeColumnWidthValue + offsetPercent) * 100) / 100;
      if (columnWidthNew < 0) {
        columnWidthNew = 0;
      }
      
      // ColumnWidthTotal
      let columnWidthTotal = 0;
      for (let i = 0; i < this.json.StyleColumnList.length; i++) {
        let widthValue = this.json.StyleColumnList[i].WidthValue;
        if (i != this.resizeColumnIndex && widthValue != null) {
          columnWidthTotal += widthValue;
        }
      }
      if (columnWidthTotal + columnWidthNew > 100) {
        columnWidthNew = 100 - columnWidthTotal;
      }

      widthValue = columnWidthNew;
      styleColumn.Width = widthValue + styleColumn.WidthUnit;
      styleColumn.WidthValue = widthValue;
    }
  }

  documentMouseUp(event: MouseEvent) {
    if (this.resizeColumnIndex != null) {
      event.stopPropagation();
      let resizeColumnIndexLocal = this.resizeColumnIndex;
      this.resizeColumnIndex = null;
      let widthValue = <number>this.json.StyleColumnList[resizeColumnIndexLocal].WidthValue;
      this.dataService.update(<CommandJson> { CommandEnum: 20, ComponentId: this.json.Id, ResizeColumnIndex: resizeColumnIndexLocal, ResizeColumnWidthValue: widthValue });
    }
  }

  clickGrid(isClickEnum, event: MouseEvent) {
    event.stopPropagation(); // Prevent underlying Grid to fire click event. (Lookup grid)
    this.json.IsShowSpinner = true;
    this.dataService.update(<CommandJson> { CommandEnum: 10, ComponentId: this.json.Id, GridIsClickEnum: isClickEnum });
  }

  trackBy(index: any, item: any) {
    return index;
  }
}