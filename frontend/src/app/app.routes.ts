import { Routes } from '@angular/router';
import { Home } from './home/home';
import { Transcript } from './transcript/transcript';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'transcript', component: Transcript }
];
