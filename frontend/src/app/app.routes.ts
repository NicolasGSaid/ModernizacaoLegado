import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'ordem-servico',
    loadChildren: () => import('./features/ordem-servico/ordem-servico.routes').then(m => m.ORDEM_SERVICO_ROUTES)
  },
  {
    path: 'cliente',
    loadChildren: () => import('./features/cliente/cliente.routes').then(m => m.CLIENTE_ROUTES)
  },
  {
    path: 'tecnico',
    loadChildren: () => import('./features/tecnico/tecnico.routes').then(m => m.TECNICO_ROUTES)
  },
  {
    path: 'primeng-test',
    loadComponent: () => import('./shared/components/primeng-test/primeng-test.component').then(m => m.PrimengTestComponent)
  }
];
