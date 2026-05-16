import React, { useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { type PromocaoDto, TipoItemDescricao } from '../types';
import { AuthContext } from '../contexts/AuthContext';

interface PromocaoCardProps {
  promocao: PromocaoDto;
  onStatusToggled: (promocao: PromocaoDto) => void;
}

export const PromocaoCard: React.FC<PromocaoCardProps> = ({ promocao, onStatusToggled }) => {
  const navigate = useNavigate();
  const { usuario } = useContext(AuthContext);

  return (
    <div className="col-md-4 mb-4">
      <div className={`card h-100 border-${promocao.ativo ? 'danger' : 'secondary'} shadow-lg ${promocao.ativo ? '' : 'opacity-75'}`}>
        <div className="card-body text-center">
          
          <div className={`badge bg-${promocao.ativo ? 'danger' : 'secondary'} mb-2 p-2`} style={{ fontSize: '1.2rem' }}>
            {(promocao.percentual * 100).toFixed(0)}% OFF
          </div>

          {!promocao.ativo && (
            <div className="badge bg-dark mb-2 ms-2 p-2">INATIVA</div>
          )}

          <h4 className="card-title fw-bold">{promocao.nome}</h4>
          <p className="text-muted small">Para ganhar, seu pedido deve conter:</p>
          <hr />

          <div className="d-flex flex-wrap justify-content-center gap-2">
            {promocao.requisitos.map((req, index) => (
              <span key={index} className="badge rounded-pill bg-warning text-dark">
                {TipoItemDescricao[req] || 'Item'}
              </span>
            ))}
          </div>
        </div>

        <div className="card-footer bg-transparent border-0 pb-3 text-center d-flex flex-column gap-2">
          <button 
            className="btn btn-outline-danger" 
            onClick={() => navigate('/')} 
            disabled={!promocao.ativo}
          >
            Aproveitar Agora
          </button>

          {/* O equivalente ao <AuthorizeView Roles="Admin"> */}
          {usuario?.role === 'Admin' && (
            <>
              <hr className="m-1" />
              <button 
                className={`btn btn-sm ${promocao.ativo ? 'btn-secondary' : 'btn-success'}`} 
                onClick={() => onStatusToggled(promocao)}
              >
                {promocao.ativo ? 'Desativar Promoção' : 'Ativar Promoção'}
              </button>
            </>
          )}
        </div>
      </div>
    </div>
  );
};