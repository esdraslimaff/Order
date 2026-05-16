import React, { useEffect, useState, useMemo } from 'react';
import { apiClient } from '../api/apiClient';
// Agora importamos também os tipos de promoção e a calculadora
import { type ItemCardapioDto, type PromocaoDto, TipoItemDescricao } from '../types';
import { MenuCard } from '../components/MenuCard';
import { calcularPedido, type PedidoResumo } from '../utils/calculadoraPedido';

interface ItemCarrinho {
  item: ItemCardapioDto;
  quantidade: number;
}

const RESTAURANTE_WHATSAPP = '5511999999999'; // Substitua pelo número real

export const Home: React.FC = () => {
  const [itensCardapio, setItensCardapio] = useState<ItemCardapioDto[]>([]);
  const [promocoes, setPromocoes] = useState<PromocaoDto[]>([]); // <- NOVO
  const [carregando, setCarregando] = useState(true);
  const [carrinho, setCarrinho] = useState<Map<string, ItemCarrinho>>(new Map());
  const [mensagem, setMensagem] = useState('');

  // Carrega cardápio e promoções (como no NovoPedido)
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
        console.error('Erro ao carregar dados', error);
        setMensagem('Erro ao carregar o cardápio. Tente novamente mais tarde.');
      } finally {
        setCarregando(false);
      }
    };
    carregarDados();
  }, []);

  const adicionarAoCarrinho = (item: ItemCardapioDto) => {
    setMensagem('');
    setCarrinho(prev => {
      const novoMap = new Map(prev);
      const existente = novoMap.get(item.id);
      if (existente) {
        novoMap.set(item.id, { ...existente, quantidade: existente.quantidade + 1 });
      } else {
        novoMap.set(item.id, { item, quantidade: 1 });
      }
      return novoMap;
    });
  };

  const removerDoCarrinho = (itemId: string) => {
    setMensagem('');
    setCarrinho(prev => {
      const novoMap = new Map(prev);
      const existente = novoMap.get(itemId);
      if (existente && existente.quantidade > 1) {
        novoMap.set(itemId, { ...existente, quantidade: existente.quantidade - 1 });
      } else {
        novoMap.delete(itemId);
      }
      return novoMap;
    });
  };

  const carrinhoArray = useMemo(() => Array.from(carrinho.values()), [carrinho]);

  // Extrai apenas os itens únicos (sem duplicar por quantidade)
  const itensUnicos = useMemo(() => {
    const map = new Map<string, ItemCardapioDto>();
    carrinhoArray.forEach(ic => map.set(ic.item.id, ic.item));
    return Array.from(map.values());
  }, [carrinhoArray]);

  // Calcula o resumo com base nos itens únicos e nas promoções ativas
  const resumo: PedidoResumo = useMemo(
    () => calcularPedido(itensUnicos, promocoes),
    [itensUnicos, promocoes]
  );

  // Gera a mensagem para o WhatsApp incluindo o desconto
  const gerarMensagemWhatsApp = (): string => {
    if (carrinho.size === 0) return '';

    let texto = '🍔 *Novo Pedido - Good Hamburger*%0A%0A';
    
    carrinhoArray.forEach(({ item, quantidade }) => {
      texto += `• ${quantidade}x ${item.nome} - R$ ${(item.precoUnitario * quantidade).toFixed(2)}%0A`;
    });

    texto += `%0A📦 *Subtotal: R$ ${resumo.subtotal.toFixed(2)}*`;

    if (resumo.valorDesconto > 0) {
      texto += `%0A🎉 *Promoção: ${resumo.nomePromocaoAtiva}*`;
      texto += `%0A🔻 *Desconto: -R$ ${resumo.valorDesconto.toFixed(2)}*`;
    }

    texto += `%0A💰 *Total final: R$ ${resumo.totalFinal.toFixed(2)}*`;
    return texto;
  };

  const abrirWhatsApp = () => {
    if (carrinho.size === 0) {
      setMensagem('Adicione pelo menos um item ao pedido.');
      return;
    }
    const mensagemPronta = gerarMensagemWhatsApp();
    const url = `https://wa.me/${RESTAURANTE_WHATSAPP}?text=${mensagemPronta}`;
    window.open(url, '_blank');
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
      <div className="text-center mb-5">
        <h2 className="display-5 fw-bold text-primary">Nosso Cardápio 🍔</h2>
        <p className="lead">Escolha seus itens e envie o pedido pelo WhatsApp.</p>
      </div>

      {carregando ? (
        <div className="text-center">
          <div className="spinner-border text-primary" role="status"></div>
        </div>
      ) : (
        <div className="row">
          <div className="col-md-8">
            {(Object.entries(itensAgrupados) as [string, ItemCardapioDto[]][]).map(([tipo, itens]) => (
              <React.Fragment key={tipo}>
                <h4 className="mt-4 border-bottom pb-2 text-secondary">{tipo}s</h4>
                <div className="row">
                  {itens.map(item => (
                    <div key={item.id} className="col-md-6 mb-3">
                      <MenuCard item={item} onAdicionar={adicionarAoCarrinho} />
                    </div>
                  ))}
                </div>
              </React.Fragment>
            ))}
          </div>

          {/* Painel lateral do pedido */}
          <div className="col-md-4">
            <div className="card shadow">
              <div className="card-header bg-white">
                <h5 className="mb-0">🛒 Seu Pedido</h5>
              </div>
              <div className="card-body">
                {carrinho.size === 0 ? (
                  <p className="text-muted">Nenhum item adicionado.</p>
                ) : (
                  <>
                    <ul className="list-group list-group-flush mb-3">
                      {carrinhoArray.map(({ item, quantidade }) => (
                        <li key={item.id} className="list-group-item d-flex justify-content-between align-items-center">
                          <div>
                            {item.nome}
                            <br />
                            <small className="text-muted">{quantidade}x R$ {item.precoUnitario.toFixed(2)}</small>
                          </div>
                          <button
                            className="btn btn-sm btn-outline-danger"
                            onClick={() => removerDoCarrinho(item.id)}
                          >
                            ✕
                          </button>
                        </li>
                      ))}
                    </ul>

                    {/* Resumo com promoção */}
                    <div className="border-top pt-2">
                      <p className="mb-1">Subtotal: R$ {resumo.subtotal.toFixed(2)}</p>
                      {resumo.valorDesconto > 0 && (
                        <>
                          <p className="mb-1 text-success">
                            🎉 {resumo.nomePromocaoAtiva} ({resumo.percentualDesconto*100}%)
                          </p>
                          <p className="mb-1 text-danger">
                            Desconto: -R$ {resumo.valorDesconto.toFixed(2)}
                          </p>
                        </>
                      )}
                      <p className="fw-bold">Total final: R$ {resumo.totalFinal.toFixed(2)}</p>
                    </div>
                  </>
                )}
                {mensagem && (
                  <div className="alert alert-warning mt-2 py-1">{mensagem}</div>
                )}
                <button
                  className="btn btn-success w-100 mt-2"
                  onClick={abrirWhatsApp}
                  disabled={carrinho.size === 0}
                >
                  📲 Enviar pedido via WhatsApp
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};