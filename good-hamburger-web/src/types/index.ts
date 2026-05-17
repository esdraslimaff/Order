export type TipoItem = 1 | 2 | 3 | 4;

export const TipoItemDescricao: Record<number, string> = {
  1: 'Sanduíche',
  2: 'Acompanhamento',
  3: 'Bebida',
  4: 'Adicional'
};

export interface OpcaoDto {
  id: string;
  nome: string;
  precoAdicional: number;
}

export interface GrupoOpcaoDto {
  id: string;
  nome: string;
  tipoSelecao: number;
  obrigatorio: boolean;
  minimoSelecoes?: number;
  maximoSelecoes?: number;
  opcoes: OpcaoDto[];
}

export interface ItemCardapioDto {
  id: string;
  nome: string;
  precoUnitario: number;
  tipo: TipoItem;
  gruposOpcoes: GrupoOpcaoDto[];
}

export interface PromocaoDto {
  id: string;
  nome: string;
  percentual: number;
  ativo: boolean;
  requisitosTipo: TipoItem[];
  itensObrigatoriosIds: string[];
}

export interface OpcaoSelecionadaRequest {
  opcaoId: string;
  quantidade: number;
}

export interface ItemPedidoRequest {
  itemId: string;
  quantidade: number;
  opcoesSelecionadas: OpcaoSelecionadaRequest[];
  observacao?: string;
}

export interface PedidoRequest {
  itens: ItemPedidoRequest[];
  observacao?: string;
}

export interface OpcaoSelecionadaResponse {
  id: string;
  nomeOpcao: string;
  nomeGrupo: string;
  precoUnitario: number;
  quantidade: number;
}

export interface PedidoItemResponse {
  id: string;
  produtoId: string;
  nome: string;
  precoUnitario: number;
  tipo: TipoItem;
  quantidade: number;
  observacao?: string;
  opcoesSelecionadas: OpcaoSelecionadaResponse[];
}

export interface PedidoResponse {
  id: string;
  promocaoId?: string;
  dataCriacao: string;
  itens: PedidoItemResponse[];
  subtotal: number;
  descontoPercentual: number;
  valorDesconto: number;
  totalFinal: number;
  observacao?: string;
}

export interface OpcaoSelecionadaLocal {
  opcaoId: string;
  nomeOpcao: string;
  nomeGrupo: string;
  precoUnitario: number;
  quantidade: number;
}

export interface ItemCarrinhoResumo {
  itemId: string;
  nome: string;
  precoUnitario: number;
  quantidade: number;
  opcoes: OpcaoSelecionadaLocal[];
  observacao?: string;
}