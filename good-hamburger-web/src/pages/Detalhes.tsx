import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { apiClient } from '../api/apiClient';
import { type PedidoResponse } from '../types';

export const Detalhes: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [pedido, setPedido] = useState<PedidoResponse | null>(null);
  const [carregando, setCarregando] = useState(true);

  useEffect(() => {
    const carregarPedido = async () => {
      try {
        const response = await apiClient.get<PedidoResponse>(`/Pedidos/${id}`);
        setPedido(response.data);
      } catch (error) {
        console.error("Erro ao carregar os detalhes do pedido", error);
        navigate('/');
      } finally {
        setCarregando(false);
      }
    };

    if (id) carregarPedido();
  }, [id, navigate]);

  return (
    <div className="container mt-4">
      <div className="card shadow">
        <div className="card-header bg-success text-white">
          <h4>Pedido Confirmado! 🎉</h4>
        </div>
        <div className="card-body">
          {carregando || !pedido ? (
            <p>Carregando detalhes do pedido...</p>
          ) : (
            <>
              <p><strong>Protocolo:</strong> {pedido.id}</p>
              <p>
                <strong>Data:</strong> {new Date(pedido.dataCriacao).toLocaleDateString('pt-BR')} às{' '}
                {new Date(pedido.dataCriacao).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}
              </p>
              {pedido.observacao && <p><strong>Observação geral:</strong> {pedido.observacao}</p>}

              <hr />
              <h5>Itens:</h5>
              <ul className="list-group mb-3">
                {pedido.itens.map(item => {
                  const opcoesPorGrupo = item.opcoesSelecionadas.reduce((acc, o) => {
                    if (!acc[o.nomeGrupo]) acc[o.nomeGrupo] = [];
                    acc[o.nomeGrupo].push(o);
                    return acc;
                  }, {} as Record<string, typeof item.opcoesSelecionadas>);
                  const totalOpcoes = item.opcoesSelecionadas.reduce((acc, o) => acc + o.precoUnitario * o.quantidade, 0);
                  const totalItem = item.precoUnitario * item.quantidade + totalOpcoes;

                  return (
                    <li key={item.id} className="list-group-item">
                      <div className="d-flex justify-content-between align-items-start">
                        <div>
                          <strong>{item.quantidade}x {item.nome}</strong>
                          {item.observacao && <small className="d-block text-muted">Obs: {item.observacao}</small>}
                          {Object.entries(opcoesPorGrupo).map(([grupo, opcoes]) => (
                            <div key={grupo} className="small mt-1">
                              <em>{grupo}:</em>
                              <ul className="mb-0 ps-3">
                                {opcoes.map(o => (
                                  <li key={o.id}>{o.quantidade}x {o.nomeOpcao} (+R$ {o.precoUnitario.toFixed(2)})</li>
                                ))}
                              </ul>
                            </div>
                          ))}
                        </div>
                        <span>R$ {totalItem.toFixed(2)}</span>
                      </div>
                    </li>
                  );
                })}
              </ul>

              <div className="text-end">
                <p>Subtotal: R$ {pedido.subtotal.toFixed(2)}</p>
                {pedido.descontoPercentual > 0 && (
                  <p className="text-success">
                    Desconto ({(pedido.descontoPercentual * 100).toFixed(0)}%):
                    - R$ {pedido.valorDesconto.toFixed(2)}
                  </p>
                )}
                <h4 className="fw-bold text-primary">Total Final: R$ {pedido.totalFinal.toFixed(2)}</h4>
              </div>
            </>
          )}
        </div>
        <div className="card-footer">
          <button className="btn btn-primary me-2" onClick={() => navigate('/novo-pedido')}>Novo Pedido</button>
          <button className="btn btn-secondary" onClick={() => navigate('/pedidos')}>Ver pedidos</button>
        </div>
      </div>
    </div>
  );
};