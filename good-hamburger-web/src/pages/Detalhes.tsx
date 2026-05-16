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
        navigate('/'); // Volta para a home se o pedido não existir
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
                <strong>Data:</strong> {new Date(pedido.dataCriacao).toLocaleDateString('pt-BR')} às {new Date(pedido.dataCriacao).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}
              </p>

              <hr />
              <h5>Itens:</h5>
              <ul className="list-group mb-3">
                {pedido.itens.map(item => (
                  <li key={item.id} className="list-group-item d-flex justify-content-between">
                    {item.nome}
                    <span>R$ {item.precoUnitario.toFixed(2)}</span>
                  </li>
                ))}
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