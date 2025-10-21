import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
// PrimeNG Imports
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ClienteService } from '../../services/cliente.service';
import { Cliente } from '../../models/cliente.model';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { NotificationService } from '../../../../core/services/notification.service';
import { ConfirmationDialogService } from '../../../../core/services/confirmation-dialog.service';

@Component({
  selector: 'app-cliente-list',
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
    // PrimeNG
    TableModule,
    ButtonModule,
    InputTextModule,
    // Shared
    PaginationComponent
  ],
  templateUrl: './cliente-list-primeng.component.html',
  styleUrl: './cliente-list-primeng.component.css'
})
export class ClienteListComponent implements OnInit {
  private clienteService = inject(ClienteService);
  private notificationService = inject(NotificationService);
  private confirmationService = inject(ConfirmationDialogService);
  private router = inject(Router);

  // Signals para estado reativo
  clientes = signal<Cliente[]>([]);
  page = signal(1);
  pageSize = signal(10);
  totalItems = signal(0);
  busca = signal('');
  isLoading = signal(false);

  displayedColumns: string[] = ['id', 'razaoSocial', 'nomeFantasia', 'cnpj', 'email', 'telefone', 'cidade', 'estado', 'acoes'];

  ngOnInit(): void {
    this.loadClientes();
  }

  loadClientes(): void {
    this.isLoading.set(true);
    this.clienteService.getAll(this.page(), this.pageSize(), this.busca()).subscribe({
      next: (result) => {
        this.clientes.set(result.data);
        this.totalItems.set(result.totalItems);
        this.isLoading.set(false);
      },
      error: () => {
        this.notificationService.error('Erro ao carregar clientes');
        this.isLoading.set(false);
      }
    });
  }

  onPageChange(newPage: number): void {
    this.page.set(newPage);
    this.loadClientes();
  }

  onPageSizeChange(newPageSize: number): void {
    this.pageSize.set(newPageSize);
    this.page.set(1);
    this.loadClientes();
  }

  onSearch(): void {
    this.page.set(1);
    this.loadClientes();
  }

  onEdit(cliente: Cliente): void {
    this.router.navigate(['/cliente/edit', cliente.id]);
  }

  onDelete(cliente: Cliente): void {
    this.confirmationService.confirm({
      title: 'Confirmar Exclusão',
      message: `Deseja realmente excluir o cliente "${cliente.razaoSocial}"?`,
      confirmText: 'Excluir',
      cancelText: 'Cancelar'
    }).subscribe(confirmed => {
      if (confirmed) {
        this.clienteService.delete(cliente.id).subscribe({
          next: () => {
            this.notificationService.success('Cliente excluído com sucesso');
            this.loadClientes();
          },
          error: () => {
            this.notificationService.error('Erro ao excluir cliente');
          }
        });
      }
    });
  }

  onCreate(): void {
    this.router.navigate(['/cliente/new']);
  }
}
