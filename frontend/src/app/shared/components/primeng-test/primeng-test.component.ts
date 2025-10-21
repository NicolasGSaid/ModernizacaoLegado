import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

interface TestData {
  id: number;
  titulo: string;
  status: string;
}

@Component({
  selector: 'app-primeng-test',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    TableModule,
    TagModule,
    InputTextModule,
    DropdownModule,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './primeng-test.component.html',
  styleUrl: './primeng-test.component.css'
})
export class PrimengTestComponent {
  private messageService = inject(MessageService);

  testData = signal<TestData[]>([
    { id: 1, titulo: 'Teste 1', status: 'Pendente' },
    { id: 2, titulo: 'Teste 2', status: 'Em Andamento' },
    { id: 3, titulo: 'Teste 3', status: 'Concluída' }
  ]);

  statusOptions = [
    { label: 'Pendente', value: 'Pendente' },
    { label: 'Em Andamento', value: 'Em Andamento' },
    { label: 'Concluída', value: 'Concluída' }
  ];

  showSuccess() {
    this.messageService.add({
      severity: 'success',
      summary: 'Sucesso',
      detail: 'PrimeNG funcionando perfeitamente!'
    });
  }

  showError() {
    this.messageService.add({
      severity: 'error',
      summary: 'Erro',
      detail: 'Teste de mensagem de erro'
    });
  }

  getSeverity(status: string): 'success' | 'info' | 'warning' | 'danger' {
    switch (status) {
      case 'Pendente': return 'warning';
      case 'Em Andamento': return 'info';
      case 'Concluída': return 'success';
      default: return 'info';
    }
  }
}
