import { Component, ViewEncapsulation, OnInit, ChangeDetectorRef, ViewChild, AfterViewInit } from '@angular/core';
import { ObservationViewModel } from '@app/_models/ObservationViewModel';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { BirdSummaryViewModel } from '@app/_models/BirdSummaryViewModel';
import { ParentErrorStateMatcher } from 'validators';
import { ErrorReportViewModel } from '@app/_models/ErrorReportViewModel';
import { Router, ActivatedRoute } from '@angular/router';
import { ObservationService } from '@app/_sharedServices/observation.service';
import { BirdsService } from '@app/_services/birds.service';
import { GeocodeService } from '@app/_services/geocode.service';
import { LocationViewModel } from '@app/_models/LocationViewModel';
import { TokenService } from '@app/_services/token.service';
import { ToastrService } from 'ngx-toastr';
import { Location } from '@angular/common';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { GoogleMap, MapInfoWindow, MapMarker } from '@angular/google-maps';
import { ObservationPosition } from '@app/_models/ObservationPosition';
import { ViewEditSingleMarkerMapComponent } from '@app/_maps/view-edit-single-marker-map/view-edit-single-marker-map.component';

@Component({
  selector: 'app-observation-edit',
  templateUrl: './observation-edit.component.html',
  styleUrls: ['./observation-edit.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ObservationEditComponent implements OnInit  {
  @ViewChild(ViewEditSingleMarkerMapComponent)
  private timerComponent: ViewEditSingleMarkerMapComponent;

  requesting: boolean;
  observation: ObservationViewModel;
  editObservationForm: FormGroup;
  birdsSpecies: BirdSummaryViewModel[];
  parentErrorStateMatcher = new ParentErrorStateMatcher();
  errorReport: ErrorReportViewModel;
  // geolocation: string;
  searchAddress = '';
  // geoError: string;
  // childLoaded = false;

  filteredOptions: Observable<BirdSummaryViewModel[]>;

  editObservation_validation_messages = {
    'quantity': [
      { type: 'required', message: 'Quantity is required' }
    ],
    'bird': [
      { type: 'required', message: 'The observed species is required' }
    ]
  };

  // @ViewChild(GoogleMap, { static: false }) map: GoogleMap
  // // @ViewChild(MapMarker, { static: false }) mark: MapMarker
  // @ViewChild(MapInfoWindow, { static: false }) infoWindow: MapInfoWindow
  // zoom = 11;
  // options: google.maps.MapOptions = {
  //   mapTypeId: 'terrain'
  // }

  constructor(private router: Router
    , private toast: ToastrService
    , private route: ActivatedRoute
    , private observationService: ObservationService
    , private tokenService: TokenService
    , private birdsService: BirdsService
    , private formBuilder: FormBuilder
    , private location: Location
    , private geocodeService: GeocodeService
    , private ref: ChangeDetectorRef) { }

  ngOnInit() {
    this.getObservation();
    // this.getBirds();
  }

  test() {
    alert(this.timerComponent.locationMarker.position.lat);
  }

  // ngAfterViewInit() {
  //   // Redefine `seconds()` to get from the `CountdownTimerComponent.seconds` ...
  //   // but wait a tick first to avoid one-time devMode
  //   // unidirectional-data-flow-violation error
  //   // setTimeout(() => this.seconds = () => this.timerComponent.seconds, 0);
  //   // alert(this.timerComponent.locationMarker.position.lat);
  //   this.childLoaded = true;
  // }

  displayFn(bird: BirdSummaryViewModel): string {
    return bird && bird.englishName ? bird.englishName : null;
  }


  getBirdAutocompleteOptions() {
    this.filteredOptions = this.editObservationForm.controls['bird'].valueChanges.pipe(
      startWith(''),
      map(value => value.length >= 1 ? this._filter(value) : this.birdsSpecies)
    );
  }

  private _filter(value: string): BirdSummaryViewModel[] {
    const filterValue = value.toLowerCase();
    return this.birdsSpecies.filter(option => option.englishName.toLowerCase().indexOf(filterValue) === 0);
  }

  createForms(): void {
    this.editObservationForm = this.formBuilder.group({
      observationId: new FormControl(this.observation.observationId),
      quantity: new FormControl(this.observation.quantity, Validators.compose([
        Validators.required
      ])),
      bird: new FormControl(this.observation.bird, Validators.compose([
        Validators.required
      ])),
      observationDateTime: new FormControl(this.observation.observationDateTime, Validators.compose([
        Validators.required
      ])),
      // noteGeneral: new FormControl(this.observation.noteGeneral),
      // noteHabitat: new FormControl(this.observation.noteHabitat),
      // noteWeather: new FormControl(this.observation.noteWeather),
      // noteAppearance: new FormControl(this.observation.noteAppearance),
      // noteBehaviour: new FormControl(this.observation.noteBehaviour),
      // noteVocalisation: new FormControl(this.observation.noteVocalisation),
    });
  }

  onSubmit(value: ObservationViewModel): void {
    this.requesting = true;

    // const position = <ObservationPosition>{
    //   latitude: this.marker.position.lat,
    //   longitude: this.marker.position.lng,
    //   formattedAddress: this.geolocation
    // }

    const observation = <ObservationViewModel>{
      quantity: value.quantity,
      observationDateTime: value.observationDateTime,
      bird: value.bird,
      birdId: value.bird.birdId,
      // noteAppearance: value.noteAppearance,
      // noteBehaviour: value.noteAppearance,
      // noteGeneral: value.noteGeneral,
      // noteHabitat: value.noteHabitat,
      // noteVocalisation: value.noteVocalisation,
      // noteWeather: value.noteWeather,
      notes: null,
      observationId: this.observation.observationId,
      user: this.observation.user,
      creationDate: this.observation.creationDate,
      hasPhotos: false, // might have a problem
      //
      position: null,

      lastUpdateDate: new Date().toISOString()
    }

    this.observationService.updateObservation(this.observation.observationId, observation)
      .subscribe(
        (data: ObservationViewModel) => {
          //this.editObservationForm.reset();
          this.router.navigate(['/observation-detail/' + data.observationId.toString()]);
        },
        (error: ErrorReportViewModel) => {
          this.requesting = false;
          this.errorReport = error;
          console.log(error.friendlyMessage);
          console.log('unsuccessful add observation');
        }
      );
  }

  goBack(): void {
    this.location.back();
  }

  getObservation(): void {
    const id = +this.route.snapshot.paramMap.get('id');

    this.observationService.getObservation(id)
      .subscribe(
        (observation: ObservationViewModel) => {
          this.observation = observation;
          if (this.tokenService.checkIsRecordOwner(observation.user.userName) === false) {
            this.toast.error(`Only the observation owner can edit their report`, `Not allowed`);
            this.router.navigate(['/observation-feed']);
            return;
          }
          this.createForms();
          // this.addMarker(observation.position.latitude, observation.position.longitude);
          this.getBirds();
        },
        (error: ErrorReportViewModel) => {
          this.errorReport = error;
          // this.router.navigate(['/page-not-found']);  // TODO: this is right for typing bad param, but what about server error?
        });
  }

  getBirds(): void {
    this.birdsService.getBirdsDdl()
      .subscribe(
        (data: BirdSummaryViewModel[]) => {
          this.birdsSpecies = data;
          this.getBirdAutocompleteOptions();
        },
        (error: ErrorReportViewModel) => {
          console.log('could not get the birds ddl');
        });
  }

  // marker; // make marker a property?
  // addMarker(latitude: number, longitude: number) {
  //   this.marker = ({
  //     position: {
  //       lat: latitude,
  //       lng: longitude
  //     },
  //     label: {
  //       color: 'red',
  //       text: 'Marker label',
  //     },
  //     title: 'Marker title',
  //     options: { draggable: true },
  //   })

  //   // this.getGeolocation(latitude, longitude);
  // }

  // openInfoWindow(marker: MapMarker) {
  //   this.infoWindow.open(marker);
  // }

  // markerChanged(event: google.maps.MouseEvent): void {
  //   this.addMarker(event.latLng.lat(), event.latLng.lng());
  // }


  // getGeolocation(latitude: number, longitude: number): void {
  //   this.geocodeService.reverseGeocode(latitude, longitude)
  //     .subscribe(
  //       (data: LocationViewModel) => {
  //         this.geolocation = data.formattedAddress;
  //         this.ref.detectChanges();
  //       },
  //       (error: any) => {
  //         //
  //       }
  //     );
  // }

  // useGeolocation(searchValue: string) {
  //   this.geocodeService.geocodeAddress(searchValue)
  //     .subscribe((location: LocationViewModel) => {
  //       // this.editObservationForm.get('locationLatitude').setValue(location.latitude);
  //       // this.editObservationForm.get('locationLongitude').setValue(location.longitude);
  //       // this.geolocation = location.formattedAddress;
  //       this.addMarker(location.latitude, location.longitude);
  //       this.searchAddress = '';
  //       this.ref.detectChanges();
  //     }
  //     );
  // }

  // closeAlert() {
  //   this.geoError = null;
  // }

  // getCurrentPosition() {
  //   if (window.navigator.geolocation) {
  //     window.navigator.geolocation.getCurrentPosition(
  //       (position) => {
  //         this.useGeolocation(position.coords.latitude.toString() + ',' + position.coords.longitude.toString());
  //       }, (error) => {
  //         switch (error.code) {
  //           case 3: // ...deal with timeout
  //             this.geoError = 'The request to get user location timed out...';
  //             break;
  //           case 2: // ...device can't get data
  //             this.geoError = 'Location information is unavailable...';
  //             break;
  //           case 1: // ...user said no ☹️
  //             this.geoError = 'User denied the request for Geolocation...';
  //             break;
  //           default:
  //             this.geoError = 'An error occurred with Geolocation...';
  //         }
  //       });
  //   } else {
  //     this.geoError = 'Geolocation not supported in this browser';
  //   }
  // }
}
