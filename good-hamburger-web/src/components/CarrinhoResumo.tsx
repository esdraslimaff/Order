import React from 'react';
import { type ItemCarrinhoResumo } from '../types';
import { type PedidoResumo } from '../utils/calculadoraPedido';

interface CarrinhoResumoProps {
  itensNoCarrinho: ItemCarrinhoResumo[];
  resumo: PedidoResumo;
  processando: boolean;
  mensagemErro?: string;
  onRemover: (itemId: string) => void;
  onFinalizar: () => void;
}

const formatCurrency = (value: number) =>
  new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);

export const CarrinhoResumo: React.FC<CarrinhoResumoProps> = ({
  itensNoCarrinho,
  resumo,
  processando,
  mensagemErro,
  onRemover,
  onFinalizar
}) => {
  return (
    <div className="card border-primary shadow">
      <div className="card-header bg-primary text-white">
        <h5 className="mb-0">Sua Bandeja 🍔</h5>
      </div>
      <div className="card-body">
        {itensNoCarrinho.length === 0 ? (
          <p className="text-muted text-center my-4">Sua bandeja está vazia.</p>
        ) : (
          <>
            <ul className="list-group list-group-flush mb-3">
              {itensNoCarrinho.map((item) => {
                const totalOpcoes = item.opcoes.reduce((acc, o) => acc + o.precoUnitario * o.quantidade, 0);
                const totalItem = item.precoUnitario * item.quantidade + totalOpcoes;
                // Agrupa opções por nome do grupo
                const opcoesPorGrupo = item.opcoes.reduce((acc, o) => {
                  if (!acc[o.nomeGrupo]) acc[o.nomeGrupo] = [];
                  acc[o.nomeGrupo].push(o);
                  return acc;
                }, {} as Record<string, typeof item.opcoes>);

                return (
                  <li key={item.itemId} className="list-group-item px-0">
                    <div className="d-flex justify-content-between align-items-start">
                      <div>
                        <strong>{item.quantidade}x {item.nome} (R$ {item.precoUnitario.toFixed(2)})</strong>
                        {item.observacao && <small className="d-block text-muted">Obs: {item.observacao}</small>}
                        {Object.entries(opcoesPorGrupo).map(([grupo, opcoes]) => (
                          <div key={grupo} className="small mt-1">
                            <em className="text-secondary">{grupo}:</em>
                            <ul className="mb-0 ps-3">
                              {opcoes.map((o, idx) => (
                                <li key={idx}>{o.quantidade}x {o.nomeOpcao} (+R$ {o.precoUnitario.toFixed(2)})</li>
                              ))}
                            </ul>
                          </div>
                        ))}
                      </div>
                      <div className="d-flex align-items-center">
                        <span className="me-2">R$ {totalItem.toFixed(2)}</span>
                        <button className="btn btn-sm btn-outline-danger border-0" onClick={() => onRemover(item.itemId)}>
                          ✕
                        </button>
                      </div>
                    </div>
                  </li>
                );
              })}
            </ul>

            <div className="border-top pt-3">
              <div className="d-flex justify-content-between small">
                <span>Subtotal</span>
                <span>{formatCurrency(resumo.subtotal)}</span>
              </div>

              {resumo.percentualDesconto > 0 && (
                <div className="d-flex justify-content-between text-success small">
                  <span>Desconto ({resumo.nomePromocaoAtiva})</span>
                  <span>- {formatCurrency(resumo.valorDesconto)}</span>
                </div>
              )}

              <div className="d-flex justify-content-between fw-bold fs-5 mt-2">
                <span>Total</span>
                <span>{formatCurrency(resumo.totalFinal)}</span>
              </div>
            </div>

            {mensagemErro && (
              <div className="alert alert-warning p-2 mt-3 small">
                <i className="bi bi-exclamation-triangle"></i> {mensagemErro}
              </div>
            )}

            <button
              className="btn btn-success w-100 mt-3 py-2 fw-bold"
              onClick={onFinalizar}
              disabled={processando}
            >
              {processando ? (
                <span className="spinner-border spinner-border-sm"></span>
              ) : (
                <span>FECHAR PEDIDO 🚀</span>
              )}
            </button>

            <div className="mt-3 p-3 bg-light rounded shadow-sm">
              <small className="text-muted">
                <strong>Dica:</strong> Escolha no máximo <strong>1 item por categoria</strong> (ex: sanduíche, acompanhamento, bebida).
                O pedido pode ter até <strong>3 itens no total</strong>.
                As alterações só serão salvas ao clicar em <strong>Fechar Pedido</strong>.
              </small>
            </div>
          </>
        )}
      </div>
    </div>
  );
};