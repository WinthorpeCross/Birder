import { Component, OnInit } from '@angular/core';
import { PageEvent } from '@angular/material';

@Component({
  selector: 'app-birds-list',
  templateUrl: './birds-list.component.html',
  styleUrls: ['./birds-list.component.css']
})
export class BirdsListComponent implements OnInit {
    // MatPaginator Inputs
    length = 100;
    pageSize = 10;
    pageSizeOptions: number[] = [5, 10, 25, 100];
  
    // MatPaginator Output
    // pageEvent: PageEvent;
  
    // setPageSizeOptions(setPageSizeOptionsInput: string) {
    //   this.pageSizeOptions = setPageSizeOptionsInput.split(',').map(str => +str);
    // }

  constructor() { }

  ngOnInit() {
  }

}
