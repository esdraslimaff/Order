import React, { useEffect, useState, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { apiClient } from '../api/apiClient';
import { type ItemCardapioDto, type PromocaoDto, type PedidoRequest, type ItemCarrinhoResumo, TipoItemDescricao, type PedidoResponse, type OpcaoSelecionadaLocal, type GrupoOpcaoDto } from '../types';
import { MenuCard } from '../components/MenuCard';
import { CarrinhoResumo } from '../components/CarrinhoResumo';
import { calcularPedido, type PedidoResumo } from '../utils/calculadoraPedido';

export const EditarPedido: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [itensCardapio, setItensCardapio] = useState<ItemCardapioDto[]>([]);
  const [promocoes, setPromocoes] = useState<PromocaoDto[]>([]);
  const [carregando, setCarregando] = useState<boolean>(true);

  const [carrinho, setCarrinho] = useState<ItemCarrinhoResumo[]>([]);
  const [descontoRegistrado, setDescontoRegistrado] = useState<number>(0);
  const [foiAlterado, setFoiAlterado] = useState<boolean>(false);
  const [processando, setProcessando] = useState<boolean>(false);
  const [mensagemErro, setMensagemErro] = useState<string>('');

  const [resumo, setResumo] = useState<PedidoResumo>({
    subtotal: 0, percentualDesconto: 0, nomePromocaoAtiva: '', valorDesconto: 0, totalFinal: 0
  });

  const [modalAberto, setModalAberto] = useState(false);
  const [itemSelecionado, setItemSelecionado] = useState<ItemCardapioDto | null>(null);
  const [quantidadeItem, setQuantidadeItem] = useState(1);
  const [opcoesSelecionadas, setOpcoesSelecionadas] = useState<OpcaoSelecionadaLocal[]>([]);
  const [observacaoItem, setObservacaoItem] = useState('');

  useEffect(() => {
    const carregarDados = async () => {
      try {
        const [resCardapio, resPedido] = await Promise.all([
          apiClient.get<ItemCardapioDto[]>('/Cardapio'),
          apiClient.get<PedidoResponse>(`/Pedidos/${id}`)
        ]);
        setItensCardapio(resCardapio.data);
        const pedido = resPedido.data;
        setDescontoRegistrado(pedido.descontoPercentual);

        const itensCarrinho: ItemCarrinhoResumo[] = pedido.itens.map(item => ({
          itemId: item.produtoId,
          nome: item.nome,
          precoUnitario: item.precoUnitario,
          quantidade: item.quantidade,
          opcoes: item.opcoesSelecionadas.map(o => ({
            opcaoId: o.id,
            nomeOpcao: o.nomeOpcao,
            nomeGrupo: o.nomeGrupo,
            precoUnitario: o.precoUnitario,
            quantidade: o.quantidade
          })),
          observacao: item.observacao
        }));
        setCarrinho(itensCarrinho);

        if (pedido.promocaoId) {
          try {
            const resPromo = await apiClient.get<PromocaoDto>(`/Promocao/${pedido.promocaoId}`);
            setPromocoes([resPromo.data]);
          } catch (e) {
            console.warn("Promoção original não encontrada");
            const resPromocoes = await apiClient.get<PromocaoDto[]>('/Promocao/PromocoesAtivas');
            setPromocoes(resPromocoes.data);
          }
        } else {
          const resPromocoes = await apiClient.get<PromocaoDto[]>('/Promocao/PromocoesAtivas');
          setPromocoes(resPromocoes.data);
        }
      } catch (error) {
        setMensagemErro("Erro ao carregar dados do pedido.");
      } finally {
        setCarregando(false);
      }
    };
    if (id) carregarDados();
  }, [id]);

  useEffect(() => {
    if (!carregando) {
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
      const resumoPromo = calcularPedido(itensPrincipais, promocoes, descontoRegistrado, foiAlterado);
      setResumo({
        ...resumoPromo,
        subtotal,
        totalFinal: subtotal - (resumoPromo.percentualDesconto * subtotal),
        valorDesconto: subtotal * resumoPromo.percentualDesconto
      });
    }
  }, [carrinho, promocoes, carregando, descontoRegistrado, foiAlterado, itensCardapio]);

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
    setFoiAlterado(true);
  };

  const removerDoCarrinho = (itemId: string) => {
    setCarrinho(prev => prev.filter(x => x.itemId !== itemId));
    setFoiAlterado(true);
  };

  const salvarAlteracoes = async () => {
    if (carrinho.length === 0) {
      setMensagemErro('Sua bandeja não pode estar vazia!');
      return;
    }
    try {
      setProcessando(true);
      const request: PedidoRequest = {
        itens: carrinho.map(c => ({
          itemId: c.itemId,
          quantidade: c.quantidade,
          opcoesSelecionadas: c.opcoes.map(o => ({ opcaoId: o.opcaoId, quantidade: o.quantidade })),
          observacao: c.observacao
        }))
      };
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
                      <MenuCard item={item} onAdicionar={abrirModal} />
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