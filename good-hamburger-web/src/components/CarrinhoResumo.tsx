import React from 'react';
import { type ItemCardapioDto } from '../types';
import { type PedidoResumo } from '../utils/calculadoraPedido';

interface CarrinhoResumoProps {
  itensNoCarrinho: ItemCardapioDto[];
  resumo: PedidoResumo;
  processando: boolean;
  mensagemErro?: string;
  onRemover: (item: ItemCardapioDto) => void;
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
              {itensNoCarrinho.map((item) => (
                <li key={item.id} className="list-group-item d-flex justify-content-between align-items-center px-0">
                  <div>
                    <span className="fw-bold">{item.nome}</span><br />
                    <small className="text-muted">{formatCurrency(item.precoUnitario)}</small>
                  </div>
                  <button 
                    className="btn btn-sm btn-outline-danger border-0" 
                    onClick={() => onRemover(item)}
                  >
                    <i className="bi bi-trash"></i> Remover
                  </button>
                </li>
              ))}
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