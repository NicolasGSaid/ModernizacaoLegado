import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

/**
 * Componente para gerenciar Técnicos
 * ⚠️ ATENÇÃO: Mesmos problemas do OrdemServicoComponent (código duplicado!)
 */
@Component({
  selector: 'app-tecnico',
  templateUrl: './tecnico.component.html',
  styleUrls: ['./tecnico.component.css']
})
export class TecnicoComponent implements OnInit {
  // ❌ Tipagem any
  tecnicos: any[] = [];
  
  // ❌ URL hardcoded
  private apiUrl = 'http://localhost:5000/api/tecnico';
  
  mostrarFormulario = false;
  editando = false;
  
  // ❌ Sem tipagem
  novoTecnico: any = {
    nome: '',
    email: '',
    telefone: '',
    especialidade: '',
    status: 'Ativo'
  };
  
  tecnicoEditando: any = null;

  // ❌ HTTP injetado diretamente
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.carregarTecnicos();
  }

  carregarTecnicos() {
    this.http.get(this.apiUrl).subscribe(
      (data: any) => {
        this.tecnicos = data;
      },
      (error) => {
        console.error('Erro ao carregar técnicos:', error);
        alert('Erro ao carregar técnicos!');
      }
    );
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
    this.novoTecnico = {
      nome: '',
      email: '',
      telefone: '',
      especialidade: '',
      status: 'Ativo'
    };
    this.tecnicoEditando = null;
  }

  salvar() {
    // ❌ Validação no componente
    if (!this.novoTecnico.nome || this.novoTecnico.nome.trim() === '') {
      alert('Nome é obrigatório!');
      return;
    }

    if (!this.novoTecnico.especialidade) {
      alert('Especialidade é obrigatória!');
      return;
    }

    if (this.editando && this.tecnicoEditando) {
      this.atualizar();
    } else {
      this.criar();
    }
  }

  criar() {
    this.http.post(this.apiUrl, this.novoTecnico).subscribe(
      (response) => {
        alert('Técnico criado com sucesso!');
        this.fecharFormulario();
        location.reload(); // ❌ Reload
      },
      (error) => {
        console.error('Erro ao criar técnico:', error);
        alert('Erro ao criar técnico!');
      }
    );
  }

  editarTecnico(tecnico: any) {
    this.tecnicoEditando = tecnico;
    this.novoTecnico = {
      nome: tecnico.nome,
      email: tecnico.email,
      telefone: tecnico.telefone,
      especialidade: tecnico.especialidade,
      status: tecnico.status
    };
    this.mostrarFormulario = true;
    this.editando = true;
  }

  atualizar() {
    const url = `${this.apiUrl}/${this.tecnicoEditando.id}`;
    this.http.put(url, this.novoTecnico).subscribe(
      (response) => {
        alert('Técnico atualizado com sucesso!');
        this.fecharFormulario();
        location.reload();
      },
      (error) => {
        console.error('Erro ao atualizar técnico:', error);
        alert('Erro ao atualizar técnico!');
      }
    );
  }

  alterarStatus(tecnico: any, novoStatus: string) {
    const url = `${this.apiUrl}/${tecnico.id}`;
    const body = { ...tecnico, status: novoStatus };
    
    this.http.put(url, body).subscribe(
      (response) => {
        alert('Status alterado com sucesso!');
        location.reload();
      },
      (error) => {
        console.error('Erro ao alterar status:', error);
        alert('Erro ao alterar status!');
      }
    );
  }

  excluir(id: number) {
    if (confirm('Tem certeza que deseja excluir este técnico?')) {
      const url = `${this.apiUrl}/${id}`;
      
      this.http.delete(url).subscribe(
        (response) => {
          alert('Técnico excluído com sucesso!');
          location.reload();
        },
        (error) => {
          console.error('Erro ao excluir técnico:', error);
          alert('Erro ao excluir técnico!');
        }
      );
    }
  }

  // ❌ Lógica de apresentação no componente
  getStatusClass(status: string): string {
    switch(status) {
      case 'Ativo':
        return 'status-ativo';
      case 'Inativo':
        return 'status-inativo';
      case 'Férias':
        return 'status-ferias';
      default:
        return '';
    }
  }

  formatarData(data: string): string {
    if (!data) return '';
    const d = new Date(data);
    return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1).toString().padStart(2, '0')}/${d.getFullYear()}`;
  }
}
