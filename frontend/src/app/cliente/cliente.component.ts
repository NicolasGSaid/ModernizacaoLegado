import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

/**
 * Componente para gerenciar Clientes
 * ⚠️ ATENÇÃO: Ainda mais código duplicado!
 */
@Component({
  selector: 'app-cliente',
  templateUrl: './cliente.component.html',
  styleUrls: ['./cliente.component.css']
})
export class ClienteComponent implements OnInit {
  clientes: any[] = [];
  private apiUrl = 'http://localhost:5000/api/cliente';
  
  mostrarFormulario = false;
  editando = false;
  
  novoCliente: any = {
    razaoSocial: '',
    nomeFantasia: '',
    cnpj: '',
    email: '',
    telefone: '',
    endereco: '',
    cidade: '',
    estado: '',
    cep: ''
  };
  
  clienteEditando: any = null;
  
  // ❌ Lista de estados hardcoded
  estados = [
    'AC', 'AL', 'AP', 'AM', 'BA', 'CE', 'DF', 'ES', 'GO', 'MA',
    'MT', 'MS', 'MG', 'PA', 'PB', 'PR', 'PE', 'PI', 'RJ', 'RN',
    'RS', 'RO', 'RR', 'SC', 'SP', 'SE', 'TO'
  ];

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.carregarClientes();
  }

  carregarClientes() {
    this.http.get(this.apiUrl).subscribe(
      (data: any) => {
        this.clientes = data;
      },
      (error) => {
        console.error('Erro ao carregar clientes:', error);
        alert('Erro ao carregar clientes!');
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
    this.novoCliente = {
      razaoSocial: '',
      nomeFantasia: '',
      cnpj: '',
      email: '',
      telefone: '',
      endereco: '',
      cidade: '',
      estado: '',
      cep: ''
    };
    this.clienteEditando = null;
  }

  salvar() {
    // ❌ Validação básica no componente
    if (!this.novoCliente.razaoSocial || this.novoCliente.razaoSocial.trim() === '') {
      alert('Razão Social é obrigatória!');
      return;
    }

    if (!this.novoCliente.cnpj || this.novoCliente.cnpj.trim() === '') {
      alert('CNPJ é obrigatório!');
      return;
    }

    // ❌ Validação de CNPJ extremamente fraca
    if (this.novoCliente.cnpj.length < 14) {
      alert('CNPJ inválido!');
      return;
    }

    if (this.editando && this.clienteEditando) {
      this.atualizar();
    } else {
      this.criar();
    }
  }

  criar() {
    this.http.post(this.apiUrl, this.novoCliente).subscribe(
      (response) => {
        alert('Cliente criado com sucesso!');
        this.fecharFormulario();
        location.reload();
      },
      (error) => {
        console.error('Erro ao criar cliente:', error);
        alert('Erro ao criar cliente!');
      }
    );
  }

  editarCliente(cliente: any) {
    this.clienteEditando = cliente;
    this.novoCliente = {
      razaoSocial: cliente.razaoSocial,
      nomeFantasia: cliente.nomeFantasia,
      cnpj: cliente.cnpj,
      email: cliente.email,
      telefone: cliente.telefone,
      endereco: cliente.endereco,
      cidade: cliente.cidade,
      estado: cliente.estado,
      cep: cliente.cep
    };
    this.mostrarFormulario = true;
    this.editando = true;
  }

  atualizar() {
    const url = `${this.apiUrl}/${this.clienteEditando.id}`;
    this.http.put(url, this.novoCliente).subscribe(
      (response) => {
        alert('Cliente atualizado com sucesso!');
        this.fecharFormulario();
        location.reload();
      },
      (error) => {
        console.error('Erro ao atualizar cliente:', error);
        alert('Erro ao atualizar cliente!');
      }
    );
  }

  excluir(id: number) {
    if (confirm('Tem certeza que deseja excluir este cliente? Esta ação pode afetar ordens de serviço vinculadas.')) {
      const url = `${this.apiUrl}/${id}`;
      
      this.http.delete(url).subscribe(
        (response) => {
          alert('Cliente excluído com sucesso!');
          location.reload();
        },
        (error) => {
          console.error('Erro ao excluir cliente:', error);
          alert('Erro ao excluir cliente! Pode haver ordens de serviço vinculadas.');
        }
      );
    }
  }

  // ❌ Máscara de CNPJ feita manualmente (ruim)
  formatarCNPJ(cnpj: string): string {
    if (!cnpj) return '';
    return cnpj.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, '$1.$2.$3/$4-$5');
  }

  formatarCEP(cep: string): string {
    if (!cep) return '';
    return cep.replace(/^(\d{5})(\d{3})$/, '$1-$2');
  }

  formatarData(data: string): string {
    if (!data) return '';
    const d = new Date(data);
    return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1).toString().padStart(2, '0')}/${d.getFullYear()}`;
  }
}
