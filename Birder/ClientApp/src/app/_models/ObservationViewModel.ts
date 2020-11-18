import { UserViewModel } from './UserViewModel';
import { BirdSummaryViewModel } from './BirdSummaryViewModel';
import { ObservationPosition } from './ObservationPosition';

export interface ObservationViewModel {
   [x: string]: any;
    observationId: number;
    // locationLatitude: number;
    // locationLongitude: number;
    quantity: number;
    noteGeneral: string;
    noteHabitat: string;
    noteWeather: string;
    noteAppearance: string;
    noteBehaviour: string;
    noteVocalisation: string;
    hasPhotos: boolean;
    // SelectedPrivacyLevel: PrivacyLevel;
    observationDateTime: Date | string;
    creationDate: Date | string;
    lastUpdateDate: Date | string;
    birdId: number;
    bird: BirdSummaryViewModel;
    user: UserViewModel;
    position: ObservationPosition;
    // ObservationTags: ObservationTag[];
}
