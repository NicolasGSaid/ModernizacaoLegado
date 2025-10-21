import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatChipsModule } from '@angular/material/chips';
// PrimeNG Imports
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { TecnicoService } from '../../services/tecnico.service';
import { Tecnico } from '../../models/tecnico.model';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { NotificationService } from '../../../../core/services/notification.service';
import { ConfirmationDialogService } from '../../../../core/services/confirmation-dialog.service';

@Component({
  selector: 'app-tecnico-list',
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
    // PrimeNG
    TableModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    // Shared
    PaginationComponent
  ],
  templateUrl: './tecnico-list-primeng.component.html',
  styleUrls: ['./tecnico-list-primeng.component.css']
})
export class TecnicoListComponent implements OnInit {
  private tecnicoService = inject(TecnicoService);
  private notificationService = inject(NotificationService);
  private confirmationService = inject(ConfirmationDialogService);
  private router = inject(Router);

  // Signals para estado reativo
  tecnicos = signal<Tecnico[]>([]);
  page = signal(1);
  pageSize = signal(10);
  totalItems = signal(0);
  filtro = signal('');
  isLoading = signal(false);

  displayedColumns: string[] = ['id', 'nome', 'email', 'telefone', 'especialidade', 'status', 'dataCadastro', 'acoes'];

  ngOnInit(): void {
    this.loadTecnicos();
  }

  loadTecnicos(): void {
    this.isLoading.set(true);
    this.tecnicoService.getAll(this.page(), this.pageSize(), this.filtro()).subscribe({
      next: (result) => {
        this.tecnicos.set(result.data);
        this.totalItems.set(result.totalItems);
        this.isLoading.set(false);
      },
      error: () => {
        this.notificationService.error('Erro ao carregar técnicos');
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(newPage: number): void {
    this.page.set(newPage);
    this.loadTecnicos();
  }

  onPageSizeChange(newPageSize: number): void {
    this.pageSize.set(newPageSize);
    this.page.set(1);
    this.loadTecnicos();
  }

  onSearch(): void {
    this.page.set(1);
    this.loadTecnicos();
  }

  onEdit(tecnico: Tecnico): void {
    this.router.navigate(['/tecnico/edit', tecnico.id]);
  }

  onDelete(tecnico: Tecnico): void {
    this.confirmationService.confirm({
      title: 'Confirmar Exclusão',
      message: `Deseja realmente excluir o técnico "${tecnico.nome}"?`,
      confirmText: 'Excluir',
      cancelText: 'Cancelar'
    }).subscribe(confirmed => {
      if (confirmed) {
        this.tecnicoService.delete(tecnico.id).subscribe({
          next: () => {
            this.notificationService.success('Técnico excluído com sucesso');
            this.loadTecnicos();
          },
          error: () => {
            this.notificationService.error('Erro ao excluir técnico');
          }
        });
      }
    });
  }

  onCreate(): void {
    this.router.navigate(['/tecnico/new']);
  }

  getStatusClass(status: string): string {
    return status === 'Ativo' ? 'status-ativo' : 'status-inativo';
  }

  getStatusSeverity(status: string): 'success' | 'danger' {
    return status === 'Ativo' ? 'success' : 'danger';
  }
}
