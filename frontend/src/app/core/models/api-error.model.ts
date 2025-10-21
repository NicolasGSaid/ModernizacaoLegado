// Modelo para erros da API
export interface ApiError {
  type: string;
  title: string;
  status: number;
  errors?: { [key: string]: string[] };
  traceId?: string;
}
