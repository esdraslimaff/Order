import React, { useEffect, useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import Swal from 'sweetalert2';
import { apiClient } from '../api/apiClient';
import { type PedidoResponse } from '../types';
import { AuthContext } from '../contexts/AuthContext';

export const Pedidos: React.FC = () => {
  const [listaPedidos, setListaPedidos] = useState<PedidoResponse[] | null>(null);
  const navigate = useNavigate();
  const { usuario } = useContext(AuthContext);

  const carregarPedidos = async () => {
    try {
      const response = await apiClient.get<PedidoResponse[]>('/Pedidos');
      setListaPedidos(response.data);
    } catch (error) {
      console.error("Erro ao carregar pedidos", error);
      setListaPedidos([]);
    }
  };

  useEffect(() => {
    carregarPedidos();
  }, []);

  const confirmarExclusao = async (id: string) => {
    const resultado = await Swal.fire({
      title: 'Deseja realmente excluir este pedido?',
      text: 'Essa ação não poderá ser desfeita!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#dc3545',
      cancelButtonColor: '#6c757d',
      confirmButtonText: 'Sim, deletar',
      cancelButtonText: 'Cancelar'
    });

    if (resultado.isConfirmed) {
      try {
        await apiClient.delete(`/Pedidos/${id}`);
        await carregarPedidos(); // Recarrega a lista
        Swal.fire('Deletado!', 'O pedido foi excluído com sucesso.', 'success');
      } catch (error) {
        Swal.fire('Erro!', 'Não foi possível excluir o pedido.', 'error');
      }
    }
  };

  return (
    <div className="container mt-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h3>Histórico de Pedidos 📋</h3>
        <button className="btn btn-primary" onClick={() => navigate('/novo-pedido')}>
          Novo Pedido
        </button>
      </div>

      {listaPedidos === null ? (
        <div className="text-center">
          <div className="spinner-border text-primary" role="status"></div>
          <p>Carregando histórico...</p>
        </div>
      ) : listaPedidos.length === 0 ? (
        <div className="alert alert-info text-center">
          Nenhum pedido registrado até o momento. <br />
          <button className="btn btn-link" onClick={() => navigate('/novo-pedido')}>
            Que tal fazer o primeiro?
          </button>
        </div>
      ) : (
        <div className="table-responsive">
          <table className="table table-hover align-middle shadow-sm bg-white">
            <thead className="table-dark">
              <tr>
                <th>Data</th>
                <th>Itens</th>
                <th>Subtotal</th>
                <th>Desconto</th>
                <th>Total Final</th>
                <th className="text-center">Ações</th>
              </tr>
            </thead>
            <tbody>
              {listaPedidos.sort((a, b) => new Date(b.dataCriacao).getTime() - new Date(a.dataCriacao).getTime()).map(pedido => (
                <tr key={pedido.id}>
                  <td>
                    <small className="text-muted">{new Date(pedido.dataCriacao).toLocaleDateString('pt-BR')}</small><br />
                    <strong>{new Date(pedido.dataCriacao).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}</strong>
                  </td>
                  <td>
                    {pedido.itens.map(item => (
                      <span key={item.id} className="badge bg-light text-dark border me-1">
                        {item.nome}
                      </span>
                    ))}
                  </td>
                  <td>R$ {pedido.subtotal.toFixed(2)}</td>
                  <td>
                    {pedido.descontoPercentual > 0 ? (
                      <span className="text-success small">
                        -R$ {pedido.valorDesconto.toFixed(2)}
                        <br />({(pedido.descontoPercentual * 100).toFixed(0)}%)
                      </span>
                    ) : (
                      <span className="text-muted small">Sem desconto</span>
                    )}
                  </td>
                  <td className="fw-bold text-primary">R$ {pedido.totalFinal.toFixed(2)}</td>
                  <td className="text-center">
                    <div className="btn-group">
                      <button className="btn btn-sm btn-outline-info" onClick={() => navigate(`/pedidos/${pedido.id}`)}>
                        Detalhes
                      </button>

                      {/* Controle de acesso baseado na role, igual ao <AuthorizeView Roles="Admin"> */}
                      {usuario?.role === 'Admin' && (
                        <>
                          <button className="btn btn-sm btn-outline-warning" onClick={() => navigate(`/pedidos/editar/${pedido.id}`)}>
                            Editar
                          </button>
                          <button className="btn btn-sm btn-outline-danger" onClick={() => confirmarExclusao(pedido.id)}>
                            Excluir
                          </button>
                        </>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};