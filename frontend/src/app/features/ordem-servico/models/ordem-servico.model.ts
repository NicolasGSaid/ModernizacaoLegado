// Modelo que reflete OrdemServicoDto do backend .NET 8
export interface OrdemServico {
  id: number;
  titulo: string;
  descricao: string;
  status: OrdemServicoStatus;
  tecnico: string;
  dataCriacao: Date;
  dataAtualizacao?: Date;
}

export enum OrdemServicoStatus {
  Pendente = 'Pendente',
  EmAndamento = 'Em Andamento',
  Concluida = 'Concluída'
}

// DTO para criação
export interface CreateOrdemServicoDto {
  titulo: string;
  descricao: string;
  tecnico: string;
}

// DTO para atualização completa (PUT)
export interface UpdateOrdemServicoDto {
  titulo: string;
  descricao?: string;
  status: string;
  tecnico: string;
}
