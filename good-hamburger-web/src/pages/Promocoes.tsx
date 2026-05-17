import React, { useEffect, useState } from 'react';
import { apiClient } from '../api/apiClient';
import { type PromocaoDto } from '../types';
import { PromocaoCard } from '../components/PromocaoCard';

export const Promocoes: React.FC = () => {
  const [promocoes, setPromocoes] = useState<PromocaoDto[] | null>(null);

  useEffect(() => {
    const carregarPromocoes = async () => {
      try {
        const response = await apiClient.get<PromocaoDto[]>('/Promocao');
        setPromocoes(response.data);
      } catch (error) {
        console.error('Erro ao carregar promoções', error);
        setPromocoes([]);
      }
    };
    carregarPromocoes();
  }, []);

  const alternarStatusPromocao = async (promo: PromocaoDto) => {
    try {
      await apiClient.patch(`/Promocao/${promo.id}/alternar-status`);
      setPromocoes(prev =>
        prev ? prev.map(p => p.id === promo.id ? { ...p, ativo: !p.ativo } : p) : null
      );
    } catch (error) {
      console.error('Erro ao alterar status', error);
    }
  };

  return (
    <div className="container mt-4">
      <div className="text-center mb-5">
        <h2 className="display-5 fw-bold text-danger">Ofertas Irresistíveis! 🔥</h2>
        <p className="lead">Combine e economize na Good Hamburger.</p>
      </div>

      <div className="card-body p-4 text-center mb-4">
        <h4 className="text-danger fw-bold mb-4">📌 O que você precisa saber sobre nossos Combos</h4>
        <div className="row text-start justify-content-center">
          <div className="col-md-5 border-end">
            <h5 className="fw-bold">🎁 Como ganhar o desconto?</h5>
            <p>Escolha <strong>exatamente</strong> os itens descritos no combo. O desconto é aplicado no valor total desses itens.</p>
          </div>
          <div className="col-md-5 ms-md-3">
            <h5 className="fw-bold text-warning">⚠️ Dica para Edição</h5>
            <p>Nossas promoções são aplicadas a combos com categorias específicas. Você pode substituir itens dentro da mesma categoria.</p>
          </div>
        </div>
      </div>

      {promocoes === null ? (
        <div className="text-center">
          <div className="spinner-grow text-danger" role="status"></div>
        </div>
      ) : promocoes.length === 0 ? (
        <div className="text-center py-5">
          <h5 className="text-muted">😕 Nenhuma promoção disponível no momento</h5>
        </div>
      ) : (
        <div className="row justify-content-center">
          {promocoes.map(promo => (
            <PromocaoCard key={promo.id} promocao={promo} onStatusToggled={alternarStatusPromocao} />
          ))}
        </div>
      )}
    </div>
  );
};