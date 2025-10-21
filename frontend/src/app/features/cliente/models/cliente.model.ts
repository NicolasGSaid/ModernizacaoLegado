// Modelo que reflete ClienteDto do backend .NET 8
export interface Cliente {
  id: number;
  razaoSocial: string;
  nomeFantasia?: string;
  cnpj: string;
  email: string;
  telefone?: string;
  endereco: string;
  cidade: string;
  estado: string;
  cep: string;
  dataCadastro: Date;
}

// DTO para criação
export interface CreateClienteDto {
  razaoSocial: string;
  nomeFantasia?: string;
  cnpj: string;
  email: string;
  telefone?: string;
  endereco: string;
  cidade: string;
  estado: string;
  cep: string;
}

// DTO para atualização
export interface UpdateClienteDto {
  razaoSocial?: string;
  nomeFantasia?: string;
  email?: string;
  telefone?: string;
  endereco?: string;
  cidade?: string;
  estado?: string;
  cep?: string;
}
