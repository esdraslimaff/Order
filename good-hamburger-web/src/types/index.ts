// O tipo substitui o Enum e é apagado na compilação (Erasable Syntax)
export type TipoItem = 1 | 2 | 3;

// O dicionário substitui a conversão de (int) para string que o C# faz
export const TipoItemDescricao: Record<number, string> = {
  1: 'Sanduíche',
  2: 'Acompanhamento',
  3: 'Bebida'
};

export interface ItemCardapioDto {
  id: string;
  nome: string;
  precoUnitario: number;
  tipo: TipoItem;
}

export interface PromocaoDto {
  id: string;
  nome: string;
  percentual: number;
  ativo: boolean;
  requisitos: TipoItem[];
}

export interface PedidoRequest {
  itensIds: string[];
}

export interface PedidoResponse {
  id: string;
  promocaoId?: string;
  dataCriacao: string;
  itens: ItemCardapioDto[];
  subtotal: number;
  descontoPercentual: number;
  valorDesconto: number;
  totalFinal: number;
}