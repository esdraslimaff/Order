import React from 'react';
// Adicione a palavra 'type' antes do DTO e importe o dicionário
import { type ItemCardapioDto, TipoItemDescricao } from '../types';

interface MenuCardProps {
  item: ItemCardapioDto;
  onAdicionar: (item: ItemCardapioDto) => void;
}

export const MenuCard: React.FC<MenuCardProps> = ({ item, onAdicionar }) => {
  return (
    <div className="card shadow-sm h-100">
      <div className="card-body d-flex flex-column">
        <div className="d-flex justify-content-between align-items-start">
          <h5 className="card-title">{item.nome}</h5>
          {/* Garanta que aqui está usando o TipoItemDescricao */}
          <span className="badge bg-secondary">{TipoItemDescricao[item.tipo]}</span>
        </div>
        <p className="card-text text-primary fw-bold mt-2">
          {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(item.precoUnitario)}
        </p>
        <button 
          className="btn btn-outline-primary mt-auto w-100"
          onClick={() => onAdicionar(item)}
        >
          <i className="bi bi-plus-lg"></i> Adicionar
        </button>
      </div>
    </div>
  );
};