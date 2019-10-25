import { Component, ViewEncapsulation } from '@angular/core';
import { tap, map, filter, debounceTime, distinct, flatMap, switchMap } from 'rxjs/operators';
import { BehaviorSubject, fromEvent, merge, Observable } from 'rxjs';
import { ObservationFeedDto } from '@app/_models/ObservationFeedDto';
import { ObservationsFeedService } from '@app/_services/observations-feed.service';
import { ErrorReportViewModel } from '@app/_models/ErrorReportViewModel';
import { ObservationViewModel } from '@app/_models/ObservationViewModel';
import { ObservationFeedFilter } from '@app/_models/ObservationFeedFilter';
import * as _ from 'lodash';

@Component({
  selector: 'app-infitite-scroll-test',
  templateUrl: './infitite-scroll-test.component.html',
  styleUrls: ['./infitite-scroll-test.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class InfititeScrollTestComponent {
  // filterOptions = ObservationFeedFilter;
  currentFilter: ObservationFeedFilter = 0;

  private displayMessage: boolean;
  private message: string;
  private allLoaded = false;
  private cache = [];
  private pageByManual$ = new BehaviorSubject(1);
  private itemHeight = 145;
  private numberOfItems = 10;
  private pageByScroll$ = fromEvent(window, 'scroll')
    .pipe(
      map(() => window.scrollY),
      filter(current => current >= document.body.clientHeight - window.innerHeight),
      debounceTime(200),
      distinct(),
      map(y => Math.ceil((y + window.innerHeight) / (this.itemHeight * this.numberOfItems)))
    );

  private pageByResize$ = fromEvent(window, 'resize')
    .pipe(
      debounceTime(200),
      map(_ => Math.ceil(
        (window.innerHeight + document.body.scrollTop) / (this.itemHeight * this.numberOfItems)
      ))
    );

  private pageToLoad$ = merge(this.pageByManual$, this.pageByScroll$, this.pageByResize$)
    .pipe(
      distinct(),
      filter(page => this.cache[page - 1] === undefined)
    );

  loading = false;


  itemResults$: Observable<ObservationViewModel[]> = this.pageToLoad$
    .pipe(
      tap(_ => this.loading = true),

      flatMap((page: number) => {

        return this.observationsFeedService.getObservationsFeed(page, this.currentFilter)
          .pipe(
            tap((resp: ObservationFeedDto) => {
              if (page === Math.ceil(<number>resp.totalItems / <number>this.numberOfItems)) { this.allLoaded = true; }
              // console.log(resp.returnFilter);
              if (this.currentFilter != resp.returnFilter) {
                console.log(`not equal - requested: ${ ObservationFeedFilter[this.currentFilter] };
                 returned: ${ ObservationFeedFilter[resp.returnFilter] }`);
                this.currentFilter = resp.returnFilter;
              }
              // this.displayMessage = resp.displayMessage;
              // this.message = resp.message;
            },
              (error: ErrorReportViewModel) => {
                // this.router.navigate(['/page-not-found']);
              }),
            map((resp: any) => resp.items), // resp.results),
            tap(resp => {
              this.cache[page - 1] = resp;
              if ((this.itemHeight * this.numberOfItems * page) < window.innerHeight) {
                this.pageByManual$.next(page + 1);
              }
            }),
          );
      }),
      map(() => _.flatMap(this.cache))
    );


  constructor(private observationsFeedService: ObservationsFeedService) { }

  getMessage(requested: ObservationFeedFilter, returned: ObservationFeedFilter): string {
    let message = '';
    if (requested === 0) { message = message + `There are no observations in your ${ ObservationFeedFilter[requested] }.  `; }
    if (requested === 1) { message = message + `You have not recorded any observations yet.  `; }

    message = message + `Your feed is showing the latest ${ ObservationFeedFilter[returned] } observations instead...`;

    return message;
  }

  onFilterFeed(): void {
    this.cache = [];
    this.allLoaded = false;

    this.itemResults$ = this.pageToLoad$
      .pipe(
        tap(_ => this.loading = true),
        switchMap((page: number) => {

          return this.observationsFeedService.getObservationsFeed(page, this.currentFilter)
            .pipe(
              tap((resp: ObservationFeedDto) => {
                if (page === Math.ceil(<number>resp.totalItems / <number>this.numberOfItems)) { this.allLoaded = true; }
                if (this.currentFilter != resp.returnFilter) {
                  alert(this.getMessage(this.currentFilter, resp.returnFilter));

                  // console.log(`not equal - requested: ${ ObservationFeedFilter[this.currentFilter] };
                  //  returned: ${ ObservationFeedFilter[resp.returnFilter] }`);
                  this.currentFilter = resp.returnFilter;
                }
                // alert(ObservationFeedFilter[this.currentFilter]);
                // if (this.currentFilter === ObservationFeedFilter.Own)
                // {
                //   alert(<any>(this.currentFilter));
                // }
                // this.displayMessage = resp.displayMessage;
                // this.message = resp.message;
              },
                (error: ErrorReportViewModel) => {
                  // this.router.navigate(['/page-not-found']);
                }),
              map((resp: any) => resp.items),
              tap(resp => {
                this.cache[page - 1] = resp;
                if ((this.itemHeight * this.numberOfItems * page) < window.innerHeight) {
                  this.pageByManual$.next(page + 1);
                }
              }),
            );
        }),
        map(() => _.flatMap(this.cache))
      );
  }
}
