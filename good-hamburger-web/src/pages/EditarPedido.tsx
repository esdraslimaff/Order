import React, { useEffect, useState, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { apiClient } from '../api/apiClient';
import { type ItemCardapioDto, type PromocaoDto, type PedidoRequest, TipoItemDescricao } from '../types';
import { MenuCard } from '../components/MenuCard';
import { CarrinhoResumo } from '../components/CarrinhoResumo';
import { calcularPedido, type PedidoResumo } from '../utils/calculadoraPedido';

export const EditarPedido: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [itensCardapio, setItensCardapio] = useState<ItemCardapioDto[]>([]);
  const [promocoesPermitidas, setPromocoesPermitidas] = useState<PromocaoDto[]>([]);
  const [carregando, setCarregando] = useState<boolean>(true);

  // Estados locais
  const [carrinho, setCarrinho] = useState<ItemCardapioDto[]>([]);
  const [descontoRegistrado, setDescontoRegistrado] = useState<number>(0);
  const [foiAlterado, setFoiAlterado] = useState<boolean>(false);
  const [processando, setProcessando] = useState<boolean>(false);
  const [mensagemErro, setMensagemErro] = useState<string>('');
  
  const [resumo, setResumo] = useState<PedidoResumo>({
    subtotal: 0, percentualDesconto: 0, nomePromocaoAtiva: '', valorDesconto: 0, totalFinal: 0
  });

  useEffect(() => {
    const carregarDadosIniciais = async () => {
      try {
        const [resCardapio, resPedido] = await Promise.all([
          apiClient.get<ItemCardapioDto[]>('/Cardapio'),
          apiClient.get(`/Pedidos/${id}`)
        ]);

        setItensCardapio(resCardapio.data);
        const pedido = resPedido.data;

        setCarrinho(pedido.itens || []);
        setDescontoRegistrado(pedido.descontoPercentual);

        // Se o pedido original tinha uma promoção, buscamos ela para manter a regra
        if (pedido.promocaoId) {
          try {
            const resPromo = await apiClient.get<PromocaoDto>(`/Promocao/${pedido.promocaoId}`);
            setPromocoesPermitidas([resPromo.data]);
          } catch (e) {
            console.warn("Promoção original não encontrada ou inativa");
          }
        }
      } catch (error) {
        setMensagemErro("Erro ao carregar dados do pedido.");
      } finally {
        setCarregando(false);
      }
    };

    if (id) carregarDadosIniciais();
  }, [id]);

  // Recalcula sempre que a bandeja sofre alteração
  useEffect(() => {
    if (!carregando) {
      setResumo(calcularPedido(carrinho, promocoesPermitidas, descontoRegistrado, foiAlterado));
    }
  }, [carrinho, promocoesPermitidas, descontoRegistrado, foiAlterado, carregando]);

  const adicionarAoCarrinho = (item: ItemCardapioDto) => {
    setMensagemErro('');
    if (carrinho.length >= 3) {
      setMensagemErro('Cada pedido pode conter no máximo 3 itens.');
      return;
    }
    if (carrinho.some(x => x.tipo === item.tipo)) {
      setMensagemErro(`Você já adicionou um item da categoria ${TipoItemDescricao[item.tipo]}.`);
      return;
    }
    setCarrinho(prev => [...prev, item]);
    setFoiAlterado(true);
  };

  const removerDoCarrinho = (item: ItemCardapioDto) => {
    setMensagemErro('');
    setCarrinho(prev => prev.filter(x => x.id !== item.id));
    setFoiAlterado(true);
  };

  const salvarAlteracoes = async () => {
    if (carrinho.length === 0) {
      setMensagemErro('Sua bandeja não pode estar vazia!');
      return;
    }

    try {
      setProcessando(true);
      const request: PedidoRequest = { itensIds: carrinho.map(x => x.id) };
      await apiClient.put(`/Pedidos/${id}`, request);
      navigate('/pedidos');
    } catch (error: any) {
      setMensagemErro('Erro ao atualizar: ' + (error.response?.data || error.message));
    } finally {
      setProcessando(false);
    }
  };

  const itensAgrupados = useMemo(() => {
    return itensCardapio.reduce((acc, item) => {
      const tipoStr = TipoItemDescricao[item.tipo] || 'Outros';
      if (!acc[tipoStr]) acc[tipoStr] = [];
      acc[tipoStr].push(item);
      return acc;
    }, {} as Record<string, ItemCardapioDto[]>);
  }, [itensCardapio]);

  return (
    <div className="container mt-4">
      {carregando ? (
        <div className="text-center py-5">
          <div className="spinner-border text-primary" role="status"></div>
          <p className="mt-2">Montando sua bandeja...</p>
        </div>
      ) : (
        <div className="row">
          <div className="col-md-8">
            <div className="d-flex justify-content-between align-items-center mb-4">
              <h3 className="fw-bold">Editar Pedido 🍔</h3>
              <button className="btn btn-outline-secondary btn-sm" onClick={() => navigate('/pedidos')}>Cancelar</button>
            </div>

            {(Object.entries(itensAgrupados) as [string, ItemCardapioDto[]][]).map(([tipo, itens]) => (
              <React.Fragment key={tipo}>
                <h5 className="text-muted border-bottom pb-2 mt-4">{tipo}s</h5>
                <div className="row g-3">
                  {itens.map(item => (
                    <div className="col-md-6" key={item.id}>
                      <MenuCard item={item} onAdicionar={adicionarAoCarrinho} />
                    </div>
                  ))}
                </div>
              </React.Fragment>
            ))}
          </div>

          <div className="col-md-4">
            <div className="sticky-top" style={{ top: '20px' }}>
              <CarrinhoResumo 
                itensNoCarrinho={carrinho}
                resumo={resumo}
                processando={processando}
                mensagemErro={mensagemErro}
                onRemover={removerDoCarrinho}
                onFinalizar={salvarAlteracoes}
              />
            </div>
          </div>
        </div>
      )}
    </div>
  );
};