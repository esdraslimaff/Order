import React, { useEffect, useState, useMemo } from 'react';
import { apiClient } from '../api/apiClient';
import { type ItemCardapioDto, type PromocaoDto, TipoItemDescricao, type ItemCarrinhoResumo, type OpcaoSelecionadaLocal, type GrupoOpcaoDto } from '../types';
import { MenuCard } from '../components/MenuCard';
import { calcularPedido, type PedidoResumo } from '../utils/calculadoraPedido';

const RESTAURANTE_WHATSAPP = '5511999999999';

export const Home: React.FC = () => {
  const [itensCardapio, setItensCardapio] = useState<ItemCardapioDto[]>([]);
  const [promocoes, setPromocoes] = useState<PromocaoDto[]>([]);
  const [carregando, setCarregando] = useState(true);

  const [carrinho, setCarrinho] = useState<ItemCarrinhoResumo[]>([]);
  const [mensagem, setMensagem] = useState('');

  const [modalAberto, setModalAberto] = useState(false);
  const [itemSelecionado, setItemSelecionado] = useState<ItemCardapioDto | null>(null);
  const [quantidadeItem, setQuantidadeItem] = useState(1);
  const [opcoesSelecionadas, setOpcoesSelecionadas] = useState<OpcaoSelecionadaLocal[]>([]);
  const [observacaoItem, setObservacaoItem] = useState('');

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

  const resumo: PedidoResumo = useMemo(() => {
    const itensPrincipais = carrinho.map(c => ({
      id: c.itemId,
      nome: c.nome,
      precoUnitario: c.precoUnitario * c.quantidade,
      tipo: itensCardapio.find(i => i.id === c.itemId)?.tipo ?? 1,
    } as ItemCardapioDto));
    const subtotal = carrinho.reduce((acc, c) => {
      const totalOpcoes = c.opcoes.reduce((sum, o) => sum + o.precoUnitario * o.quantidade, 0);
      return acc + c.precoUnitario * c.quantidade + totalOpcoes;
    }, 0);
    const resumoPromo = calcularPedido(itensPrincipais, promocoes);
    return {
      ...resumoPromo,
      subtotal,
      totalFinal: subtotal - (resumoPromo.percentualDesconto * subtotal),
      valorDesconto: subtotal * resumoPromo.percentualDesconto
    };
  }, [carrinho, promocoes, itensCardapio]);

  const abrirModal = (item: ItemCardapioDto) => {
    setItemSelecionado(item);
    setQuantidadeItem(1);
    setOpcoesSelecionadas([]);
    setObservacaoItem('');
    setModalAberto(true);
  };

  const handleSelecaoOpcao = (grupo: GrupoOpcaoDto, opcaoId: string, nomeOpcao: string, preco: number, checked: boolean) => {
    if (checked) {
      if (grupo.tipoSelecao === 1) {
        setOpcoesSelecionadas(prev => [
          ...prev.filter(o => o.nomeGrupo !== grupo.nome),
          { opcaoId, nomeOpcao, nomeGrupo: grupo.nome, precoUnitario: preco, quantidade: 1 }
        ]);
      } else {
        setOpcoesSelecionadas(prev => [
          ...prev,
          { opcaoId, nomeOpcao, nomeGrupo: grupo.nome, precoUnitario: preco, quantidade: 1 }
        ]);
      }
    } else {
      setOpcoesSelecionadas(prev => prev.filter(o => o.opcaoId !== opcaoId));
    }
  };

  const handleQuantidadeOpcao = (opcaoId: string, quantidade: number) => {
    setOpcoesSelecionadas(prev => prev.map(o => o.opcaoId === opcaoId ? { ...o, quantidade } : o));
  };

  const adicionarItemAoCarrinho = () => {
    if (!itemSelecionado) return;
    const novoItem: ItemCarrinhoResumo = {
      itemId: itemSelecionado.id,
      nome: itemSelecionado.nome,
      precoUnitario: itemSelecionado.precoUnitario,
      quantidade: quantidadeItem,
      opcoes: opcoesSelecionadas,
      observacao: observacaoItem || undefined
    };
    setCarrinho(prev => [...prev, novoItem]);
    setModalAberto(false);
    setItemSelecionado(null);
    setMensagem('');
  };

  const removerDoCarrinho = (itemId: string) => {
    setCarrinho(prev => prev.filter(x => x.itemId !== itemId));
    setMensagem('');
  };

  const gerarMensagemWhatsApp = (): string => {
    if (carrinho.length === 0) return '';

    let texto = '🍔 *Novo Pedido - Good Hamburger*%0A%0A';

    carrinho.forEach(({ nome, quantidade, precoUnitario, opcoes, observacao }) => {
      const totalOpcoes = opcoes.reduce((sum, o) => sum + o.precoUnitario * o.quantidade, 0);
      const precoItem = precoUnitario * quantidade + totalOpcoes;
      texto += `• ${quantidade}x ${nome} - R$ ${precoItem.toFixed(2)}%0A`;
      const opcoesPorGrupo = opcoes.reduce((acc, o) => {
        if (!acc[o.nomeGrupo]) acc[o.nomeGrupo] = [];
        acc[o.nomeGrupo].push(o);
        return acc;
      }, {} as Record<string, typeof opcoes>);
      Object.entries(opcoesPorGrupo).forEach(([grupo, ops]) => {
        texto += `   ${grupo}:%0A`;
        ops.forEach(o => {
          texto += `     - ${o.quantidade}x ${o.nomeOpcao} (+R$ ${(o.precoUnitario * o.quantidade).toFixed(2)})%0A`;
        });
      });
      if (observacao) texto += `   Obs: ${observacao}%0A`;
    });

    texto += `%0A📦 *Subtotal: R$ ${resumo.subtotal.toFixed(2)}*`;
    if (resumo.valorDesconto > 0) {
      texto += `%0A🎉 *Promoção: ${resumo.nomePromocaoAtiva} (${(resumo.percentualDesconto * 100).toFixed(0)}%)*`;
      texto += `%0A🔻 *Desconto: -R$ ${resumo.valorDesconto.toFixed(2)}*`;
    }
    texto += `%0A💰 *Total final: R$ ${resumo.totalFinal.toFixed(2)}*`;

    if (carrinho.some(c => c.observacao)) {
      texto += `%0A%0A📝 *Observações gerais:*`;
      carrinho.forEach(c => {
        if (c.observacao) texto += `%0A- ${c.nome}: ${c.observacao}`;
      });
    }

    return texto;
  };

  const abrirWhatsApp = () => {
    if (carrinho.length === 0) {
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
        <p className="lead">Escolha seus itens, adicione detalhes e envie pelo WhatsApp.</p>
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
                      <MenuCard item={item} onAdicionar={abrirModal} />
                    </div>
                  ))}
                </div>
              </React.Fragment>
            ))}
          </div>

          <div className="col-md-4">
            <div className="card shadow">
              <div className="card-header bg-white">
                <h5 className="mb-0">🛒 Seu Pedido</h5>
              </div>
              <div className="card-body">
                {carrinho.length === 0 ? (
                  <p className="text-muted">Nenhum item adicionado.</p>
                ) : (
                  <>
                    <ul className="list-group list-group-flush mb-3">
                      {carrinho.map((item) => {
                        const totalOpcoes = item.opcoes.reduce((sum, o) => sum + o.precoUnitario * o.quantidade, 0);
                        const precoItem = item.precoUnitario * item.quantidade + totalOpcoes;
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
                                <span className="me-2">R$ {precoItem.toFixed(2)}</span>
                                <button className="btn btn-sm btn-outline-danger border-0" onClick={() => removerDoCarrinho(item.itemId)}>
                                  ✕
                                </button>
                              </div>
                            </div>
                          </li>
                        );
                      })}
                    </ul>

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
                  disabled={carrinho.length === 0}
                >
                  📲 Enviar pedido via WhatsApp
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {modalAberto && itemSelecionado && (
        <div className="modal d-block" tabIndex={-1} style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">{itemSelecionado.nome}</h5>
                <button type="button" className="btn-close" onClick={() => setModalAberto(false)}></button>
              </div>
              <div className="modal-body">
                <div className="mb-3">
                  <label className="form-label">Quantidade</label>
                  <input type="number" className="form-control" min={1} value={quantidadeItem} onChange={e => setQuantidadeItem(parseInt(e.target.value) || 1)} />
                </div>

                {itemSelecionado.gruposOpcoes.map(grupo => {
                  const opcoesDoGrupo = opcoesSelecionadas.filter(o => o.nomeGrupo === grupo.nome);
                  return (
                    <div key={grupo.id} className="mb-3">
                      <label className="form-label">
                        {grupo.nome} {grupo.obrigatorio && <span className="text-danger">*</span>}
                      </label>
                      <small className="text-muted d-block mb-1">
                        {grupo.tipoSelecao === 1 ? 'Escolha uma opção' : `Escolha até ${grupo.maximoSelecoes ?? 'várias'} opções`}
                      </small>
                      {grupo.opcoes.map(opcao => {
                        const selecionada = opcoesDoGrupo.find(o => o.opcaoId === opcao.id);
                        if (grupo.tipoSelecao === 1) {
                          return (
                            <div key={opcao.id} className="form-check">
                              <input
                                type="radio"
                                className="form-check-input"
                                name={`grupo-${grupo.id}`}
                                checked={!!selecionada}
                                onChange={() => handleSelecaoOpcao(grupo, opcao.id, opcao.nome, opcao.precoAdicional, true)}
                              />
                              <label className="form-check-label">{opcao.nome} {opcao.precoAdicional > 0 && `(+R$ ${opcao.precoAdicional.toFixed(2)})`}</label>
                            </div>
                          );
                        } else {
                          return (
                            <div key={opcao.id} className="d-flex align-items-center mb-2">
                              <input
                                type="checkbox"
                                checked={!!selecionada}
                                onChange={e => handleSelecaoOpcao(grupo, opcao.id, opcao.nome, opcao.precoAdicional, e.target.checked)}
                              />
                              <span className="ms-2">{opcao.nome} {opcao.precoAdicional > 0 && `(+R$ ${opcao.precoAdicional.toFixed(2)})`}</span>
                              {selecionada && (
                                <input
                                  type="number"
                                  className="form-control form-control-sm ms-2"
                                  style={{ width: '70px' }}
                                  min={1}
                                  value={selecionada.quantidade}
                                  onChange={e => handleQuantidadeOpcao(opcao.id, parseInt(e.target.value) || 1)}
                                />
                              )}
                            </div>
                          );
                        }
                      })}
                    </div>
                  );
                })}

                <div className="mb-3">
                  <label className="form-label">Observação</label>
                  <textarea className="form-control" rows={2} value={observacaoItem} onChange={e => setObservacaoItem(e.target.value)} placeholder="Ex.: sem cebola, bem passado..."></textarea>
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn btn-secondary" onClick={() => setModalAberto(false)}>Cancelar</button>
                <button className="btn btn-primary" onClick={adicionarItemAoCarrinho}>Adicionar ao pedido</button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};