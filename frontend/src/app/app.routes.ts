import { Routes } from '@angular/router';
import { Home } from './home/home';
import {Auth} from './auth/auth';
import { Transcript } from './transcript/transcript';
import { AuthGuard } from './auth/auth.guard';

export const routes: Routes = [
  { path: '', component: Auth },
  { path: 'transcript', component: Transcript, canActivate: [AuthGuard] }
];
