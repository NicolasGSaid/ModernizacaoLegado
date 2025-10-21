import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';

/**
 * Status Chip Component
 * Componente para exibir status com cores semânticas
 * 
 * @example
 * <app-status-chip status="Pendente"></app-status-chip>
 * <app-status-chip status="Ativo" [showIcon]="true"></app-status-chip>
 */
@Component({
  selector: 'app-status-chip',
  standalone: true,
  imports: [
    CommonModule,
    MatChipsModule,
    MatIconModule
  ],
  template: `
    <mat-chip [class]="'status-chip status-' + statusClass" [disabled]="true">
      <mat-icon *ngIf="showIcon" class="status-icon">{{ icon }}</mat-icon>
      {{ label }}
    </mat-chip>
  `,
  styles: [`
    .status-chip {
      font-weight: 500;
      font-size: 0.875rem;
      border-radius: 16px;
      padding: 4px 12px;
      height: auto;
      min-height: 28px;
    }
    
    .status-icon {
      font-size: 16px;
      width: 16px;
      height: 16px;
      margin-right: 4px;
    }
    
    /* Status de Ordem de Serviço */
    .status-pendente {
      background-color: #FEF3C7 !important;
      color: #92400E !important;
    }
    
    .status-em-andamento {
      background-color: #DBEAFE !important;
      color: #1E40AF !important;
    }
    
    .status-concluida {
      background-color: #D1FAE5 !important;
      color: #065F46 !important;
    }
    
    .status-cancelada {
      background-color: #FEE2E2 !important;
      color: #991B1B !important;
    }
    
    /* Status de Técnico */
    .status-ativo {
      background-color: #D1FAE5 !important;
      color: #065F46 !important;
    }
    
    .status-inativo {
      background-color: #E5E7EB !important;
      color: #374151 !important;
    }
    
    /* Hover effect */
    .status-chip:hover {
      opacity: 0.9;
    }
  `]
})
export class StatusChipComponent {
  /** Status a ser exibido */
  @Input() status!: string;
  
  /** Exibir ícone ao lado do texto */
  @Input() showIcon = false;
  
  /** Getter para classe CSS baseada no status */
  get statusClass(): string {
    const statusMap: { [key: string]: string } = {
      'Pendente': 'pendente',
      'Em Andamento': 'em-andamento',
      'Concluída': 'concluida',
      'Concluida': 'concluida', // Sem acento
      'Cancelada': 'cancelada',
      'Ativo': 'ativo',
      'Inativo': 'inativo'
    };
    
    return statusMap[this.status] || 'pendente';
  }
  
  /** Getter para label exibido */
  get label(): string {
    return this.status || 'Indefinido';
  }
  
  /** Getter para ícone baseado no status */
  get icon(): string {
    const iconMap: { [key: string]: string } = {
      'Pendente': 'schedule',
      'Em Andamento': 'autorenew',
      'Concluída': 'check_circle',
      'Concluida': 'check_circle',
      'Cancelada': 'cancel',
      'Ativo': 'check_circle',
      'Inativo': 'block'
    };
    
    return iconMap[this.status] || 'help';
  }
}
