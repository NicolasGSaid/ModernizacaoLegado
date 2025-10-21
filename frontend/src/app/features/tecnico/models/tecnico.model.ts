// Modelo que reflete TecnicoDto do backend .NET 8
export interface Tecnico {
  id: number;
  nome: string;
  email: string;
  telefone: string;
  especialidade: string;
  status: TecnicoStatus;
  dataCadastro: Date;
}

export enum TecnicoStatus {
  Ativo = 'Ativo',
  Inativo = 'Inativo'
}

// DTO para criação
export interface CreateTecnicoDto {
  nome: string;
  email: string;
  telefone: string;
  especialidade: string;
}

// DTO para atualização
export interface UpdateTecnicoDto {
  nome?: string;
  email?: string;
  telefone?: string;
  especialidade?: string;
  status?: TecnicoStatus;
}
