import { Routes } from '@angular/router';

export const TECNICO_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./components/tecnico-list/tecnico-list.component').then(m => m.TecnicoListComponent)
  },
  {
    path: 'new',
    loadComponent: () => import('./components/tecnico-form/tecnico-form.component').then(m => m.TecnicoFormComponent)
  },
  {
    path: 'edit/:id',
    loadComponent: () => import('./components/tecnico-form/tecnico-form.component').then(m => m.TecnicoFormComponent)
  }
];
