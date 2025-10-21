import { Routes } from '@angular/router';

export const ORDEM_SERVICO_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./components/ordem-servico-list/ordem-servico-list.component').then(m => m.OrdemServicoListComponent)
  },
  {
    path: 'novo',
    loadComponent: () => import('./components/ordem-servico-form/ordem-servico-form.component').then(m => m.OrdemServicoFormComponent)
  },
  {
    path: 'editar/:id',
    loadComponent: () => import('./components/ordem-servico-form/ordem-servico-form.component').then(m => m.OrdemServicoFormComponent)
  }
];
