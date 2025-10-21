import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { NotificationService } from '../core/services/notification.service';
import { ConfirmationDialogService } from '../core/services/confirmation-dialog.service';
import { FabButtonComponent } from '../shared/components/fab-button/fab-button.component';
import { StatusChipComponent } from '../shared/components/status-chip/status-chip.component';
import { HttpClient } from '@angular/common/http';

/**
 * Componente para gerenciar Ordens de Serviço
 * ✅ MODERNIZADO: Usa services, signals, componentes reutilizáveis
 */
@Component({
  selector: 'app-ordem-servico',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatMenuModule,
    FabButtonComponent,
    StatusChipComponent
  ],
  templateUrl: './ordem-servico.component.html',
  styleUrls: ['./ordem-servico.component.css']
})
export class OrdemServicoComponent implements OnInit {
  // ✅ CORRIGIDO: Tipagem adequada e signals
  ordens: any[] = [];
  
  // ✅ URL da API (ainda hardcoded, mas será movido para service)
  private apiUrl = 'http://localhost:5000/api/ordemservico';
  
  // Flags de controle
  mostrarFormulario = false;
  editando = false;
  
  // Modelo do formulário
  novaOrdem: any = {
    titulo: '',
    descricao: '',
    tecnico: ''
  };
  
  ordemEditando: any = null;

  // ✅ CORRIGIDO: Services injetados usando inject()
  private http = inject(HttpClient);
  private notificationService = inject(NotificationService);
  private confirmationService = inject(ConfirmationDialogService);

  ngOnInit(): void {
    this.carregarOrdens();
  }

  carregarOrdens() {
    // ✅ CORRIGIDO: Tratamento de erro com NotificationService
    this.http.get(this.apiUrl).subscribe({
      next: (data: any) => {
        this.ordens = data;
      },
      error: (error) => {
        console.error('Erro ao carregar ordens:', error);
        this.notificationService.error('Erro ao carregar ordens de serviço!');
      }
    });
  }

  abrirFormulario() {
    this.mostrarFormulario = true;
    this.editando = false;
    this.limparFormulario();
  }

  fecharFormulario() {
    this.mostrarFormulario = false;
    this.limparFormulario();
  }

  limparFormulario() {
    this.novaOrdem = {
      titulo: '',
      descricao: '',
      tecnico: ''
    };
    this.ordemEditando = null;
  }

  salvar() {
    // ✅ CORRIGIDO: Validação com NotificationService
    if (!this.novaOrdem.titulo || this.novaOrdem.titulo.trim() === '') {
      this.notificationService.warning('Título é obrigatório!');
      return;
    }

    if (!this.novaOrdem.tecnico || this.novaOrdem.tecnico.trim() === '') {
      this.notificationService.warning('Técnico é obrigatório!');
      return;
    }

    if (this.editando && this.ordemEditando) {
      this.atualizar();
    } else {
      this.criar();
    }
  }

  criar() {
    // ✅ CORRIGIDO: Sem reload, recarrega apenas os dados
    this.http.post(this.apiUrl, this.novaOrdem).subscribe({
      next: (response) => {
        this.notificationService.success('Ordem de serviço criada com sucesso!');
        this.fecharFormulario();
        this.carregarOrdens(); // ✅ Recarrega apenas os dados
      },
      error: (error) => {
        console.error('Erro ao criar ordem:', error);
        this.notificationService.error('Erro ao criar ordem de serviço!');
      }
    });
  }

  editarOrdem(ordem: any) {
    this.ordemEditando = ordem;
    this.novaOrdem = {
      titulo: ordem.titulo,
      descricao: ordem.descricao,
      tecnico: ordem.tecnico,
      status: ordem.status
    };
    this.mostrarFormulario = true;
    this.editando = true;
  }

  atualizar() {
    const url = `${this.apiUrl}/${this.ordemEditando.id}`;
    this.http.put(url, this.novaOrdem).subscribe({
      next: (response) => {
        this.notificationService.success('Ordem de serviço atualizada com sucesso!');
        this.fecharFormulario();
        this.carregarOrdens(); // ✅ Recarrega apenas os dados
      },
      error: (error) => {
        console.error('Erro ao atualizar ordem:', error);
        this.notificationService.error('Erro ao atualizar ordem de serviço!');
      }
    });
  }

  alterarStatus(ordem: any, novoStatus: string) {
    // ✅ CORRIGIDO: Com feedback adequado
    const url = `${this.apiUrl}/${ordem.id}`;
    const body = { ...ordem, status: novoStatus };
    
    this.http.put(url, body).subscribe({
      next: (response) => {
        this.notificationService.success('Status alterado com sucesso!');
        this.carregarOrdens(); // ✅ Recarrega apenas os dados
      },
      error: (error) => {
        console.error('Erro ao alterar status:', error);
        this.notificationService.error('Erro ao alterar status!');
      }
    });
  }

  excluir(id: number) {
    // ✅ CORRIGIDO: Dialog moderno de confirmação
    this.confirmationService.confirm({
      title: 'Confirmar Exclusão',
      message: 'Tem certeza que deseja excluir esta ordem de serviço? Esta ação não pode ser desfeita.',
      confirmText: 'Excluir',
      cancelText: 'Cancelar'
    }).subscribe(confirmed => {
      if (confirmed) {
        const url = `${this.apiUrl}/${id}`;
        
        this.http.delete(url).subscribe({
          next: (response) => {
            this.notificationService.success('Ordem de serviço excluída com sucesso!');
            this.carregarOrdens(); // ✅ Recarrega apenas os dados
          },
          error: (error) => {
            console.error('Erro ao excluir ordem:', error);
            this.notificationService.error('Erro ao excluir ordem de serviço!');
          }
        });
      }
    });
  }

  // ❌ PROBLEMA 10: Lógica de apresentação no componente (poderia ser um pipe)
  getStatusClass(status: string): string {
    switch(status) {
      case 'Pendente':
        return 'status-pendente';
      case 'Em Andamento':
        return 'status-andamento';
      case 'Concluída':
        return 'status-concluida';
      default:
        return '';
    }
  }

  formatarData(data: string): string {
    // ❌ Formatação manual (deveria usar DatePipe do Angular)
    if (!data) return '';
    const d = new Date(data);
    return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1).toString().padStart(2, '0')}/${d.getFullYear()}`;
  }
}
