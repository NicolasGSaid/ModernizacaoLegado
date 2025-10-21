import { Routes } from '@angular/router';

export const CLIENTE_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./components/cliente-list/cliente-list.component').then(m => m.ClienteListComponent)
  },
  {
    path: 'new',
    loadComponent: () => import('./components/cliente-form/cliente-form.component').then(m => m.ClienteFormComponent)
  },
  {
    path: 'edit/:id',
    loadComponent: () => import('./components/cliente-form/cliente-form.component').then(m => m.ClienteFormComponent)
  }
];
