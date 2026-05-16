import React, { useEffect, useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiClient } from '../api/apiClient';
import { type ItemCardapioDto, type PromocaoDto, type PedidoRequest, TipoItemDescricao } from '../types';
import { MenuCard } from '../components/MenuCard';
import { CarrinhoResumo } from '../components/CarrinhoResumo';
import { calcularPedido, type PedidoResumo } from '../utils/calculadoraPedido';

export const NovoPedido: React.FC = () => {
  const navigate = useNavigate();

  // Estados dos dados (Servidor)
  const [itensCardapio, setItensCardapio] = useState<ItemCardapioDto[]>([]);
  const [promocoes, setPromocoes] = useState<PromocaoDto[]>([]);
  const [carregando, setCarregando] = useState<boolean>(true);

  // Estados do usuário (Local)
  const [carrinho, setCarrinho] = useState<ItemCardapioDto[]>([]);
  const [mensagemErro, setMensagemErro] = useState<string>('');
  const [processando, setProcessando] = useState<boolean>(false);
  const [resumo, setResumo] = useState<PedidoResumo>({
    subtotal: 0,
    percentualDesconto: 0,
    nomePromocaoAtiva: 'Sem promoção aplicável',
    valorDesconto: 0,
    totalFinal: 0
  });

  // Carregamento inicial (OnInitializedAsync)
  useEffect(() => {
    const carregarDados = async () => {
      try {
        const [resCardapio, resPromocoes] = await Promise.all([
          apiClient.get<ItemCardapioDto[]>('/Cardapio'),
          apiClient.get<PromocaoDto[]>('/Promocao/PromocoesAtivas')
        ]);
        setItensCardapio(resCardapio.data);
        setPromocoes(resPromocoes.data);
      } catch (error) {
        setMensagemErro('Erro ao carregar o cardápio. Verifique se a API está rodando.');
      } finally {
        setCarregando(false);
      }
    };
    carregarDados();
  }, []);

  // Efeito disparado sempre que o carrinho ou as promoções mudarem
  useEffect(() => {
    const novoResumo = calcularPedido(carrinho, promocoes);
    setResumo(novoResumo);
  }, [carrinho, promocoes]);

  // Regras de negócio
  const adicionarAoCarrinho = (item: ItemCardapioDto) => {
    setMensagemErro('');

    if (carrinho.length >= 3) {
      setMensagemErro('Cada pedido pode conter no máximo 3 itens.');
      return;
    }

    if (carrinho.some(x => x.tipo === item.tipo)) {
      setMensagemErro(`Você já adicionou um item da categoria ${TipoItemDescricao[item.tipo] || 'sem categoria'}.`);
      return;
    }

    setCarrinho(prev => [...prev, item]);
  };

  const removerDoCarrinho = (item: ItemCardapioDto) => {
    setMensagemErro('');
    setCarrinho(prev => prev.filter(x => x.id !== item.id));
  };

  const finalizarPedido = async () => {
    if (carrinho.length === 0) {
      setMensagemErro('Sua bandeja está vazia!');
      return;
    }

    try {
      setProcessando(true);
      const request: PedidoRequest = { itensIds: carrinho.map(x => x.id) };
      const resultado = await apiClient.post('/Pedidos', request);
      
      // Redireciona para a página de detalhes após o sucesso
      if (resultado.data && resultado.data.id) {
        navigate(`/pedidos/${resultado.data.id}`);
      }
    } catch (error: any) {
      setMensagemErro('Erro ao processar pedido: ' + (error.response?.data || error.message));
    } finally {
      setProcessando(false);
    }
  };

  // Agrupamento para exibição visual
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
      <div className="row">
        <div className="col-md-8">
          <h3 className="mb-4">Cardápio Good Hamburger 🍔</h3>

          {carregando ? (
            <div className="d-flex justify-content-center my-5">
              <div className="spinner-border text-primary" role="status"></div>
            </div>
          ) : (
            (Object.entries(itensAgrupados) as [string, ItemCardapioDto[]][]).map(([tipo, itens]) => (
              <React.Fragment key={tipo}>
                <h5 className="mt-4 border-bottom pb-2">{tipo}s</h5>
                <div className="row">
                  {itens.map(item => (
                    <div key={item.id} className="col-md-6 mb-3">
                      <MenuCard item={item} onAdicionar={adicionarAoCarrinho} />
                    </div>
                  ))}
                </div>
              </React.Fragment>
            ))
          )}
        </div>

        <div className="col-md-4">
          <CarrinhoResumo 
            itensNoCarrinho={carrinho}
            resumo={resumo}
            processando={processando}
            mensagemErro={mensagemErro}
            onRemover={removerDoCarrinho}
            onFinalizar={finalizarPedido}
          />
        </div>
      </div>
    </div>
  );
};