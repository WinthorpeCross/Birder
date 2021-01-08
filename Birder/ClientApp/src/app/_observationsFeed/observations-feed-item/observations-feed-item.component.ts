import { Component, Input, ViewEncapsulation } from '@angular/core';
import { ObservationFeedDto } from '@app/_models/ObservationFeedDto';
import { UserViewModel } from '@app/_models/UserViewModel';

@Component({
  selector: 'app-observations-feed-item',
  templateUrl: './observations-feed-item.component.html',
  styleUrls: ['./observations-feed-item.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ObservationsFeedItemComponent {
  @Input() observation: ObservationFeedDto;
  @Input() user: UserViewModel;

  constructor() { }
}