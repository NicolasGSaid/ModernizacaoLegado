import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
// PrimeNG Imports
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';
import { Router } from '@angular/router';
import { OrdemServicoService } from '../../services/ordem-servico.service';
import { OrdemServico } from '../../models/ordem-servico.model';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner.component';
import { FabButtonComponent } from '../../../../shared/components/fab-button/fab-button.component';
import { StatusChipComponent } from '../../../../shared/components/status-chip/status-chip.component';
import { BreadcrumbComponent, BreadcrumbItem } from '../../../../shared/components/breadcrumb/breadcrumb.component';
import { NotificationService } from '../../../../core/services/notification.service';
import { ConfirmationDialogService } from '../../../../core/services/confirmation-dialog.service';

@Component({
  selector: 'app-ordem-servico-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    // Material (manter temporariamente)
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatChipsModule,
    MatTooltipModule,
    // PrimeNG
    TableModule,
    ButtonModule,
    TagModule,
    InputTextModule,
    TooltipModule,
    // Shared Components
    PaginationComponent,
    BreadcrumbComponent
  ],
  templateUrl: './ordem-servico-list.component.html',
  styleUrl: './ordem-servico-list.component.css'
})
export class OrdemServicoListComponent implements OnInit {
  private ordemServicoService = inject(OrdemServicoService);
  private notificationService = inject(NotificationService);
  private confirmationService = inject(ConfirmationDialogService);
  private router = inject(Router);

  // Signals para estado reativo
  ordensServico = signal<OrdemServico[]>([]);
  page = signal(1);
  pageSize = signal(10);
  totalItems = signal(0);
  filtro = signal('');
  isLoading = signal(false);

  // Breadcrumbs
  breadcrumbs: BreadcrumbItem[] = [
    { label: 'Ordens de Serviço', icon: 'list' }
  ];

  displayedColumns: string[] = ['id', 'titulo', 'descricao', 'status', 'tecnico', 'dataCriacao', 'acoes'];

  ngOnInit(): void {
    this.loadOrdens();
  }

  loadOrdens(): void {
    this.isLoading.set(true);
    this.ordemServicoService.getAll(this.page(), this.pageSize(), this.filtro()).subscribe({
      next: (result) => {
        this.ordensServico.set(result.data);
        this.totalItems.set(result.totalItems);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.notificationService.error('Erro ao carregar ordens de serviço');
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(newPage: number): void {
    this.page.set(newPage);
    this.loadOrdens();
  }

  onPageSizeChange(newPageSize: number): void {
    this.pageSize.set(newPageSize);
    this.page.set(1);
    this.loadOrdens();
  }

  // Método para lazy loading do PrimeNG
  onLazyLoad(event: any): void {
    if (event.first !== undefined && event.rows !== undefined) {
      const newPage = Math.floor(event.first / event.rows) + 1;
      this.page.set(newPage);
      this.pageSize.set(event.rows);
      this.loadOrdens();
    }
  }

  onSearch(): void {
    this.page.set(1);
    this.loadOrdens();
  }

  onDelete(ordem: OrdemServico): void {
    this.confirmationService.confirm({
      title: 'Confirmar Exclusão',
      message: `Deseja realmente excluir a ordem de serviço "${ordem.titulo}"?`,
      confirmText: 'Excluir',
      cancelText: 'Cancelar'
    }).subscribe(confirmed => {
      if (confirmed) {
        this.ordemServicoService.delete(ordem.id).subscribe({
          next: () => {
            this.notificationService.success('Ordem de serviço excluída com sucesso');
            this.loadOrdens();
          },
          error: () => {
            this.notificationService.error('Erro ao excluir ordem de serviço');
          }
        });
      }
    });
  }

  onCreate(): void {
    this.router.navigate(['/ordem-servico/novo']);
  }

  onEdit(ordem: OrdemServico): void {
    this.router.navigate(['/ordem-servico/editar', ordem.id]);
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Pendente': return 'status-pendente';
      case 'Em Andamento': return 'status-andamento';
      case 'Concluída': return 'status-concluida';
      default: return '';
    }
  }

  // Método para PrimeNG badges
  getStatusSeverity(status: string): 'success' | 'info' | 'warning' | 'danger' {
    switch (status) {
      case 'Pendente': return 'warning';
      case 'Em Andamento': return 'info';
      case 'Concluída': return 'success';
      case 'Cancelada': return 'danger';
      default: return 'info';
    }
  }
}
