import type { ItemCardapioDto, PromocaoDto, TipoItem } from '../types';

export interface PedidoResumo {
  subtotal: number;
  percentualDesconto: number;
  nomePromocaoAtiva: string;
  valorDesconto: number;
  totalFinal: number;
}

export const calcularPedido = (
  itens: ItemCardapioDto[],
  promocoesPermitidas: PromocaoDto[],
  descontoPercentualSalvo: number = 0,
  foiAlterado: boolean = false
): PedidoResumo => {
  const subtotal = itens.reduce((acc, item) => acc + item.precoUnitario, 0);
  
  let resumo: PedidoResumo = {
    subtotal,
    percentualDesconto: 0,
    nomePromocaoAtiva: "Sem promoção aplicável",
    valorDesconto: 0,
    totalFinal: subtotal
  };

  if (!foiAlterado && descontoPercentualSalvo > 0) {
    const promoOriginal = promocoesPermitidas[0];
    resumo.percentualDesconto = descontoPercentualSalvo;
    resumo.nomePromocaoAtiva = promoOriginal?.nome ?? "Promoção Original";
  } else {
    // Pega os tipos distintos presentes no carrinho
    const tiposNoCarrinho = [...new Set(itens.map(x => x.tipo))];

    // Encontra a melhor promoção
    const melhorPromocao = promocoesPermitidas
      .filter(p => 
        p.requisitos.every((tipoReq: TipoItem) => tiposNoCarrinho.includes(tipoReq)) &&
        p.requisitos.length === tiposNoCarrinho.length
      )
      .sort((a, b) => b.percentual - a.percentual)[0];

    if (melhorPromocao) {
      resumo.percentualDesconto = melhorPromocao.percentual;
      resumo.nomePromocaoAtiva = melhorPromocao.nome;
    }
  }

  resumo.valorDesconto = resumo.subtotal * resumo.percentualDesconto;
  resumo.totalFinal = resumo.subtotal - resumo.valorDesconto;

  return resumo;
};