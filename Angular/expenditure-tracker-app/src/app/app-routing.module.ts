import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth-guard/auth.guard';

const routes: Routes = [
  {
    // Lazy Loading the public module (all children routes will be under '/public/{route from lazy loaded module}')
    path: 'public',
    loadChildren: () => import('./public/public.module').then(m => m.PublicModule)
  },
  {
    // Lazy Loading the protected module (all children routes will be under '/home/{route from lazy loaded module}')
    // The guard will check if the user is having a jwt, otherwise he will be redirected to the base route
    path: 'home',
    loadChildren: () => import('./home/home.module').then(m => m.HomeModule)
  },
  {
    // Redirects all paths that are not matching to the 'public' route/path
    path: '**',
    redirectTo: 'public/login',
    pathMatch: 'full'
  }
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
