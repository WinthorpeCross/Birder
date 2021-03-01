import { Component, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ErrorReportViewModel } from '@app/_models/ErrorReportViewModel';
import { NetworkUserViewModel } from '@app/_models/UserProfileViewModel';
import { NetworkService } from '@app/_services/network.service';

@Component({
  selector: 'app-following',
  templateUrl: './following.component.html',
  styleUrls: ['./following.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class FollowingComponent {
  username: string;
  requesting: boolean;
  following: NetworkUserViewModel[];

  constructor(private route: ActivatedRoute
    , private networkService: NetworkService) {
    route.params.subscribe(_ => {
      this.route.paramMap.subscribe(pmap => {
        this.username = pmap.get('username');
        this.getFollowing();
      })
    });
  }

  getFollowing(): void {
    this.requesting = true;
    this.networkService.getFollowing(this.username)
      .subscribe(
        (data: NetworkUserViewModel[]) => {
          this.following = data;
        },
        (error: ErrorReportViewModel) => {
          console.log(error);
        },
        () => this.requesting = false
      );
  }
}