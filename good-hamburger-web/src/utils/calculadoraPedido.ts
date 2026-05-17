import type { ItemCardapioDto, PromocaoDto } from '../types';

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
    const tiposNoCarrinho = [...new Set(itens.map(x => x.tipo))];
    const idsNoCarrinho = itens.map(x => x.id);

    const melhorPromocao = promocoesPermitidas
      .filter(p => {
        if (p.requisitosTipo.length > 0) {
          if (!p.requisitosTipo.every(t => tiposNoCarrinho.includes(t))) return false;
        }
        if (p.itensObrigatoriosIds.length > 0) {
          if (!p.itensObrigatoriosIds.every(id => idsNoCarrinho.includes(id))) return false;
        }
        if (p.requisitosTipo.length === 0 && p.itensObrigatoriosIds.length === 0) return false;
        return true;
      })
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